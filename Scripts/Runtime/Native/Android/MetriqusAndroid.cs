using System;
using UnityEngine;

namespace MetriqusSdk.Android
{
#if UNITY_ANDROID
    internal sealed class MetriqusAndroid : MetriqusNative
    {
        private static AndroidJavaObject ajoCurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        private static AndroidJavaClass ajcMetriqus = new AndroidJavaClass("com.metriqus.metriquslibrary.Metriqus");

        internal override void InitSdk(MetriqusSettings metriqusSettings)
        {
            this.metriqusSettings = metriqusSettings;
            ReadAdid((adId) =>
            {
                // if ad id is not null or empty App Tracking is allowed
                isTrackingEnabled = string.IsNullOrEmpty(adId) == false;

                this.adId = adId;

                if (Metriqus.LogLevel != LogLevel.NoLog)
                    Metriqus.DebugLog("ad Id : " + adId);

                base.InitSdk(metriqusSettings);
            });
        }

        internal override void ReadAdid(Action<string> callback)
        {
            try
            {
                if (this.adId != null)
                {
                    callback(this.adId);
                }

                AdidReadListener onAdidReadProxy = new AdidReadListener(callback);

                ajcMetriqus.CallStatic("getAdId", ajoCurrentActivity, onAdidReadProxy);
            }
            catch (Exception e)
            {
                Debug.LogError("Error fetching Advertising ID: " + e.ToString());
            }
        }

        internal override void ReadAttribution(Action<MetriqusAttribution> onReadCallback, Action<string> onError)
        {
            try
            {
                AttributionReadListener onAttributionReadProxy = new AttributionReadListener(onReadCallback, onError);

                ajcMetriqus.CallStatic("readAttribution", ajoCurrentActivity, onAttributionReadProxy);
            }
            catch (Exception e)
            {
                Debug.LogError("Error Reading Attribution: " + e.ToString());
            }
        }

        internal override void GetInstallTime(Action<long> callback)
        {
            OnInstallTimeReadListener onInstallTimeReadProxy = new OnInstallTimeReadListener(callback);
            ajcMetriqus.CallStatic("getInstallTime", ajoCurrentActivity, onInstallTimeReadProxy);
        }

        private class OnInstallTimeReadListener : AndroidJavaProxy
        {
            private Action<long> callback;

            public OnInstallTimeReadListener(Action<long> pCallback) : base("com.metriqus.metriquslibrary.OnInstallTimeReadListener")
            {
                this.callback = pCallback;
            }

            // native method:
            // void onInstallTimeRead(long adidinstallTime
            public void onInstallTimeRead(long installTime)
            {
                if (this.callback == null)
                {
                    return;
                }

                Metriqus.EnqueueCallback(() => { this.callback(installTime); });
            }
        }

        private class AdidReadListener : AndroidJavaProxy
        {
            private Action<string> callback;

            public AdidReadListener(Action<string> pCallback) : base("com.metriqus.metriquslibrary.OnAdidReadListener")
            {
                this.callback = pCallback;
            }

            // native method:
            // void onAdidRead(String adid);
            public void onAdidRead(string adid)
            {
                if (this.callback == null)
                {
                    return;
                }
                Metriqus.EnqueueCallback(() => { this.callback(adid); });
            }
        }

        private class AttributionReadListener : AndroidJavaProxy
        {
            private Action<MetriqusAttribution> successCallback;
            private Action<string> errorCallback;

            public AttributionReadListener(Action<MetriqusAttribution> _successCallback, Action<string> _errorCallback)
                : base("com.metriqus.metriquslibrary.OnAttributionReadListener")
            {
                this.successCallback = _successCallback;
                this.errorCallback = _errorCallback;
            }

            // native method:
            // void onAttributionRead(string referrerUrl);
            public void onAttributionRead(string referrerUrl)
            {
                if (this.successCallback == null)
                {
                    return;
                }

                Metriqus.EnqueueCallback(() =>
                {
                    MetriqusAttribution attribution = new MetriqusAttribution(referrerUrl);

                    this.successCallback(attribution);
                });
            }

            public void onError(string error)
            {
                if (this.successCallback == null)
                {
                    return;
                }

                Metriqus.EnqueueCallback(() => { this.errorCallback(error); });
            }
        }
    }
#endif
}
