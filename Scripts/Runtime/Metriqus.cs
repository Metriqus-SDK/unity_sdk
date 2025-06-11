using MetriqusSdk.Android;
using MetriqusSdk.iOS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MetriqusSdk
{
    public class Metriqus : MonoBehaviour
    {
        [SerializeField] private MetriqusSettings metriqusSettings;

        private const string errorMsgEditor = "SDK can not be used in Editor.";
        private const string errorMsgStart = "SDK not initialized. Initialize it manually using the 'InitSdk' method.";
        private const string errorMsgPlatform = "SDK can only be used in Android and iOS apps.";

        private const string sdkPrefix = "unity-1.0.11";

        private static MetriqusNative native = null;

        private static LogLevel logLevel = LogLevel.Verbose;

        private static UnityEvent<string, LogType> onLog = new();
        private static UnityEvent<bool> onSdkInitialize = new();

        private static readonly Queue<Action> _executionQueue = new Queue<Action>();

        public static UnityEvent<string, LogType> OnLog => onLog;
        public static UnityEvent<bool> OnSdkInitialize => onSdkInitialize;
        public static LogLevel LogLevel => logLevel;

        void Awake()
        {
            StartCoroutine(SavePassedTimeEveryMinute());

            if (IsEditor())
            {
                return;
            }

            DontDestroyOnLoad(gameObject);

            // initialize sdk
            if (!metriqusSettings.ManuelStart)
                InitSdk(metriqusSettings);
        }

        private void Update()
        {
            lock (_executionQueue)
            {
                while (_executionQueue.Count > 0)
                {
                    _executionQueue.Dequeue()?.Invoke();
                }
            }
        }

        internal static void EnqueueCallback(Action action)
        {
            lock (_executionQueue)
            {
                _executionQueue.Enqueue(action);
            }
        }

        public static void InitSdk(MetriqusSettings metriqusSettings)
        {
            if (IsEditor())
            {
                return;
            }

            if (metriqusSettings == null)
            {
                DebugLog("InitSdk missing settings to start.", LogType.Error);
                return;
            }

            logLevel = metriqusSettings.LogLevel;

#if UNITY_IOS
            native = new MetriqusIOS();
            native.InitSdk(metriqusSettings);
#elif UNITY_ANDROID
            native = new MetriqusAndroid();
            native.InitSdk(metriqusSettings);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        private void OnApplicationQuit()
        {
            if (!IsInitialized()) return;

            native.OnQuit();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (!IsInitialized()) return;

            if (isPaused)
            {
                native.OnPause();
            }
            else
            {
                native.OnResume();
            }
        }

        /// <summary>
        /// Track IAP EVENT
        /// </summary>
        /// <param name="metriqusEvent"></param>
        public static void TrackIAPEvent(MetriqusInAppRevenue metriqusEvent)
        {
            if (metriqusEvent == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing event to track.", LogType.Error);
                return;
            }
#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.TrackIAPEvent(metriqusEvent);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        /// <summary>
        /// Track Custom Event. Set key and parameters as you wish.
        /// </summary>
        /// <param name="_event"></param>
        public static void TrackCustomEvent(MetriqusCustomEvent _event)
        {
            if (_event == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing custom event to track.", LogType.Error);
                return;
            }
#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.TrackCustomEvent(_event);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        /// <summary>
        /// Track Fps and other performance metrics.
        /// </summary>
        /// <param name="_event"></param>
        public static void TrackPerformance(int fps, List<TypedParameter> parameters = null)
        {
            var paramList = parameters ?? new();
            paramList.Add(new TypedParameter(MetriqusEventKeys.ParameterFps, fps));

            TrackCustomEvent(new MetriqusCustomEvent(MetriqusEventKeys.EventPerformance, paramList));
        }

        /// <summary>
        /// Item used event is for tracking any item such as currency, equipment or any spendable unit.
        /// </summary>
        /// <param name="_event"></param>
        public static void TrackItemUsed(MetriqusItemUsedEvent _event)
        {
            TrackCustomEvent(_event);
        }

        /// <summary>
        /// Track Level Started Event. Call When a Level started.
        /// </summary>
        /// <param name="_event"></param>
        public static void TrackLevelStarted(MetriqusLevelStartedEvent _event)
        {
            TrackCustomEvent(_event);
        }

        /// <summary>
        /// Track Level Completed Event. Call When A Level completed.
        /// </summary>
        /// <param name="_event"></param>
        public static void TrackLevelCompleted(MetriqusLevelCompletedEvent _event)
        {
            TrackCustomEvent(_event);
        }

        /// <summary>
        /// Track Campaign Actions such as Showed, Clicked, Closed, Purchased
        /// </summary>
        /// <param name="_event"></param>
        public static void TrackCampaignAction(MetriqusCampaignActionEvent _event)
        {
            TrackCustomEvent(_event);
        }

        /// <summary>
        /// Tracks Screen View Event. Use it when to track screen view analytics.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="parameters"></param>
        public static void TrackScreenView(string screenName, List<TypedParameter> parameters = null)
        {
            var paramList = parameters ?? new();
            paramList.Add(new TypedParameter(MetriqusEventKeys.ParameterScreenName, screenName));

            TrackCustomEvent(new MetriqusCustomEvent(MetriqusEventKeys.EventScreenView, paramList));
        }

        /// <summary>
        /// Tracks Button Click Event. Use it when to track click analytics of a button.
        /// </summary>
        /// <param name="buttonName"></param>
        /// <param name="parameters"></param>
        public static void TrackButtonClick(string buttonName, List<TypedParameter> parameters = null)
        {
            var paramList = parameters ?? new();
            paramList.Add(new TypedParameter(MetriqusEventKeys.ParameterButtonName, buttonName));

            TrackCustomEvent(new MetriqusCustomEvent(MetriqusEventKeys.EventButtonClick, paramList));
        }

        /// <summary>
        /// Track Applovin Ad Revenue
        /// </summary>
        /// <param name="matriqusAdRevenue"></param>
        public static void TrackApplovinAdRevenue(MetriqusApplovinAdRevenue matriqusAdRevenue)
        {
            if (matriqusAdRevenue == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing applovin ad revenue to track.", LogType.Error);
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.TrackAdRevenue(matriqusAdRevenue);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        /// <summary>
        /// Track Admob Ad Revenue
        /// </summary>
        /// <param name="matriqusAdRevenue"></param>
        public static void TrackAdmobAdRevenue(MetriqusAdmobAdRevenue matriqusAdRevenue)
        {
            if (matriqusAdRevenue == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing admob ad revenue to track.", LogType.Error);
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.TrackAdRevenue(matriqusAdRevenue);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        /// <summary>
        /// Track Ad Revenue
        /// </summary>
        /// <param name="matriqusAdRevenue"></param>
        public static void TrackAdRevenue(MetriqusAdRevenue matriqusAdRevenue)
        {

            if (matriqusAdRevenue == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing ad revenue to track.", LogType.Error);
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.TrackAdRevenue(matriqusAdRevenue);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

#if UNITY_IOS
        public static void UpdateiOSConversionValue(int conversionValue)
        {
            if(IsInitialized())
                native.UpdateIOSConversionValue(conversionValue);
        }
#endif


        /// <summary>
        /// Sets a user attribute spesific to user.
        /// </summary>
        /// <param name="parameter"></param>
        public static void SetUserAttribute(TypedParameter parameter)
        {
            if (parameter == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing parameter to set user attributes", LogType.Error);
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.SetUserAttribute(parameter);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        public static List<TypedParameter> GetUserAttributes()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                return native.UserAttributes.Parameters;
#else
            DebugLog(errorMsgPlatform);
#endif

            return null;
        }


        public static void RemoveUserAttribute(string key)
        {
            if (key == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog("Missing key to remove from user attributes");
                return;
            }

#if UNITY_ANDROID || UNITY_IOS
            if (IsInitialized())
                native.RemoveUserAttribute(key);
#else
            DebugLog(errorMsgPlatform);
#endif
        }

        public static string GetAdid()
        {
            if (IsEditor())
            {
                return "";
            }

#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return "";

            return native.GetAdid();
#else
            DebugLog(errorMsgPlatform);
            return "";
#endif
        }

        internal static string GetClientSdk()
        {
#if UNITY_ANDROID || UNITY_IOS
            return sdkPrefix;
#else
            DebugLog(errorMsgPlatform);
            return "editor";
#endif
        }

        public static MetriqusSettings GetMetriqusSettings()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return null;

            return native.GetMetriqusSettings();
#else
            DebugLog(errorMsgPlatform);
            return null;
#endif
        }

        internal static MetriqusRemoteSettings GetMetriqusRemoteSettings()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return null;

            return native.GetMetriqusRemoteSettings();
#else
            DebugLog(errorMsgPlatform);
            return null;
#endif
        }

        public static IPGeolocation.Geolocation GetGeolocation()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return null;

            return native.GetGeolocation();
#else
            DebugLog(errorMsgPlatform);
            return null;
#endif
        }

        public static DeviceInfo GetDeviceInfo()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return null;

            return native.DeviceInfo;
#else
            DebugLog(errorMsgPlatform);
            return null;
#endif
        }

        public static string GetUniqueUserId()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return null;

            return native.UniqueUserIdentifier.Id;
#else
            DebugLog(errorMsgPlatform);
            return null;
#endif
        }

        internal static bool GetIsFirstLaunch()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return false;

            return native.IsFirstLaunch;
#else
            DebugLog(errorMsgPlatform);
            return false;
#endif
        }

        internal static DateTime GetUserFirstTouchTimestamp()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return MetriqusUtils.GetUtcStartTime();

            return native.GetFirstLaunchTime();
#else
            DebugLog(errorMsgPlatform);
            return DateTime.UtcNow;
#endif
        }

        internal static string GetSessionId()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (!IsInitialized()) return "";

            return native.SessionId;
#else
            DebugLog(errorMsgPlatform);
            return "";
#endif
        }

        private static bool IsEditor()
        {
#if UNITY_EDITOR
            if (LogLevel != LogLevel.NoLog)
                DebugLog(errorMsgEditor);
            return true;
#else
            return false;
#endif
        }

        public static bool IsInitialized()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (native == null || !native.IsInitialized)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog(errorMsgStart);
                return false;
            }

            return native.IsInitialized;
#else
            DebugLog(errorMsgPlatform);
            return false;
#endif
        }

        public static bool IsTrackingEnabled()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (native == null || !native.IsInitialized)
            {
                if (LogLevel != LogLevel.NoLog)
                    DebugLog(errorMsgStart);
                return false;
            }

            return native.IsTrackingEnabled;
#else
            DebugLog(errorMsgPlatform);
            return false;
#endif
        }

        private IEnumerator SavePassedTimeEveryMinute()
        {
            while (true)
            {
                // Wait for 60 seconds
                yield return new WaitForSeconds(60);

                // Update the passed time
                float timePassed = Time.time;

#if UNITY_ANDROID || UNITY_IOS
                if (IsInitialized())
                {
                    native.SendSessionBeatEvent();
                    _ = native.InternetConnectionChecker.CheckInternetConnection();
                }
#endif

                //DebugLog($"Time saved: {timePassed} seconds since app start");
            }
        }

        public static void DebugLog(string log, LogType logType = LogType.Log)
        {
            if (log == null)
            {
                if (LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog("Given log is null");
                return;
            }

            try
            {
                if (logType == LogType.Error)
                {
                    EnqueueCallback(() =>
                    {
                        log = $"[MetriqusError]: {log}\n";
                        Debug.LogError(log);

                        onLog?.Invoke(log, logType);
                    });
                }
                else if (logLevel == LogLevel.Verbose)
                {
                    if (logType == LogType.Warning)
                    {
                        EnqueueCallback(() =>
                        {
                            log = $"[MetriqusWarning]: {log}\n";
                            Debug.LogWarning(log);

                            onLog?.Invoke(log, logType);
                        });
                    }
                    else
                    {
                        EnqueueCallback(() =>
                        {
                            log = $"\n[Metriqus]: {log}\n";
                            Debug.Log(log);

                            onLog?.Invoke(log, logType);
                        });
                    }
                }
            }
            catch (System.Exception)
            { Debug.LogError("[MetriqusError]: Error occured while logging\n"); }
        }
    }
}