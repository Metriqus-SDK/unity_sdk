using MetriqusSdk.Web;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MetriqusSdk
{
    public static class IPGeolocation
    {
        // two different api service if one fails will use other
        private const string ApiUrl = "https://sdk.metriqus.com/event/geo";

        /// <summary>
        /// Fetch Geolocation data by ip
        /// </summary>
        /// <returns></returns>
        public static async Task<Geolocation> GetCountryByIP()
        {
            Dictionary<string, string> header = new();

            RequestSender.AddContentType(header, RequestSender.ContentTypeJson);
            RequestSender.AddAccept(header, RequestSender.ContentTypeJson);

            Geolocation location = null;

            var response = await RequestSender.GetAsync(ApiUrl, header);

            // if it is successful parse and return
            if (response.IsSuccess)
            {
                MetriqusResponseObject mro = MetriqusResponseObject.Parse(response.Data);

                if (mro != null)
                {
                    location = ParseApiJson(mro.Data);
                }
            }

            return location;
        }

        private static Geolocation ParseApiJson(string value)
        {
            var jsonNode = JSON.Parse(value);

            if (jsonNode == null)
            {
                return null;
            }

            Geolocation info = new Geolocation();

            try
            {
                info.Country = MetriqusJSON.GetJsonString(jsonNode, "country");
            }
            catch (Exception)
            { }

            try
            {
                info.CountryCode = MetriqusJSON.GetJsonString(jsonNode, "countryCode");
            }
            catch (Exception)
            { }

            try
            {
                info.Region = MetriqusJSON.GetJsonString(jsonNode, "region");
            }
            catch (Exception)
            { }

            try
            {
                info.RegionName = MetriqusJSON.GetJsonString(jsonNode, "regionName");
            }
            catch (Exception)
            { }

            try
            {
                info.City = MetriqusJSON.GetJsonString(jsonNode, "city");
            }
            catch (Exception)
            { }

            return info;
        }

        public class Geolocation
        {
            public string Country;
            public string CountryCode;
            public string City;
            public string Region;
            public string RegionName;
        }
    }
}