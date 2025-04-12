using MetriqusSdk.Web;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace MetriqusSdk
{
    public class MetaUtmDecryptionRequest
    {
        public string data;
        public string nonce;
        public string bundle;
        public string uid;
    }

    public static class MetaAttributionUtilities
    {
        public const string FacebookUtmSource = "apps.facebook.com";
        public const string InstagramUtmSource = "apps.instagram.com";

        private const string MetaUtmDecryptionUrl = "https://mtrqs.com/meta/decrypt";

        public static bool IsMetaUtm(string utmSource)
        {
            if (string.IsNullOrEmpty(utmSource))
                return false;

            return utmSource == FacebookUtmSource || utmSource == InstagramUtmSource;
        }

        public static async Task<string> DecryptMetaUtm(string utmContent)
        {
            if (utmContent == null) return null;

            var jsonNode = JSON.Parse(utmContent);

            if (jsonNode == null)
            {
                return null;
            }

            var source = jsonNode["source"];

            if (source == null) return null;

            var data = source["data"];
            var nonce = source["nonce"];

            if (data == null || nonce == null) return null;

            var headers = new Dictionary<string, string>();
            RequestSender.AddContentType(headers, RequestSender.ContentTypeJson);
            RequestSender.AddAccept(headers, RequestSender.ContentTypeJson);

            MetaUtmDecryptionRequest req = new()
            {
                data = data.Value,
                nonce = nonce.Value,
                bundle = "com.tiamogames.mergegame",
                uid = "B96ACF896222B65F",
            };

            var response = await RequestSender.PostAsync(MetaUtmDecryptionUrl, JsonUtility.ToJson(req), headers);

            if(response.IsSuccess)
                return response.Data;
            else
            {
                Metriqus.DebugLog($"DecryptMetaUtm failed, status code: {response.StatusCode} error: {response.Errors[0]}");
                return null;
            }
        }
    }
}