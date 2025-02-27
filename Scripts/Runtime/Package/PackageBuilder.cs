using System;
using System.Collections.Generic;

namespace MetriqusSdk
{
    internal class PackageBuilder
    {
        private DeviceInfo deviceInfo;
        private DateTime createdAt;
        private MetriqusSettings metriqusSettings;

        public PackageBuilder(MetriqusSettings metriqusSettings, DeviceInfo deviceInfo, DateTime createdAt)
        {
            this.metriqusSettings = metriqusSettings;
            this.deviceInfo = deviceInfo;
            this.createdAt = createdAt;
        }

        public Package BuildSessionStartPackage()
        {
            var package = GetDefaultPackage();

            AddDefaultParameters(package);

            package.SetKey("session_start");

            return package;
        }

        public Package BuildSessionBeatPackage()
        {
            var package = GetDefaultPackage();

            AddDefaultParameters(package);

            package.SetKey("session_beat");

            return package;
        }

        public Package BuildIAPEventPackage(MetriqusInAppRevenue metriqusEvent)
        {
            var package = GetDefaultPackage();

            SetIAPEventParameters(package, metriqusEvent);

            package.SetKey("iap_revenue");

            return package;
        }

        public Package BuildAttributionPackage(MetriqusAttribution metriqusAttribution)
        {
            var package = GetDefaultPackage();

            SetAttributionParameters(package, metriqusAttribution);

            package.SetKey("attribution");

            return package;
        }

        public Package BuildAdRevenueEventPackage(MetriqusAdRevenue adRevenue)
        {
            var package = GetDefaultPackage();

            SetAdRevenueEventParameters(package, adRevenue);

            package.SetKey("ad_revenue");

            return package;
        }

        public Package BuildCustomEventPackage(MetriqusCustomEvent customEvent)
        {
            var package = GetDefaultPackage();

            SetCustomEventParameters(package, customEvent);

            package.SetKey(customEvent.Key);

            return package;
        }

        private void SetIAPEventParameters(Package package, MetriqusInAppRevenue _event)
        {
            AddDefaultParameters(package);

            List<DynamicParameter> iapParameters = new List<DynamicParameter>();

            if (_event.Revenue != null)
            {
                AddInteger(iapParameters, "revenue", (int)(_event.Revenue * 1000000)); // multiply with 1m and convert to integer
            }

            AddString(iapParameters, "currency", _event.Currency);
            AddString(iapParameters, "product_id", _event.ProductId);
            AddString(iapParameters, "name", _event.Name);
            AddString(iapParameters, "brand", _event.Brand);
            AddString(iapParameters, "variant", _event.Variant);
            AddString(iapParameters, "category", _event.Category);
            AddString(iapParameters, "category2", _event.Category2);
            AddString(iapParameters, "category3", _event.Category3);
            AddString(iapParameters, "category4", _event.Category4);
            AddString(iapParameters, "category5", _event.Category5);
            AddDouble(iapParameters, "price", _event.Price);
            AddInteger(iapParameters, "quantity", _event.Quantity);
            AddDouble(iapParameters, "refund", _event.Refund);
            AddString(iapParameters, "coupon", _event.Coupon);
            AddString(iapParameters, "affiliation", _event.Affiliation);
            AddString(iapParameters, "location_id", _event.LocationId);
            AddString(iapParameters, "list_id", _event.ListId);
            AddString(iapParameters, "list_name", _event.ListName);
            AddInteger(iapParameters, "list_index", _event.ListIndex);
            AddString(iapParameters, "promotion_id", _event.PromotionId);
            AddString(iapParameters, "promotion_name", _event.PromotionName);
            AddString(iapParameters, "creative_name", _event.CreativeName);
            AddString(iapParameters, "creative_slot", _event.CreativeSlot);
            AddList(iapParameters, "item_params", _event.ItemParams);

            package.item = iapParameters;
        }

