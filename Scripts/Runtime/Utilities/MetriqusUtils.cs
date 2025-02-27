using System;
using System.Collections.Generic;
using System.Globalization;

namespace MetriqusSdk
{
    public class MetriqusUtils
    {
        public static string KeySource = "utm_source";
        public static string KeyMedium = "utm_medium";
        public static string KeyCampaign = "utm_campaign";
        public static string KeyTerm = "utm_term";
        public static string KeyContent = "utm_content";


        public static int ConvertBool(bool? value)
        {
            if (value == null)
            {
                return -1;
            }
            if (value.Value)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static double ConvertDouble(double? value)
        {
            if (value == null)
            {
                return -1;
            }

            return (double)value;
        }

        public static int ConvertInt(int? value)
        {
            if (value == null)
            {
                return -1;
            }

            return (int)value;
        }

        public static long ConvertLong(long? value)
        {
            if (value == null)
            {
                return -1;
            }

            return (long)value;
        }

        public static string TryGetValue(Dictionary<string, string> dictionary, string key)
        {
            string value;
            if (dictionary.TryGetValue(key, out value))
            {
                if (value == "")
                {
                    return null;
                }
                return value;
            }
            return null;
        }

        /// <summary>
        /// Parses and sanitizes query string parameters.
        /// </summary>
        /// <param name="queryString">The URL query string.</param>
        /// <returns>A dictionary of sanitized query parameters.</returns>
        public static Dictionary<string, string> ParseAndSanitize(string queryString)
        {
            var sanitizedParameters = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(queryString))
                return sanitizedParameters;

            // Ensure the query string starts with '?' if it doesn't already
            if (!queryString.StartsWith("?"))
                queryString = "?" + queryString;

            var uri = new Uri("http://dummy" + queryString); // Dummy base URI to enable parsing
            var query = uri.Query;

            // Split query parameters
            var parameters = query.TrimStart('?').Split('&');
            foreach (var param in parameters)
            {
                var keyValue = param.Split(new[] { '=' }, 2); // Split on the first '=' only
                if (keyValue.Length != 2)
                    continue;

                var key = Uri.UnescapeDataString(keyValue[0]).Trim();
                var value = Uri.UnescapeDataString(keyValue[1]).Trim();

                if (!string.IsNullOrEmpty(key))
                    sanitizedParameters[key] = value;
            }

            return sanitizedParameters;
        }

        public static string ConvertDateToString(DateTime date)
        {
            DateTime _date = date.ToUniversalTime();

            return _date.ToString("yyyy-MM-ddTHH:mm:ss.fffK");
        }

        public static DateTime ParseDate(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-ddTHH:mm:ss.fffK", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime _date))
            {
                return _date;

            }

            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static int DateToTimestamp(DateTime date)
        {
            return (int)((DateTimeOffset)date).ToUnixTimeSeconds();
        }
    }
}
