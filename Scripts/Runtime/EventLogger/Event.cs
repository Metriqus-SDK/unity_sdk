using System;
using System.Collections.Generic;

namespace MetriqusSdk
{
    /// <summary>
    /// Metriqus Logger Event.
    /// </summary>
    internal class Event
    {
        public string eventName;

        public string eventId;
        public string sessionId;
        public string clientSdk;
        public bool isFirstLaunch;
        public long eventTimestamp;
        public string userId;
        public long userFirstTouchTimestamp;
        public string environment;

        public IEnumerable<TypedParameter> parameters = null;
        public IEnumerable<TypedParameter> userAttributes = null;
        public IEnumerable<DynamicParameter> device = null;
        public IEnumerable<DynamicParameter> geolocation = null;
        public AppInfoPackage appInfo = null;
        public IEnumerable<DynamicParameter> item = null;
        public IEnumerable<DynamicParameter> publisher = null;
        public IDictionary<string, List<DynamicParameter>> attribution = null;

        public string EventName => eventName;
        public IEnumerable<TypedParameter> Parameters => parameters;

        public Event(string eventName)
        {
            this.eventName = eventName;
        }

        public Event(string eventName, IEnumerable<TypedParameter> parameters)
        {
            this.eventName = eventName;
            this.parameters = parameters;
        }

        public Event(Package package)
        {
            this.eventName = package.eventName;
            this.eventId = package.eventId;
            this.sessionId = package.sessionId;
            this.eventTimestamp = package.eventTimestamp;
            this.clientSdk = package.clientSdk;
            this.isFirstLaunch = package.isFirstLaunch;
            this.userId = package.userId;
            this.userFirstTouchTimestamp = package.userFirstTouchTimestamp;
            this.environment = package.environment.ToLowercaseString();

            this.device = package.device;
            this.geolocation = package.geolocation;
            this.appInfo = package.appInfo;
            this.publisher = package.publisher;
            this.item = package.item;
            this.attribution = package.attribution;
            this.parameters = package.parameters;
            this.userAttributes = package.userAttributes;
        }


        public Event(
            string eventName,
            string eventId,
            string sessionId,
            long eventTimestamp,
            string clientSdk,
            bool isFirstLaunch,
            string userId,
            long userFirstTouchTimestamp,
            string environment,
            IEnumerable<TypedParameter> parameters = null,
            IEnumerable<TypedParameter> userAttributes = null,
            IEnumerable<DynamicParameter> device = null,
            IEnumerable<DynamicParameter> geolocation = null,
            AppInfoPackage appInfo = null,
            IEnumerable<DynamicParameter> item = null,
            IEnumerable<DynamicParameter> publisher = null,
            IDictionary<string, List<DynamicParameter>> attribution = null)
        {
            this.eventName = eventName;
            this.eventId = eventId;
            this.sessionId = sessionId;
            this.eventTimestamp = eventTimestamp;
            this.clientSdk = clientSdk;
            this.isFirstLaunch = isFirstLaunch;
            this.userId = userId;
            this.userFirstTouchTimestamp = userFirstTouchTimestamp;
            this.environment = environment;
            this.device = device;
            this.geolocation = geolocation;
            this.appInfo = appInfo;
            this.publisher = publisher;
            this.item = item;
            this.attribution = attribution;
            this.parameters = parameters;
            this.userAttributes = userAttributes;
        }

        /// <summary>
        /// Convert event to json
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            string result = $"{{\n" +
                $"\"event_name\" : \"{eventName}\",\n" +
                $"\"event_id\" : \"{eventId}\",\n" +
                $"\"session_id\" : \"{sessionId}\",\n" +
                $"\"client_sdk\" : \"{clientSdk}\",\n" +
                $"\"is_first_launch\" : {MetriqusJSON.SerializeValue(isFirstLaunch)},\n" +
                $"\"event_timestamp\" : {MetriqusJSON.SerializeValue(eventTimestamp)},\n" +
                $"\"user_id\" : \"{userId}\",\n" +
                $"\"user_first_touch_timestamp\" : {MetriqusJSON.SerializeValue(userFirstTouchTimestamp)},\n" +
                $"\"environment\" : \"{environment}\",\n";

