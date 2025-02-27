using System;
using System.Collections.Generic;

namespace MetriqusSdk
{
    public class MetriqusResponseObject
    {
        public string Data;
        public long StatusCode;
        public string[] ErrorMessages;
        public bool IsSuccess => (ErrorMessages == null || ErrorMessages.Length == 0)
                            && StatusCode >= 200 && StatusCode < 300;

        public MetriqusResponseObject(string data, long statusCode, string[] errors)
        {
            this.Data = data;
            this.StatusCode = statusCode;
            this.ErrorMessages = errors;
        }

        public static MetriqusResponseObject Parse(string json)
        {
            if (json == null) return null;

            var jsonNode = JSON.Parse(json);

            if (jsonNode == null)
            {
                return null;
            }

            long statusCode = 0;
            string data = "";
            List<string> errors = new();

            try {
                var dataValue = jsonNode["data"] ;

                if (dataValue == null)
                {
                    return null;
                }

                data = dataValue.ToString();
            } catch (Exception) { return null; }
            try { statusCode = MetriqusJSON.GetJsonLong(jsonNode, "statusCode"); } catch (Exception) { return null; }
            try
            {
                var errorArray = jsonNode["errorMessages"];

                foreach (var itemParamNode in errorArray.Childs)
                {
                    errors.Add(itemParamNode);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return new MetriqusResponseObject(data,statusCode,errors.ToArray());
        }
    }
}