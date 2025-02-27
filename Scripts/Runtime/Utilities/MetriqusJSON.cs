using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace MetriqusSdk
{
    public static class MetriqusJSON
    {
        public static string GetJsonString(JSONNode node, string key)
        {
            if (node == null)
            {
                return null;
            }

            // Access value object and cast it to JSONData.
            var nodeValue = node[key] as JSONData;

            if (nodeValue == null)
            {
                return null;
            }

            if (nodeValue == "")
            {
                return null;
            }

            return nodeValue.Value;
        }

        public static long GetJsonLong(JSONNode node, string key)
        {
            if (node == null)
            {
                return 0;
            }

            // Access value object and cast it to JSONData.
            var nodeValue = node[key] as JSONData;

            if (nodeValue == null)
            {
                return 0;
            }

            if (nodeValue == "")
            {
                return 0;
            }

            return ParseLong(nodeValue.Value);
        }

        public static int GetJsonInt(JSONNode node, string key)
        {
            if (node == null)
            {
                return 0;
            }

            // Access value object and cast it to JSONData.
            var nodeValue = node[key] as JSONData;

            if (nodeValue == null)
            {
                return 0;
            }

            if (nodeValue == "")
            {
                return 0;
            }

            return ParseInt(nodeValue.Value);
        }

        public static float GetJsonFloat(JSONNode node, string key)
        {
            if (node == null)
            {
                return 0;
            }

            // Access value object and cast it to JSONData.
            var nodeValue = node[key] as JSONData;

            if (nodeValue == null)
            {
                return 0;
            }

            if (nodeValue == "")
            {
                return 0;
            }

            return ParseFloat(nodeValue.Value);
        }

        public static bool GetJsonBool(JSONNode node, string key)
        {
            if (node == null)
            {
                return false;
            }

            // Access value object and cast it to JSONData.
            var nodeValue = node[key] as JSONData;

            if (nodeValue == null)
            {
                return false;
            }

            if (nodeValue == "")
            {
                return false;
            }

            return ParseBool(nodeValue.Value);
        }

        // PARSE STRING TO PRIMITIVE
        public static long ParseLong(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longResult))
            {
                return longResult;
            }

            return 0;
        }

        public static int ParseInt(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intResult))
            {
                return intResult;
            }

            return 0;
        }

        public static float ParseFloat(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatResult))
            {
                return floatResult;
            }

            return 0;
        }

        public static double ParseDouble(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleResult))
            {
                return doubleResult;
            }

            return 0;
        }

        public static bool ParseBool(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (bool.TryParse(value, out bool boolResult))
            {
                return boolResult;
            }

            return false;
        }


        // SERIALIZE OBJECT TO JSON
        public static string SerializeValue(object value)
        {
            return value switch
            {
                string str => $"\"{str}\"", // Quote strings
                TypedParameter typedParameter => typedParameter.Serialize(),
                bool boolValue => boolValue.ToString(CultureInfo.InvariantCulture).ToLower(),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture), // Format numbers/dates
                IDictionary dictionary => SerializeDictionary(dictionary), // Serialize dictionaries recursively
                IEnumerable enumerable => SerializeArray(enumerable), // Serialize arrays/lists
                null => "null", // Handle null
                _ => $"\"{value}\"" // Default: Quote as string
            };
        }

        private static string SerializeDictionary(IDictionary dictionary)
        {
            var result = "{";

            foreach (DictionaryEntry entry in dictionary)
            {
                string key = entry.Key.ToString();
                string value = SerializeValue(entry.Value);
                result += $"\"{key}\" : {value}, ";
            }

            if (result.EndsWith(", "))
            {
                result = result[..^2];
            }

            result += "}";
            return result;
        }

        private static string SerializeArray(IEnumerable enumerable)
        {
            var result = "[";

            foreach (var item in enumerable)
            {
                result += SerializeValue(item) + ", ";
            }

            if (result.EndsWith(", "))
            {
                result = result[..^2];
            }

            result += "]";
            return result;
        }
        //  END - SERIALIZE OBJECT TO JSON

        // CONVERT JSON TO OBJECT
        public static bool TryParseValue(JSONNode jsonNode, out object result)
        {
            try
            {
                if (jsonNode is JSONClass)
                {
                    Dictionary<string, object> dict = new();

                    var jsonDict = jsonNode as JSONClass;
                    foreach (var dictItem in jsonDict)
                    {
                        var item = (KeyValuePair<string, JSONNode>)dictItem;

                        if (TryParseValue(item.Value, out object val))
                        {
                            dict.Add(item.Key, val);
                        }
                    }

                    result = dict;

                    return true;
                }
                else if (jsonNode is JSONArray)
                {
                    var jsonArray = jsonNode as JSONArray;

                    object[] array = new object[jsonArray.Count];

                    int i = 0;
                    foreach (var arrayItem in jsonArray.Childs)
                    {
                        if (TryParseValue(arrayItem, out object val))
                        {
                            array[i] = val;
                        }
                        i++;
                    }

                    result = array;

                    return true;
                }
                else
                {
                    string value = jsonNode.Value.Trim();
                    
                    if (bool.TryParse(value, out bool boolResult))
                    {
                        result = boolResult;
                    }
                    else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intResult))
                    {
                        result = intResult;
                    }
                    else if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longResult))
                    {
                        result = longResult;
                    }
                    else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatResult))
                    {
                        result = floatResult;
                    }
                    else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleResult))
                    {
                        result = doubleResult;
                    }
                    else
                    {
                        result = value;
                    }

                    return true;
                }
            }
            catch// (Exception ex)
            {
                result = "";
                return false;
            }
        }
    }
}