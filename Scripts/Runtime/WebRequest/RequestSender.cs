using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace MetriqusSdk.Web
{
    /// <summary>
    /// Wrapper class for sending web requests using UnityWebRequest.
    /// </summary>
    public static class RequestSender
    {
        public const string ContentTypeJson = "application/json";

        public static async Task<Response> GetAsync(string url, Dictionary<string, string> headers = null, int timeout = 0)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // Set Timeout
                if (timeout != 0)
                    webRequest.timeout = timeout;

                // Add optional headers
                AddHeaders(webRequest, headers);

                // Send the request asynchronously
                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield(); // Wait for the request to complete
                }

                ErrorType errorType = ErrorType.NoError;

                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    // Indicates a network issue, such as no internet connection or DNS resolution failure.
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"Connection (cannot reach the server) error: {webRequest.error}, url: {url}");
                }
                else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    errorType = ErrorType.ProtocolError;
                    // Indicates an HTTP error returned by the server (e.g., 404 Not Found, 500 Internal Server Error).
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"HTTP (protocol error) error: {webRequest.error}, url: {url}");
                }
                else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    errorType = ErrorType.DataProcessingError;

                    // Indicates an error while processing the response data.
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"Data processing (response was corrupted or not in correct format) error: {webRequest.error}, url: {url}");
                }

                // Collect response details
                string body = webRequest.downloadHandler.text;
                long statusCode = webRequest.responseCode;
                string error = webRequest.result != UnityWebRequest.Result.Success ? webRequest.error : null;

                return new Response(statusCode, body ?? "", (error != null) ? new string[] { error } : new string[] { }, errorType);
            }
        }

        public static async Task<Response> PostAsync(string url, string jsonBody, Dictionary<string, string> headers = null)
        {
            using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                // Encode the JSON body and set the upload handler
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();

                // Add optional headers
                AddHeaders(webRequest, headers);

                // Set the content type for JSON
                //webRequest.SetRequestHeader("Content-Type", ContentTypeJson);

                // Send the request asynchronously
                var operation = webRequest.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield(); // Wait for the request to complete
                }

                ErrorType errorType = ErrorType.NoError;

                if (webRequest.result == UnityWebRequest.Result.ConnectionError)
                {
                    // Indicates a network issue, such as no internet connection or DNS resolution failure.
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"Connection (cannot reach the server) error: {webRequest.error}, url: {url}");
                }
                else if (webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    errorType = ErrorType.ProtocolError;
                    // Indicates an HTTP error returned by the server (e.g., 404 Not Found, 500 Internal Server Error).
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"HTTP (protocol error) error: {webRequest.error}, url: {url}");
                }
                else if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    errorType = ErrorType.DataProcessingError;

                    // Indicates an error while processing the response data.
                    if (Metriqus.LogLevel != LogLevel.NoLog)
                        Metriqus.DebugLog($"Data processing (response was corrupted or not in correct format) error: {webRequest.error}, url: {url}");
                }

                // Collect response details
                string body = webRequest.downloadHandler.text;
                long statusCode = webRequest.responseCode;
                string error = webRequest.result != UnityWebRequest.Result.Success ? webRequest.error : null;

                string[] errors = error != null ? new string[] { error } : new string[] { };

                return new Response(statusCode, body ?? "", errors, errorType);
            }
        }

        /// <summary>
        /// Adds custom headers to the UnityWebRequest.
        /// </summary>
        /// <param name="webRequest">The UnityWebRequest to add headers to.</param>
        /// <param name="headers">The headers to add.</param>
        private static void AddHeaders(UnityWebRequest webRequest, Dictionary<string, string> headers)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }
        }

        /// <summary>
        /// Adds an Authorization header with a Bearer token.
        /// </summary>
        /// <param name="headers">The headers dictionary to add the Authorization header to.</param>
        /// <param name="token">The Bearer token.</param>
        public static void AddAuthorization(Dictionary<string, string> headers, string token)
        {
            if (headers != null)
            {
                headers["Authorization"] = $"Bearer {token}";
            }
        }

        /// <summary>
        /// Adds a Content-Type header.
        /// </summary>
        /// <param name="headers">The headers dictionary to add the Content-Type header to.</param>
        /// <param name="contentType">The value for the Content-Type header (e.g., application/json).</param>
        public static void AddContentType(Dictionary<string, string> headers, string contentType)
        {
            if (headers != null)
            {
                headers["Content-Type"] = contentType;
            }
        }

        /// <summary>
        /// Adds an Accept header.
        /// </summary>
        /// <param name="headers">The headers dictionary to add the Accept header to.</param>
        /// <param name="acceptType">The value for the Accept header (e.g., application/json).</param>
        public static void AddAccept(Dictionary<string, string> headers, string acceptType)
        {
            if (headers != null)
            {
                headers["Accept"] = acceptType;
            }
        }

        /// <summary>
        /// Adds a custom header to the headers dictionary.
        /// </summary>
        /// <param name="headers">The headers dictionary to add the custom header to.</param>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value.</param>
        public static void AddCustomHeader(Dictionary<string, string> headers, string key, string value)
        {
            if (headers != null)
            {
                headers[key] = value;
            }
        }

        /// <summary>
        /// Adds a User-Agent header.
        /// </summary>
        /// <param name="headers">The headers dictionary to add the User-Agent header to.</param>
        /// <param name="userAgent">The value for the User-Agent header.</param>
        public static void AddUserAgent(Dictionary<string, string> headers, string userAgent)
        {
            if (headers != null)
            {
                headers["User-Agent"] = userAgent;
            }
        }
    }
}