        private void SetAttributionParameters(Package package, MetriqusAttribution _attribution)
        {
            AddDefaultParameters(package);

            Dictionary<string, List<DynamicParameter>> attributionParams = new();

#if UNITY_IOS
            attributionParams.Add("ios", new List<DynamicParameter>());

            AddBoolean(attributionParams["ios"], "attribution", _attribution.Attribution);
            AddLong(attributionParams["ios"], "org_id", _attribution.OrgId);
            AddLong(attributionParams["ios"], "campaign_id", _attribution.CampaignId);
            AddString(attributionParams["ios"], "conversion_type", _attribution.ConversionType);
            AddString(attributionParams["ios"], "click_date", _attribution.ClickDate);
            AddString(attributionParams["ios"], "claim_type", _attribution.ClaimType);
            AddLong(attributionParams["ios"], "ad_group_id", _attribution.AdGroupId);
            AddString(attributionParams["ios"], "country_or_region", _attribution.CountryOrRegion);
            AddLong(attributionParams["ios"], "keyword_id", _attribution.KeywordId);
            AddLong(attributionParams["ios"], "attribution_ad_id", _attribution.AdId);
#elif UNITY_ANDROID
            attributionParams.Add("android", new List<DynamicParameter>());

            AddString(attributionParams["android"], "source", _attribution.Source);
            AddString(attributionParams["android"], "medium", _attribution.Medium);
            AddString(attributionParams["android"], "campaign", _attribution.Campaign);
            AddString(attributionParams["android"], "term", _attribution.Term);
            AddString(attributionParams["android"], "content", _attribution.Content);
#endif
            package.attribution = attributionParams;
        }

        private void SetAdRevenueEventParameters(Package package, MetriqusAdRevenue _event)
        {
            AddDefaultParameters(package);

            List<DynamicParameter> publisherParameters = new List<DynamicParameter>();

            AddString(publisherParameters, "ad_source", _event.GetSource());
            AddInteger(publisherParameters, "ad_revenue", (int)(_event.Revenue * 1000000)); // multiply with 1m and convert to integer
            AddString(publisherParameters, "ad_currency", _event.Currency);
            AddInteger(publisherParameters, "ad_impression_count", _event.AdImpressionsCount);
            AddString(publisherParameters, "ad_revenue_network", _event.AdRevenueNetwork);
            AddString(publisherParameters, "ad_revenue_unit", _event.AdRevenueUnit);
            AddString(publisherParameters, "ad_revenue_placement", _event.AdRevenuePlacement);

            package.publisher = publisherParameters;
        }

        private void SetCustomEventParameters(Package package, MetriqusCustomEvent _event)
        {
            AddDefaultParameters(package);

            package.parameters = _event.GetParameters();
        }

