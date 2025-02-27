using MetriqusSdk.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MetriqusSdk
{
    internal static class EventRequestSender
    {
        public static async Task<bool> PostEventBatch(string eventsJson)
        {
            try
            {
                //Metriqus.DebugLog("Posting Event Batch: " + eventsJson);

                var metriqusSettings = Metriqus.GetMetriqusSettings();

                var remoteSettings = Metriqus.GetMetriqusRemoteSettings();

                if (remoteSettings == null || String.IsNullOrEmpty(remoteSettings.EventPostUrl))
                {
                    Metriqus.DebugLog("Can't find event post url", LogType.Warning);
                    return false;
                }
                else
                {
                    string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

                    string encryptedBody = Encrypt(eventsJson, metriqusSettings.ClientSecret, metriqusSettings.ClientKey);

                    string signature = CreateHmacSignature(metriqusSettings.ClientKey, metriqusSettings.ClientSecret, encryptedBody, timestamp);

                    var headers = new Dictionary<string, string>();

                    RequestSender.AddContentType(headers, RequestSender.ContentTypeJson);
                    RequestSender.AddAccept(headers, RequestSender.ContentTypeJson);
                    RequestSender.AddCustomHeader(headers, "ClientKey", metriqusSettings.ClientKey);
                    RequestSender.AddCustomHeader(headers, "Signature", signature);
                    RequestSender.AddCustomHeader(headers, "Timestamp", timestamp);

                    string encryptedJsonData = $"{{ \"encryptedData\": \"{encryptedBody}\" }}";

                    //Metriqus.DebugLog("encryptedJsonData: " + encryptedJsonData);

                    var response = await RequestSender.PostAsync(remoteSettings.EventPostUrl, encryptedJsonData, headers);

                    if (response.IsSuccess)
                    {
                        MetriqusResponseObject mro = MetriqusResponseObject.Parse(response.Data);

                        return mro.IsSuccess;
                    }
                    else
                    {
                        foreach (var error in response.Errors)
                        {
                            Metriqus.DebugLog($"Sending events failed. Error: {error}, message: {response.Data}", LogType.Error);
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Metriqus.DebugLog($"Sending events failed. Error: {e.ToString()}", LogType.Error);

                return false;
            }
        }

        private static string CreateHmacSignature(string clientKey, string clientSecret, string encryptedBody, string timestamp)
        {
            string data = $"{clientKey}{timestamp}{encryptedBody}";
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(clientSecret));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToBase64String(hash);
        }

        private static string Encrypt(string plainText, string _clientSecret, string _clientKey)
        {
            using var aes = Aes.Create();
            aes.Key = GenerateAESKey(_clientSecret);
            aes.IV = GenerateAESIV(_clientKey);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            using var sw = new StreamWriter(cs);

            sw.Write(plainText);
            sw.Flush();
            cs.FlushFinalBlock();

            return Convert.ToBase64String(ms.ToArray());
        }

        private static byte[] GenerateAESKey(string secret)
        {
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
            return hash[..32]; // Sadece ilk 32 byte'ý al
        }

        private static byte[] GenerateAESIV(string clientKey)
        {
            using var md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(clientKey));
            return hash[..16]; // Sadece ilk 16 byte'ý al
        }
    }
}