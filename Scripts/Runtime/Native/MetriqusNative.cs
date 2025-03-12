using MetriqusSdk.Storage;
using MetriqusSdk.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using UnityEngine;

namespace MetriqusSdk
{
    internal abstract class MetriqusNative
    {
        private const string FirstLaunchKey = "metriqus_first_launch";
        private const string LastSessionStartTimeKey = "metriqus_last_session_start_time";
        private const string SessionIdKey = "metriqus_session_id";
        private const string LastSendAttributionDateKey = "metriqus_last_send_attribution_date";
        private const string RemoteSettingsKey = "metriqus_remote_settings";
        private const string GeolocationKey = "geolocation_settings";
        private const string GeolocationLastFetchedTimeKey = "geolocation_last_fetched_time";

        private IPackageSender packageSender;
        private DeviceInfo deviceInfo;
        private MetriqusRemoteSettings remoteSettings = null;
        private IPGeolocation.Geolocation geolocation = null;
        private InternetConnectionChecker internetConnectionChecker;
        protected UniqueUserIdentifier uniqueUserIdentifier = null;
        protected UserAttributes userAttributes = null;
        protected IStorage storage;
        protected MetriqusSettings metriqusSettings;

        protected bool isTrackingEnabled = false;
        private bool isInitialized = false;
        private bool isFirstLaunch = false;
        private bool remoteSettingsFetched = false;
        private bool geolocationFetched = false;
        private string sessionId;
        protected string adId = null;

        protected Action onFirstLaunch;

        public bool IsTrackingEnabled => isTrackingEnabled;
        public bool IsFirstLaunch => isFirstLaunch;
        public bool IsInitialized => isInitialized;
        public string SessionId => sessionId;
        public DeviceInfo DeviceInfo => deviceInfo;
        public UniqueUserIdentifier UniqueUserIdentifier => uniqueUserIdentifier;
        public UserAttributes UserAttributes => userAttributes;
        public InternetConnectionChecker InternetConnectionChecker => internetConnectionChecker;