            AddDynamicParametersToJson(ref result, "device", device);
            AddDynamicParametersToJson(ref result, "geo", geolocation);
            AddDynamicParametersToJson(ref result, "item", item);
            AddDynamicParametersToJson(ref result, "publisher", publisher);

            // SERIALIZE APP INFO
            if (appInfo != null)
                result += $"\"app_info\": {appInfo.Serialize()},\n";

            if (attribution != null)
            {
                result += "\"attribution\": {\n";

                foreach (var attributionItem in attribution)
                {
                    AddDynamicParametersToJson(ref result, attributionItem.Key, attributionItem.Value);
                }

                // Remove trailing comma and close JSON
                if (result.EndsWith(",\n"))
                {
                    result = result[..^2];
                }

                result += "},\n";
            }

            if (parameters != null)
            {
                result += $"\"event_params\": {TypedParameter.Serialize(parameters)},\n";
            }

            if (userAttributes != null)
            {
                result += $"\"user_properties\": {TypedParameter.Serialize(userAttributes)},\n";
            }

            // Remove trailing comma and close JSON
            if (result.EndsWith(",\n"))
            {
                result = result[..^2];
            }

            result += "}";

            return result;
        }

        private void AddDynamicParametersToJson(ref string jsonString, string key, IEnumerable<DynamicParameter> parameters)
        {
            if (parameters != null)
            {
                jsonString += $"\"{key}\" : {{\n";

                foreach (var item in parameters)
                {
                    string val = MetriqusJSON.SerializeValue(item.Value);
                    jsonString += $"\"{item.Name}\" : {val},\n";
                }

                // Remove trailing comma and close JSON
                if (jsonString.EndsWith(",\n"))
                {
                    jsonString = jsonString[..^2];
                }

                jsonString += "},\n";
            }
        }

