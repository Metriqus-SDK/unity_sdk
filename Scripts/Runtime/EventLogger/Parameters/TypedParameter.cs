using System;
using System.Collections.Generic;

namespace MetriqusSdk
{
    public class TypedParameter : IParameter
    {
        private string name;

        private string stringValue = null;
        private int? intValue = null;
        private float? floatValue = null;
        private bool? booleanValue = null;
        private long? longValue = null;

        public string Name => name;
        public object Value {
            get { 
                if (stringValue != null)
                    return stringValue;
                else if (intValue != null)
                    return intValue;
                else if (floatValue != null)
                    return floatValue;
                else if (booleanValue != null)
                    return booleanValue;
                else if (longValue != null)
                    return longValue;

                return null;
            }
        }

        public TypedParameter(string name, string value)
        {
            this.name = name;
            this.stringValue = value;
        }

        public TypedParameter(string name, int value)
        {
            this.name = name;
            this.intValue = value;
        }

        public TypedParameter(string name, float value)
        {
            this.name = name;
            this.floatValue = value;
        }

        public TypedParameter(string name, bool value)
        {
            this.name = name;
            this.booleanValue = value;
        }

        public TypedParameter(string name, long value)
        {
            this.name = name;
            this.longValue = value;
        }

        public string Serialize()
        {
            string json = $"{{\n" +
                $"\"key\": \"{name}\",\n" +
                $"\"value\": {{\n";

            if (stringValue != null)
            {
                json += $"\"string_value\": \"{stringValue}\"\n";
            }
            else if (intValue != null)
            {
                json += $"\"int_value\": {MetriqusJSON.SerializeValue(intValue)}\n";
            }
            else if (booleanValue != null)
            {
                json += $"\"bool_value\": {MetriqusJSON.SerializeValue(booleanValue)}\n";
            }
            else if (floatValue != null)
            {
                json += $"\"float_value\": {MetriqusJSON.SerializeValue(floatValue)}\n";
            }
            else if (longValue != null)
            {
                json += $"\"long_value\": {MetriqusJSON.SerializeValue(longValue)}\n";
            }

            json += "}\n}\n";

            return json;
        }

        public static TypedParameter Deserialize(JSONNode jsonNode)
        {
            if (jsonNode == null) return null;

            string name;

            try
            {
                name = MetriqusJSON.GetJsonString(jsonNode, "key");
            }
            catch (Exception e)
            {
                Metriqus.DebugLog("TypedParameter Deserialize Error:\n" + e.ToString() + ",\njson: " + jsonNode, UnityEngine.LogType.Error);
                return null;
            }

            try
            {
                var valueObj = jsonNode["value"] as JSONClass;

                if (valueObj != null)
                {
                    foreach (var item in valueObj)
                    {
                        var pair = (KeyValuePair<string, JSONNode>)item;

                        if (pair.Key == "string_value")
                            return new TypedParameter(name, pair.Value);
                        else if (pair.Key == "int_value")
                            return new TypedParameter(name, MetriqusJSON.ParseInt(pair.Value));
                        else if (pair.Key == "bool_value")
                            return new TypedParameter(name, MetriqusJSON.ParseBool(pair.Value));
                        else if (pair.Key == "float_value")
                            return new TypedParameter(name, MetriqusJSON.ParseFloat(pair.Value));
                        else if (pair.Key == "long_value")
                            return new TypedParameter(name, MetriqusJSON.ParseLong(pair.Value));
                    }
                }
            }
            catch (Exception e)
            {
                Metriqus.DebugLog("TypedParameter Deserialize Error:\n" + e.ToString() + ",\njson: " + jsonNode, UnityEngine.LogType.Error);
                return null;
            }

            return null;
        }

        public static string Serialize(IEnumerable<TypedParameter> parameters)
        {
            string json = "[";

            foreach (var item in parameters)
            {
                json += item.Serialize() + ",\n";
            }

            // Remove trailing comma and close JSON
            if (json.EndsWith(",\n"))
            {
                json = json[..^2];
            }

            json += "]";
            return json;
        }

        public static List<TypedParameter> Deserialize(JSONArray array)
        {
            if (array == null) return null;

            var list = new List<TypedParameter>();

            foreach (var itemParamNode in array.Childs)
            {
                var typedParameter = Deserialize(itemParamNode);

                if (typedParameter != null)
                {
                    list.Add(typedParameter);
                }
            }

            return list;
        }
    }
}