        private void AddDefaultParameters(Package package)
        {
            try
            {
                #region SET DEVICE INFO
                List<DynamicParameter> deviceParameters = new List<DynamicParameter>();

                AddString(deviceParameters, "unity_version", deviceInfo.unityVersion);
                AddString(deviceParameters, "device_type", deviceInfo.deviceType);
                AddString(deviceParameters, "device_name", deviceInfo.deviceName);
                AddString(deviceParameters, "device_model", deviceInfo.deviceModel);
                AddString(deviceParameters, "graphics_device_name", deviceInfo.graphicsDeviceName);
                AddString(deviceParameters, "os_name", deviceInfo.osName);
                AddInteger(deviceParameters, "system_memory_size", deviceInfo.systemMemorySize);
                AddInteger(deviceParameters, "graphics_memory_size", deviceInfo.graphicsMemorySize);
                AddString(deviceParameters, "language", deviceInfo.language);
                AddString(deviceParameters, "country", deviceInfo.country);
                AddFloat(deviceParameters, "screen_dpi", deviceInfo.screenDpi);
                AddInteger(deviceParameters, "screen_width", deviceInfo.screenWidth);
                AddInteger(deviceParameters, "screen_height", deviceInfo.screenHeight);
                AddString(deviceParameters, "device_id", deviceInfo.deviceId);
                AddString(deviceParameters, "vendor_id", deviceInfo.vendorId);
                AddInteger(deviceParameters, "platform", deviceInfo.platform);

                AddString(deviceParameters, "ad_id", Metriqus.GetAdid());
                AddBoolean(deviceParameters, "tracking_enabled", Metriqus.IsTrackingEnabled());

                package.device = deviceParameters;
                #endregion

                #region SET GEOLOCATION INFO
                var geolocation = Metriqus.GetGeolocation();

                List<DynamicParameter> geolocationParameters = new List<DynamicParameter>();

                AddString(geolocationParameters, "country", geolocation?.CountryCode ?? deviceInfo.country);
                AddString(geolocationParameters, "country_code", geolocation?.CountryCode ?? deviceInfo.country);
                AddString(geolocationParameters, "city", geolocation?.City ?? null);
                AddString(geolocationParameters, "region", geolocation?.Region ?? null);
                AddString(geolocationParameters, "region_name", geolocation?.RegionName ?? null);

                package.geolocation = geolocationParameters;
                #endregion

                #region SET APP INFO
                package.appInfo = new AppInfoPackage(deviceInfo.packageName, deviceInfo.appVersion);
                #endregion

                #region SET USER ATTRIBUTES
                package.userAttributes = Metriqus.GetUserAttributes();
                #endregion
            }
            catch (Exception e)
            {
                Metriqus.DebugLog("Package Builder, AddDefaultParameters Error: " + e.ToString(), UnityEngine.LogType.Error);
            }
        }

        private Package GetDefaultPackage()
        {
            return new Package(
                eventId: Guid.NewGuid().ToString(),
                sessionId: Metriqus.GetSessionId(),
                clientSdk: Metriqus.GetClientSdk(),
                isFirstLaunch: Metriqus.GetIsFirstLaunch(),
                userId: Metriqus.GetUniqueUserId(),
                eventTimestamp: MetriqusUtils.DateToTimestamp(createdAt),
                userFirstTouchTimestamp: Metriqus.GetUserFirstTouchTimestamp(),
                environment: metriqusSettings?.Environment ?? MetriqusEnvironment.Sandbox);
        }

        public static void AddString(List<DynamicParameter> parameters, string key, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            parameters.Add(new DynamicParameter(key, value));
        }

        public static void AddBoolean(List<DynamicParameter> parameters, string key, bool? value)
        {
            if (value == null || !value.HasValue) return;

            parameters.Add(new DynamicParameter(key, value));
        }

        public static void AddLong(List<DynamicParameter> parameters, string key, long? value)
        {
            if (value < 0 || !value.HasValue || value == null)
            {
                return;
            }

            parameters.Add(new DynamicParameter(key, value));
        }

        public static void AddDouble(List<DynamicParameter> parameters, string key, double? value)
        {
            if (value < 0 || value == null || !value.HasValue)
            {
                return;
            }

            parameters.Add(new DynamicParameter(key, (double)Math.Round((double)value, 5, MidpointRounding.AwayFromZero)));
        }

        public static void AddFloat(List<DynamicParameter> parameters, string key, float? value)
        {
            if (value < 0 || value == null || !value.HasValue)
            {
                return;
            }

            parameters.Add(new DynamicParameter(key, (float)Math.Round((float)value, 5, MidpointRounding.AwayFromZero)));
        }

        public static void AddDate(List<DynamicParameter> parameters, string key, DateTime value)
        {
            if (value == null)
            {
                return;
            }
            AddString(parameters, key, MetriqusUtils.ConvertDateToString(value));
        }

        public static void AddDuration(List<DynamicParameter> parameters, string key, long? durationInMilliSeconds)
        {
            if (durationInMilliSeconds < 0 || !durationInMilliSeconds.HasValue || durationInMilliSeconds == null)
            {
                return;
            }
            long durationInSeconds = (((long)durationInMilliSeconds) + 500) / 1000;
            AddLong(parameters, key, durationInSeconds);
        }

