using AOT;
using System;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MetriqusSdk.iOS
{
#if UNITY_IOS

    internal sealed class MetriqusIOS : MetriqusNative
    {
        private const string installTimeKey = "MetriqusInstallTime";

        [DllImport("__Internal")]
        private static extern void metriqusRequestTrackingPermission(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void metriqusReportAdNetworkAttribution(Action<string> callback);

        [DllImport("__Internal")]
        private static extern void metriqusUpdateConversionValue(int value, Action<string> callback);

        [DllImport("__Internal")]
        private static extern void metriqusReadAttributionToken(Action<string> callback, Action<string> errorCallback);

        internal override void InitSdk(MetriqusSettings metriqusSettings)
        {
            this.metriqusSettings = metriqusSettings;

            try
            {
                ReadAdid((adId) =>
                {
                    // if ad id is not null or empty App Tracking is allowed
                    isTrackingEnabled = string.IsNullOrEmpty(adId) == false;

                    if (adId == null)
                    {
                        if (Metriqus.LogLevel != LogLevel.NoLog)
                            Metriqus.DebugLog("Ad id is null");
                    }

                    this.adId = adId;
                    Metriqus.DebugLog("ad Id : " + adId);
                    Metriqus.EnqueueCallback(() =>
                    {
                        try
                        {
                            base.InitSdk(metriqusSettings);
                        }
                        catch (System.Exception e)
                        {
                            if (Metriqus.LogLevel != LogLevel.NoLog)
                                Metriqus.DebugLog("Error while initializing base class: " + e);
                        }
                    });

                });
            }
            catch (Exception e)
            {
                Metriqus.DebugLog(e.ToString(), LogType.Error);
            }
        }

        protected override void OnFirstLaunch()
        {
            ReportAdNetworkAttribution();
            SetInstallTime();
        }

        internal override void ReadAdid(Action<string> callback)
        {
            AdIdCallback = callback;

            // if tracking disabled directly return empty string
            if (this.GetMetriqusSettings().iOSUserTrackingDisabled)
            {
                isTrackingEnabled = false;
                callback("");
            }
            else
            {
                metriqusRequestTrackingPermission(StaticAdIdCallback);
            }
        }

        private static Action<string> AdIdCallback;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void StaticAdIdCallback(string adId)
        {
            AdIdCallback?.Invoke(adId);
        }

        internal override void GetInstallTime(Action<long> callback)
        {
            long _iTime = storage.LoadLongData(installTimeKey);

            if (Metriqus.LogLevel != LogLevel.NoLog)
                Metriqus.DebugLog("GetInstallTime: " + _iTime);

            callback(_iTime);
        }

        private void SetInstallTime()
        {
            storage.SaveData(installTimeKey, MetriqusJSON.SerializeValue(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
        }

        private void ReportAdNetworkAttribution()
        {
            Metriqus.DebugLog("ReportAdNetworkAttribution");

            ReportAdNetworkAttributionCallback = (message) =>
            {
                Metriqus.DebugLog(message);
            };

            metriqusReportAdNetworkAttribution(StaticReportAdNetworkAttributionCallback);
        }

        private static Action<string> ReportAdNetworkAttributionCallback;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void StaticReportAdNetworkAttributionCallback(string message)
        {
            ReportAdNetworkAttributionCallback?.Invoke(message);
        }

        internal override void UpdateIOSConversionValue(int value)
        {
            UpdateConversionValueCallback = (message) =>
            {
                Metriqus.DebugLog(message);
            };

            metriqusUpdateConversionValue(value, StaticUpdateConversionValueCallback);
        }

        private static Action<string> UpdateConversionValueCallback;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void StaticUpdateConversionValueCallback(string message)
        {
            UpdateConversionValueCallback?.Invoke(message);
        }

        internal override void ReadAttribution(Action<MetriqusAttribution> onReadCallback, Action<string> onError)
        {
            if (Metriqus.LogLevel != LogLevel.NoLog)
                Metriqus.DebugLog("ReadAttribution iOS");

            ReadAttributionCallback = async (attToken) =>
            {
                if (attToken != null)
                {
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog("Attribution Token: " + attToken);

                    // TODO: ask server for attribution data
                    // TODO: return attribution with callback
                    var mAttribution = await RequestAttributionDataAsync(attToken);
                    onReadCallback(mAttribution);
                }
                else if (Metriqus.LogLevel != LogLevel.NoLog)
                {
                    Metriqus.DebugLog("Attribution Token is null");
                }
            };
            ReadAttributionFailureCallback = (message) =>
            {
                Metriqus.DebugLog(message, LogType.Error);
            };

            metriqusReadAttributionToken(StaticReadAttributionCallback, StaticReadAttributionFailureCallback);
        }

        private static Action<string> ReadAttributionCallback;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void StaticReadAttributionCallback(string token)
        {
            ReadAttributionCallback?.Invoke(token);
        }

        private static Action<string> ReadAttributionFailureCallback;

        [MonoPInvokeCallback(typeof(Action<string>))]
        private static void StaticReadAttributionFailureCallback(string error)
        {
            ReadAttributionFailureCallback?.Invoke(error);
        }

        public async Task<MetriqusAttribution> RequestAttributionDataAsync(string token)
        {
            using (HttpClient client = new HttpClient())
            {
                int attempts = 0;
                bool requestSuccessful = false;

                while (attempts < 3 && !requestSuccessful)
                {
                    attempts++;

                    try
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api-adservices.apple.com/api/v1/")
                        {
                            Content = new StringContent(token, Encoding.UTF8, "text/plain")
                        };

                        HttpResponseMessage response = await client.SendAsync(request);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();

                            if (Metriqus.LogLevel != LogLevel.NoLog)
                                Metriqus.DebugLog("Attribution Response: " + responseBody);

                            requestSuccessful = true;

                            return MetriqusAttribution.Parse(responseBody);
                        }
                        else if ((int)response.StatusCode == 404)
                        {
                            if (Metriqus.LogLevel != LogLevel.NoLog)
                                Metriqus.DebugLog("404 Not Found. Retrying...");

                            await Task.Delay(5000); // 5 sec between every try
                        }
                        else if ((int)response.StatusCode == 400)
                        {
                            if (Metriqus.LogLevel != LogLevel.NoLog)
                                Metriqus.DebugLog("Attribution Status code 400. The token is invalid.", LogType.Warning);
                        }
                        else if ((int)response.StatusCode == 500)
                        {
                            if (Metriqus.LogLevel != LogLevel.NoLog)
                                Metriqus.DebugLog("Attribution Status code  500. Apple Search Ads server is temporarily down or unreachable..", LogType.Warning);
                        }
                        else
                        {
                            if (Metriqus.LogLevel != LogLevel.NoLog)
                                Metriqus.DebugLog($"Error: {response.StatusCode} - {response.ReasonPhrase}", LogType.Warning);

                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (Metriqus.LogLevel != LogLevel.NoLog)
                            Metriqus.DebugLog("Exception: " + ex.Message, LogType.Error);

                        break;
                    }
                }

                if (!requestSuccessful)
                {
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog("Attribution Request failed to get a successful response after multiple attempts.", LogType.Error);
                }

                return null;
            }
        }

    }
#endif
}