        public static Event ParseJson(JSONNode jsonNode)
        {
            if (jsonNode == null) return null;

            string eventName, eventId, sessionId, clientSdk, userId, environment;
            bool isFirstLaunch = false;
            long userFirstTouchTimestamp = 0, eventTimestamp = 0;

            // DESIRIALIZE BASIC PARAMETERS
            try
            {
                eventName = MetriqusJSON.GetJsonString(jsonNode, "event_name");
                eventId = MetriqusJSON.GetJsonString(jsonNode, "event_id");
                sessionId = MetriqusJSON.GetJsonString(jsonNode, "session_id");
                clientSdk = MetriqusJSON.GetJsonString(jsonNode, "client_sdk");
                userId = MetriqusJSON.GetJsonString(jsonNode, "user_id");
                environment = MetriqusJSON.GetJsonString(jsonNode, "environment");

                isFirstLaunch = MetriqusJSON.GetJsonBool(jsonNode, "is_first_launch");

                userFirstTouchTimestamp = MetriqusJSON.GetJsonLong(jsonNode, "user_first_touch_timestamp");
                eventTimestamp = MetriqusJSON.GetJsonLong(jsonNode, "event_timestamp");
            }
            catch (Exception)
            {
                return null;
            }

            // DESIRIALIZE DEVICE PARAMETERS
            List<DynamicParameter> device = null;
            ParseDynamicParameters(jsonNode, "device", ref device);

            // DESIRIALIZE GEOLOCATION PARAMETERS
            List<DynamicParameter> geolocation = null;
            ParseDynamicParameters(jsonNode, "geo", ref geolocation);

            // DESIRIALIZE APP INFO PARAMETERS
            AppInfoPackage appInfoPackage = AppInfoPackage.Deserialize(jsonNode["app_info"]);

            // DESIRIALIZE PUBLISHER PARAMETERS
            List<DynamicParameter> publisher = null;
            ParseDynamicParameters(jsonNode, "publisher", ref publisher);

            Dictionary<string, List<DynamicParameter>> attribution = null;

            // DESIRIALIZE ATTRIBUTION PARAMETERS
            var attributionNode = jsonNode["attribution"];

            if (attributionNode != null)
            {
                attribution = new();
#if UNITY_IOS
                // IOS ATTRIBUTION
                List<DynamicParameter> attributionIosParameters = null;

                ParseDynamicParameters(attributionNode, "ios", ref attributionIosParameters);

                if (attributionIosParameters != null && attributionIosParameters.Count > 0)
                    attribution.Add("ios", attributionIosParameters);
#elif UNITY_ANDROID
                // ANDROID ATTRIBUTION
                List<DynamicParameter> attributionAndroidParameters = null;

                ParseDynamicParameters(attributionNode, "android", ref attributionAndroidParameters);

                if (attributionAndroidParameters != null && attributionAndroidParameters.Count > 0)
                    attribution.Add("android", attributionAndroidParameters);
#endif

                if (attribution.Count == 0)
                    attribution = null;
            }


            List<DynamicParameter> itemParameters = null;
            var itemNode = jsonNode["item"];

            if (itemNode != null)
            {
                itemParameters = new();

                var parameters = itemNode as JSONClass;

                foreach (var _item in parameters)
                {
                    var pair = (KeyValuePair<string, JSONNode>)_item;

                    // DESERIALIZE TYPED PARAMETERS OF ITEM LIST
                    if (pair.Key == "item_params" && pair.Value != null)
                    {
                        List<TypedParameter> typedParameters = TypedParameter.Deserialize(pair.Value.AsArray);

                        if (typedParameters != null && typedParameters.Count > 0)
                            itemParameters.Add(new DynamicParameter(pair.Key, typedParameters));
                    }
                    else if (MetriqusJSON.TryParseValue(pair.Value, out object val))
                    {
                        itemParameters.Add(new DynamicParameter(pair.Key, val));
                    }

                    if (itemParameters.Count == 0)
                        itemParameters = null;
                }
            }

            List<TypedParameter> eventParameters = null;
            var eventParamsNode = jsonNode["event_params"];

            if (eventParamsNode != null)
            {
                eventParameters = TypedParameter.Deserialize(eventParamsNode.AsArray);

                if (eventParameters != null && eventParameters.Count == 0)
                    eventParameters = null;
            }

            List<TypedParameter> userProperties = null;
            var userPropertiesNode = jsonNode["user_properties"];

            if (userPropertiesNode != null)
            {
                userProperties = TypedParameter.Deserialize(userPropertiesNode.AsArray);

                if (userProperties != null && userProperties.Count == 0)
                    userProperties = null;
            }

            return new Event(
                eventName: eventName,
                eventId: eventId,
                sessionId: sessionId,
                eventTimestamp: eventTimestamp,
                clientSdk: clientSdk,
                isFirstLaunch: isFirstLaunch,
                userId: userId,
                userFirstTouchTimestamp: userFirstTouchTimestamp,
                environment: environment,
                parameters: eventParameters,
                userAttributes: userProperties,
                device: device,
                geolocation: geolocation,
                appInfo: appInfoPackage,
                item: itemParameters,
                publisher: publisher,
                attribution: attribution);
        }

        private static void ParseDynamicParameters(JSONNode node, string key, ref List<DynamicParameter> list)
        {
            if (node != null)
            {
                var parameters = node[key] as JSONClass;

                if (parameters != null)
                {
                    if (list == null)
                        list = new List<DynamicParameter>();

                    foreach (var item in parameters)
                    {
                        var pair = (KeyValuePair<string, JSONNode>)item;

                        if (MetriqusJSON.TryParseValue(pair.Value, out object val))
                        {
                            list.Add(new DynamicParameter(pair.Key, val));
                        }
                    }

                    if (list.Count == 0)
                        list = null;

                    return;
                }
            }

            list = null;
        }
    }
}