        public static void AddDoubleWithoutRounding(List<DynamicParameter> parameters, string key, double value)
        {
            parameters.Add(new DynamicParameter(key, value));
        }

        public static void AddInteger(List<DynamicParameter> parameters, string key, int? value)
        {
            parameters.Add(new DynamicParameter(key, value));
        }

        public static void AddList(List<DynamicParameter> parameters, string key, List<DynamicParameter> value)
        {
            if (value == null || value.Count <= 0)
            {
                return;
            }

            parameters.Add(new DynamicParameter(key, value));
        }

        public static void AddList(List<DynamicParameter> parameters, string key, List<TypedParameter> value)
        {
            if (value == null || value.Count <= 0)
            {
                return;
            }

            parameters.Add(new DynamicParameter(key, value));
        }
    }
    internal class Package
    {
        public string eventName;

        public string eventId;
        public string sessionId;
        public string clientSdk;
        public bool isFirstLaunch;
        public int eventTimestamp;
        public string userId;
        public int userFirstTouchTimestamp;
        public MetriqusEnvironment environment;

        public List<DynamicParameter> device = null;
        public List<DynamicParameter> geolocation = null;
        public AppInfoPackage appInfo = null;
        public List<DynamicParameter> item = null;
        public List<DynamicParameter> publisher = null;
        public Dictionary<string, List<DynamicParameter>> attribution = null;
        public List<TypedParameter> parameters = null;
        public List<TypedParameter> userAttributes = null;

        public Package()
        {
            parameters = new();
        }

        public Package(
            string eventId,
            string sessionId,
            string clientSdk,
            bool isFirstLaunch,
            string userId,
            int eventTimestamp,
            int userFirstTouchTimestamp,
            MetriqusEnvironment environment)
        {
            this.eventId = eventId;
            this.sessionId = sessionId;
            this.clientSdk = clientSdk;
            this.isFirstLaunch = isFirstLaunch;
            this.userId = userId;
            this.eventTimestamp = eventTimestamp;
            this.userFirstTouchTimestamp = userFirstTouchTimestamp;
            this.environment = environment;

            device = null;
            geolocation = null;
            appInfo = null;
            item = null;
            publisher = null;
            attribution = null;
            parameters = null;
            userAttributes = null;
        }

        public void SetKey(string _key)
        {
            this.eventName = _key;
        }

        public override string ToString()
        {
            string s = "";
            if (parameters != null)
            {
                s += "Typed Parameters: \n";
                foreach (var item in parameters)
                {
                    s += item.Name + ": " + item.Value + ",\n";
                }

            }

            if (device != null)
            {
                s += "Device Parameters: \n";

                foreach (var item in device)
                {
                    s += item.Name + ": " + item.Value + ",\n";
                }

            }

            if (publisher != null)
            {
                s += "Publisher Parameters: \n";

                foreach (var item in publisher)
                {
                    s += item.Name + ": " + item.Value + ",\n";
                }

            }

            if (item != null)
            {
                s += "Item Parameters: \n";

                foreach (var item in item)
                {
                    s += item.Name + ": " + item.Value + ",\n";
                }

            }

            if (attribution != null)
            {
                s += "Attribution Parameters: \n";

                foreach (var item in attribution)
                {
                    foreach (var item1 in item.Value)
                    {
                        s += item1.Name + ": " + item1.Value + ",\n";
                    }
                }

            }

            if (userAttributes != null)
            {
                s += "User Attributes Parameters: \n";

                foreach (var item in userAttributes)
                {
                    s += item.Name + ": " + item.Value + ",\n";
                }

            }

            if (geolocation != null)
            {
                s += "Geolocation Parameters: \n";

                foreach (var item in geolocation)
                {
                    s += item.Name + ": " + item.Value + ",\n";
                }

            }

            return s;
        }
    }
}