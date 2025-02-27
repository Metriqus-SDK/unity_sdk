using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace MetriqusSdk
{
    /// <summary>
    /// ScriptableObject containing settings for Metriqus integration.
    /// </summary>
    [CreateAssetMenu(fileName = "MetriqusSettings", menuName = "Metriqus/MetriqusSettings")]
    public class MetriqusSettings : ScriptableObject
    {
        private static MetriqusSettings instance;
        public const string MetriqusSettingsExportPath = "Metriqus/Settings/MetriqusSettings.asset";

        [Header("Application Settings")]

        /// <summary>
        /// The environment configuration for Metriqus.
        /// </summary>
        [Tooltip("Specifies the environment configuration for Metriqus.")]
        [SerializeField]
        public MetriqusEnvironment Environment;

        /// <summary>
        /// If true, the Metriqus service will start manually instead of automatically.
        /// </summary>
        [Tooltip("If enabled, Metriqus must be started manually.")]
        public bool ManuelStart = false;

        /// <summary>
        /// The logging level for Metriqus.
        /// </summary>
        [Tooltip("Defines the logging level for Metriqus.")]
        public LogLevel LogLevel;

        [Header("Metriqus Information")]

        /// <summary>
        /// The client key for authentication.
        /// </summary>
        [Tooltip("The client key used for authentication with Metriqus.")]
        public string ClientKey;

        /// <summary>
        /// The client secret for authentication.
        /// </summary>
        [Tooltip("The client secret used for authentication with Metriqus.")]
        public string ClientSecret;

        [Header("iOS Privacy")]

        /// <summary>
        /// Enables or disables user tracking on iOS. 
        /// This setting controls whether the app requests permission to track the user across apps and websites.
        /// </summary>
        [Tooltip("Enable or disable user tracking on iOS. This determines if the app requests tracking permission.")]
        public bool iOSUserTrackingDisabled;

        /// <summary>
        /// The description displayed to users when requesting tracking permission on iOS.
        /// This text should clearly explain why tracking is needed and how it benefits the user.
        /// </summary>
        [Tooltip("The message shown to users when requesting iOS tracking permission.")]
        public string iOSUserTrackingUsageDescription;


        /*[Header("Link iOS Frameworks")]

        /// <summary>
        /// Links the AdSupport framework on iOS, allowing access to the Identifier for Advertisers (IDFA).
        /// Required for ad tracking and attribution.
        /// </summary>
        [Tooltip("Links the AdSupport framework to access IDFA for ad tracking and attribution.")]
        public bool iOSFrameworkAdSupport = true;

        /// <summary>
        /// Links the AdServices framework on iOS, used for Apple Search Ads attribution.
        /// </summary>
        [Tooltip("Links the AdServices framework for Apple Search Ads attribution.")]
        public bool iOSFrameworkAdServices = true;

        /// <summary>
        /// Links the AppTrackingTransparency framework on iOS, required for requesting user tracking permission.
        /// </summary>
        [Tooltip("Links the AppTrackingTransparency framework to request tracking permission from users.")]
        public bool iOSFrameworkAppTrackingTransparency = true;

        /// <summary>
        /// Links the StoreKit framework on iOS, used for handling in-app purchases and subscriptions.
        /// </summary>
        [Tooltip("Links the StoreKit framework for managing in-app purchases and subscriptions.")]
        public bool iOSFrameworkStoreKit = true;*/


        /*[Header("Android Permissions")]

        /// <summary>
        /// Requests the INTERNET permission on Android, required for network communications.
        /// </summary>
        [Tooltip("Grants the app access to the internet for network communications.")]
        public bool androidPermissionInternet;

        /// <summary>
        /// Requests the INSTALL_REFERRER service permission on Android, used for tracking app installs.
        /// </summary>
        [Tooltip("Allows tracking app install sources using the Google Play Install Referrer API.")]
        public bool androidPermissionInstallReferrerService;

        /// <summary>
        /// Requests access to the Android Advertising ID (AdID), used for ad personalization and analytics.
        /// </summary>
        [Tooltip("Grants access to the Android Advertising ID (AdID) for ad tracking and analytics.")]
        public bool androidPermissionAdId;

        /// <summary>
        /// Requests permission to access network state on Android, used to check connectivity status.
        /// </summary>
        [Tooltip("Allows the app to check network connectivity status.")]
        public bool androidPermissionAccessNetworkState;*/

#if UNITY_EDITOR
        public static MetriqusSettings Instance
        {
            get
            {
                instance = NullableInstance;

                if (instance == null)
                {
                    // Create MetriqusSettings.asset inside the folder in which MetriqusSettings.cs reside.
                    instance = ScriptableObject.CreateInstance<MetriqusSettings>();
                    var guids = AssetDatabase.FindAssets(string.Format("{0} t:script", "MetriqusSettings"));
                    if (guids == null || guids.Length <= 0)
                    {
                        return instance;
                    }

                    var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]).Replace("MetriqusSettings.cs", "MetriqusSettings.asset");
                    // MetriqusSettings.asset will be stored inside of the Metriqus/Settings directory
                    if (assetPath.StartsWith("Assets"))
                    {
                        // plugin located in Assets directory
                        string rootDir = assetPath.Replace("/Metriqus/Scripts/Editor/MetriqusSettings.asset", "");
                        string metriqusSettingsPath = Path.Combine(rootDir, "Metriqus/Settings");
                        if (!Directory.Exists(metriqusSettingsPath))
                        {
                            Directory.CreateDirectory(metriqusSettingsPath);
                        }
                        assetPath = Path.Combine(rootDir, MetriqusSettingsExportPath);
                    }
                    else
                    {
                        // plugin located in Packages folder
                        string metriqusSettingsPath = Path.Combine("Assets", "Metriqus/Settings");
                        if (!Directory.Exists(metriqusSettingsPath))
                        {
                            Directory.CreateDirectory(metriqusSettingsPath);
                        }
                        assetPath = Path.Combine("Assets", MetriqusSettingsExportPath);
                    }

                    AssetDatabase.CreateAsset(instance, assetPath);
                }

                return instance;
            }
        }

        public static MetriqusSettings NullableInstance
        {
            get
            {
                if (instance == null)
                {
                    var guids = AssetDatabase.FindAssets(string.Format("{0} t:ScriptableObject", "MetriqusSettings"));
                    if (guids == null || guids.Length <= 0)
                    {
                        return instance;
                    }
                    var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    instance = (MetriqusSettings)AssetDatabase.LoadAssetAtPath(assetPath, typeof(MetriqusSettings));
                }

                return instance;
            }
        }
#endif
    }

    /// <summary>
    /// Defines the logging levels for Metriqus.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// No logging.
        /// </summary>
        NoLog,

        /// <summary>
        /// Logs all details.
        /// </summary>
        Verbose,

        /// <summary>
        /// Logs errors only.
        /// </summary>
        ErrorsOnly
    }
}