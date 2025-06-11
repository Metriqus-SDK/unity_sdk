using System;
using System.Globalization;
using UnityEngine;
#if PLATFORM_IOS
using UnityEngine.iOS;
#endif

namespace MetriqusSdk
{
    public class DeviceInfo
    {
        public string packageName;
        public string appVersion;
        public string unityVersion;
        public string deviceType;
        public string deviceName;
        public string deviceModel;
        public int platform;
        public string graphicsDeviceName;
        public string osName;
        public int systemMemorySize;
        public int graphicsMemorySize;
        public string language;
        public string country;
        public float screenDpi;
        public int screenWidth;
        public int screenHeight;
        public string deviceId;
        public string vendorId = null;

        public DeviceInfo()
        {
            packageName = Application.identifier;
            appVersion = Application.version;
            unityVersion = $"unity-{Application.unityVersion}";
            deviceType = GetDeviceType();
            deviceName = SystemInfo.deviceName;
            deviceModel = SystemInfo.deviceModel;
            platform = Application.platform == RuntimePlatform.Android ? 1 : Application.platform == RuntimePlatform.IPhonePlayer ? 0 : -1;
            graphicsDeviceName = SystemInfo.graphicsDeviceName;
            osName = SystemInfo.operatingSystem;
            systemMemorySize = SystemInfo.systemMemorySize;
            graphicsMemorySize = SystemInfo.graphicsMemorySize;
            language = Application.systemLanguage.ToString();
            country = GetCountryCode();
            screenDpi = Screen.dpi;
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            deviceId = SystemInfo.deviceUniqueIdentifier;
#if PLATFORM_IOS
            vendorId = Device.vendorIdentifier;
#endif
        }

        private float DeviceDiagonalSizeInInches()
        {
            float screenWidth = Screen.width / Screen.dpi;
            float screenHeight = Screen.height / Screen.dpi;
            float diagonalInches = Mathf.Sqrt(Mathf.Pow(screenWidth, 2) + Mathf.Pow(screenHeight, 2));

            return diagonalInches;
        }

        public string GetDeviceType()
        {
            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
#if UNITY_IOS
                bool deviceIsIpad = UnityEngine.iOS.Device.generation.ToString().Contains("iPad");
                if (deviceIsIpad)
                {
                    return "tablet";
                }

                bool deviceIsIphone = UnityEngine.iOS.Device.generation.ToString().Contains("iPhone");
                if (deviceIsIphone)
                {
                    return "phone";
                }
#elif UNITY_ANDROID

                float aspectRatio = Mathf.Max(Screen.width, Screen.height) / Mathf.Min(Screen.width, Screen.height);
                bool isTablet = (DeviceDiagonalSizeInInches() > 6.5f && aspectRatio < 2f);

                if (isTablet)
                {
                    return "tablet";
                }
                else
                {
                    return "phone";
                }
#else
                return "desktop";
#endif
            }
            else if (SystemInfo.deviceType == DeviceType.Console)
            {
                return "console";
            }
            else if (SystemInfo.deviceType == DeviceType.Desktop)
            {
                return "desktop";
            }

            return "desktop";
        }

        private string GetCountryCode()
        {
            // Get the current culture info
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Use RegionInfo to get the country code
            RegionInfo region = new RegionInfo(currentCulture.Name);

            // Return the two-letter ISO country code
            return region.TwoLetterISORegionName; // Example: "US", "GB", "FR"
        }
    }
}