        internal async virtual void InitSdk(MetriqusSettings metriqusSettings)
        {
            try
            {
                //Metriqus.DebugLog("Native Base Init SDK");
                this.metriqusSettings = metriqusSettings;
                storage = new Storage.Storage(new EncryptedStorageHandler());
                deviceInfo = new DeviceInfo();
                packageSender = new MetriqusPackageSender();
                internetConnectionChecker = new InternetConnectionChecker();

                uniqueUserIdentifier = new UniqueUserIdentifier(storage, adId, deviceInfo.deviceId);
                userAttributes = new UserAttributes(storage);

                internetConnectionChecker.OnConnectedToInternet += OnConnectedToInternet;

                // Wait for fetching remote settings and fetching geo location

                await FetchRemoteSettings();
                await FetchGeolocation();

                MetriqusLogger.Init(storage);

                isInitialized = true;

                ProcessIsFirstLaunch();

                ProcessSession();

                ProcessAttribution();
            }
            catch (Exception e)
            {
                if (Metriqus.LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog("Error while initializing Native: " + e, LogType.Error);
            }
        }

        internal string GetAdid() => adId;

        internal abstract void ReadAdid(Action<string> callback);
        internal abstract void ReadAttribution(Action<MetriqusAttribution> onReadCallback, Action<string> onError);
        internal abstract void GetInstallTime(Action<long> callback);

        private async void OnConnectedToInternet()
        {
            /*if (Metriqus.LogLevel != LogLevel.NoLog)
                Metriqus.DebugLog("OnConnectedToInternet");*/

            List<Task> tasks = new();

            if (!remoteSettingsFetched)
            {
                tasks.Add(FetchRemoteSettings());
            }

            if (!geolocationFetched)
            {
                tasks.Add(FetchGeolocation());
            }

            await Task.WhenAll(tasks);
        }

        internal void TrackAdRevenue(MetriqusAdRevenue adRevenue)
        {
            packageSender.SendAdRevenuePackage(adRevenue);
        }

        internal void TrackIAPEvent(MetriqusInAppRevenue metriqusEvent)
        {
            packageSender.SendIAPEventPackage(metriqusEvent);
        }

        internal void TrackCustomEvent(MetriqusCustomEvent _event)
        {
            packageSender.SendCustomPackage(_event);
        }

        internal void SetUserAttribute(TypedParameter parameter)
        {
            userAttributes.AddUserAttribute(parameter);
        }

        internal void RemoveUserAttribute(string key)
        {
            userAttributes.RemoveUserAttribute(key);
        }

        internal void SendSessionBeatEvent()
        {
            packageSender.SendSessionBeatPackage();
        }

        internal virtual void UpdateConversionValue(int value)
        {

        }

        internal void OnPause()
        {
        }

        internal void OnResume()
        {
            if (Metriqus.LogLevel != LogLevel.NoLog)
                Metriqus.DebugLog("Application resumed. Processing session.");
            ProcessSession();
        }

        internal void OnQuit()
        {

        }

        private void ProcessSession()
        {
            DateTime currentTime = DateTime.UtcNow;

            // check is this first session by checking lastSessionStartTimeKey exist
            bool isLastSessionStartTimeSaved = storage.CheckKeyExist(LastSessionStartTimeKey);

            // if LastSessionStartTimeKey already saved, it means this is not first session
            if (isLastSessionStartTimeSaved == true)
            {
                // THIS IS NOT FIRST SESSION
                string lastSessionStartTimeStr = storage.LoadData(LastSessionStartTimeKey);
                DateTime lastSessionStartTime = MetriqusUtils.ParseDate(lastSessionStartTimeStr);

                var remoteSettings = GetMetriqusRemoteSettings();

                double passedMinutesSinceLastSession = (currentTime.Subtract(lastSessionStartTime)).TotalMinutes;

                if (Metriqus.LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog("Passed Minutes Since Last Session: " + passedMinutesSinceLastSession);

                if (passedMinutesSinceLastSession >= remoteSettings.SessionIntervalMinutes)
                {
                    sessionId = Guid.NewGuid().ToString();
                    storage.SaveData(SessionIdKey, sessionId);

                    packageSender.SendSessionStartPackage();
                }
                else
                {
                    bool isSessionIdKeyExist = storage.CheckKeyExist(SessionIdKey);

                    if (isSessionIdKeyExist)
                    {
                        sessionId = storage.LoadData(SessionIdKey);
                    }
                    else
                    {
                        sessionId = Guid.NewGuid().ToString();
                    }
                }
            }
            else
            {
                // THIS IS THE FIRST SESSION
                sessionId = Guid.NewGuid().ToString();
                storage.SaveData(SessionIdKey, sessionId);

                packageSender.SendSessionStartPackage();
            }

            storage.SaveData(LastSessionStartTimeKey, MetriqusUtils.ConvertDateToString(currentTime));
        }

        private void ProcessAttribution()
        {
            /*if (Metriqus.LogLevel != LogLevel.NoLog)
                Metriqus.DebugLog("ProcessAttribution");*/

            // Cancel attribution on ios platform if tracking disabled
            if (isTrackingEnabled == false)
            {
                Metriqus.DebugLog("ProcessAttribution canceled: user not allowed tracking");
                return;
            }

#if UNITY_IOS
            // Cancel attribution on ios platform if tracking disabled
            if (metriqusSettings.iOSUserTrackingDisabled)
            {
                Metriqus.DebugLog("ProcessAttribution canceled: iOS User Tracking Disabled");
                return;
            }
#endif

            DateTime currentDate = DateTime.UtcNow;

            void sendAttr()
            {
                ReadAttribution(
                    (attribution) =>
                    {
                        if (Metriqus.LogLevel != LogLevel.NoLog)
                            Metriqus.DebugLog("Attribution read successfuly: " + JsonUtility.ToJson(attribution));

                        packageSender.SendAttributionPackage(attribution);

                        storage.SaveData(LastSendAttributionDateKey, MetriqusUtils.ConvertDateToString(currentDate));
                    },
                    (error) =>
                    {
                        if (Metriqus.LogLevel != LogLevel.NoLog)
                            Metriqus.DebugLog("Attribution read error: " + error, LogType.Warning);
                    });
            }
            ;

            GetInstallTime((installTime) =>
            {
                DateTime installDate = DateTimeOffset.FromUnixTimeMilliseconds(installTime).DateTime;

                bool lastSendAttributionDateExist = storage.CheckKeyExist(LastSendAttributionDateKey);

                var remoteSettings = GetMetriqusRemoteSettings();

                if (currentDate.Subtract(installDate).Days < remoteSettings.AttributionCheckWindow)
                {
                    // if it has been 20 days send attribution 
                    sendAttr();
                }
                else if (!lastSendAttributionDateExist)
                {
                    // if didnt send any attribution send it
                    sendAttr();
                }
                else
                {
                    // if last attribution send date before 20 days and now it passed 
                    // 20 send last one more time
                    string lastAttributionDateStr = storage.LoadData(LastSendAttributionDateKey);
                    DateTime lastAttributionDate = MetriqusUtils.ParseDate(lastAttributionDateStr);

                    if (lastAttributionDate.Subtract(installDate).Days < remoteSettings.AttributionCheckWindow
                        && currentDate.Subtract(installDate).Days > remoteSettings.AttributionCheckWindow)
                    {
                        sendAttr();
                    }
                }
            });
        }

        private void ProcessIsFirstLaunch()
        {
            bool isIsFirstLaunchKeyExist = storage.CheckKeyExist(FirstLaunchKey);

            if (!isIsFirstLaunchKeyExist)
            {
                isFirstLaunch = true;

                storage.SaveData(FirstLaunchKey, MetriqusJSON.SerializeValue(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));

                onFirstLaunch?.Invoke();
            }
        }

        internal int GetFirstLaunchTime()
        {
            bool isIsFirstLaunchKeyExist = storage.CheckKeyExist(FirstLaunchKey);

            if (isIsFirstLaunchKeyExist)
            {
                return storage.LoadIntData(FirstLaunchKey);
            }

            return 0;
        }

        internal MetriqusSettings GetMetriqusSettings()
        {
            return metriqusSettings;
        }

        internal MetriqusRemoteSettings GetMetriqusRemoteSettings()
        {
            return remoteSettings;
        }

        private async Task<IPGeolocation.Geolocation> FetchGeolocation()
        {
            IPGeolocation.Geolocation info = null;

            DateTime geolocationLastFetchedTime = DateTime.MinValue;

            // Check if geolocation fetched before
            bool GeolocationLastFetchedTimeKeyExist = storage.CheckKeyExist(GeolocationLastFetchedTimeKey);
            if (GeolocationLastFetchedTimeKeyExist)
            {
                // load and parse last fetched date
                string geolocationLastFetchedTimeStr = storage.LoadData(GeolocationLastFetchedTimeKey);
                geolocationLastFetchedTime = MetriqusUtils.ParseDate(geolocationLastFetchedTimeStr);
            }

            var remoteSettings = GetMetriqusRemoteSettings();

            bool fetchingSuccessful = true;

            if (DateTime.UtcNow.Subtract(geolocationLastFetchedTime).Days > remoteSettings.GeolocationFetchIntervalDays)
            {
                // it passed {remoteSettings.GeolocationFetchIntervalDays} since last fetched geolocation
                if (Metriqus.LogLevel == LogLevel.Verbose)
                    Metriqus.DebugLog($"Fetching geolocating. Last Fetched at: {geolocationLastFetchedTime.ToString()}");

                var fetchedGeolocation = await IPGeolocation.GetCountryByIP();

                fetchingSuccessful = fetchedGeolocation != null;
                
                info = fetchedGeolocation;
            }

            if (info != null)
            {
                this.geolocationFetched = true;
                this.geolocation = info;

                if (Metriqus.LogLevel == LogLevel.Verbose)
                    Metriqus.DebugLog("Geolocation fetched: " + JsonUtility.ToJson(info));

                storage.SaveData(GeolocationKey, JsonUtility.ToJson(info));

                storage.SaveData(GeolocationLastFetchedTimeKey, MetriqusUtils.ConvertDateToString(DateTime.Now));

                return info;
            }
            else
            {
                if (fetchingSuccessful)
                    this.geolocationFetched = true;

                bool isGeolocationKeyExist = storage.CheckKeyExist(GeolocationKey);
                if (isGeolocationKeyExist)
                {
                    string geolocationJson = storage.LoadData(GeolocationKey);
                    this.geolocation = JsonUtility.FromJson<IPGeolocation.Geolocation>(geolocationJson);

                    if (Metriqus.LogLevel == LogLevel.Verbose)
                        Metriqus.DebugLog("Geolocation loaded from storage: " + geolocationJson);

                    return this.geolocation;
                }
            }

            geolocationFetched = false;
            return null;
        }

        public IPGeolocation.Geolocation GetGeolocation() => geolocation;

        private class RemoteSettingRequestParams
        {
            public int Platform;
            public string ClientKey;
            public string PackageName;
        }
        private async Task<bool> FetchRemoteSettings()
        {
            var headers = new Dictionary<string, string>();
            RequestSender.AddContentType(headers, RequestSender.ContentTypeJson);
            RequestSender.AddAccept(headers, RequestSender.ContentTypeJson);

            var response = await RequestSender.PostAsync("https://rmt.metriqus.com/event/remote-settings",
                JsonUtility.ToJson(new RemoteSettingRequestParams()
                {
                    Platform = deviceInfo.platform,
                    ClientKey = metriqusSettings.ClientKey,
                    PackageName = deviceInfo.packageName
                }), headers);

            var mro = MetriqusResponseObject.Parse(response.Data);

            if (response.IsSuccess && mro != null)
            {
                remoteSettings = MetriqusRemoteSettings.Parse(mro.Data);

                storage.SaveData(RemoteSettingsKey, mro.Data);
                remoteSettingsFetched = true;

                /*if (Metriqus.LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog($"Remote Settings Fetched: {mro.Data}");*/

                return true;
            }
            else if (remoteSettings == null)
            {
                bool isKeyExist = storage.CheckKeyExist(RemoteSettingsKey);

                if (isKeyExist)
                {
                    string data = storage.LoadData(RemoteSettingsKey);
                    /*if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"Remote Settings loaded from storage: {data}");*/
                    remoteSettings = MetriqusRemoteSettings.Parse(data);
                }
                else
                {
                    remoteSettings = new MetriqusRemoteSettings();
                    Metriqus.DebugLog($"Remote Settings couldn't fetched or couldn't loaded from storage: {JsonUtility.ToJson(remoteSettings)}", LogType.Warning);
                }
            }

            remoteSettingsFetched = false;
            return false;
        }
    }
}