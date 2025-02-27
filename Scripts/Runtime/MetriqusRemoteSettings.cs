namespace MetriqusSdk
{
    internal sealed class MetriqusRemoteSettings
    {
        public int MaxEventBatchCount = 10;
        public int MaxEventStoreSeconds = 30 * 60;// 30 minutes //86400; // a day

        public int SendEventIntervalSeconds = 2;

        public int SessionIntervalMinutes = 30;

        public int AttributionCheckWindow = 20;
        public int GeolocationFetchIntervalDays = 2;

        public string EventPostUrl;

        public static MetriqusRemoteSettings Parse(string json)
        {
            var jsonNode = JSON.Parse(json);

            if (jsonNode == null)
            {
                return null;
            }

            MetriqusRemoteSettings mRemoteSettings = new();

            try
            {
                mRemoteSettings.MaxEventBatchCount = int.Parse(MetriqusJSON.GetJsonString(jsonNode, "maxEventBatchCount"));
            }
            catch
            { }

            try
            {
                mRemoteSettings.MaxEventStoreSeconds = int.Parse(MetriqusJSON.GetJsonString(jsonNode, "maxEventStoreSeconds"));
            }
            catch
            { }

            try
            {
                mRemoteSettings.SendEventIntervalSeconds = int.Parse(MetriqusJSON.GetJsonString(jsonNode, "sendEventIntervalSeconds"));
            }
            catch
            { }

            try
            {
                mRemoteSettings.SessionIntervalMinutes = int.Parse(MetriqusJSON.GetJsonString(jsonNode, "sessionIntervalMinutes"));
            }
            catch
            { }

            try
            {
                mRemoteSettings.AttributionCheckWindow = int.Parse(MetriqusJSON.GetJsonString(jsonNode, "attributionCheckWindow"));
            }
            catch
            { }

            try
            {
                mRemoteSettings.GeolocationFetchIntervalDays = int.Parse(MetriqusJSON.GetJsonString(jsonNode, "geolocationFetchIntervalDays"));
            }
            catch
            { }

            try
            {
                mRemoteSettings.EventPostUrl = MetriqusJSON.GetJsonString(jsonNode, "eventPostUrl");
            }
            catch
            { }

            return mRemoteSettings;
        }
    }
}
