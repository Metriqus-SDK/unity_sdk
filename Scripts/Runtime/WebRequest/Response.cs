namespace MetriqusSdk.Web
{
    /// <summary>
    /// Error type of http request
    /// </summary>
    public enum ErrorType
    {
        // Özet:
        //     Failed to communicate with the server. For example, the request couldn't connect
        //     or it could not establish a secure channel.
        ConnectionError,
        //
        // Özet:
        //     The server returned an error response. The request succeeded in communicating
        //     with the server, but received an error as defined by the connection protocol.
        ProtocolError,
        //
        // Özet:
        //     Error processing data. The request succeeded in communicating with the server,
        //     but encountered an error when processing the received data. For example, the
        //     data was corrupted or not in the correct format.
        DataProcessingError,
        //
        // Özet:
        //     No Error
        NoError
    }

    /// <summary>
    /// Struct to encapsulate response data from a web request.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// The HTTP status code of the response.
        /// </summary>
        public long StatusCode;

        /// <summary>
        /// The body of the response as a string.
        /// </summary>
        public string Data;

        /// <summary>
        /// Any error message associated with the request, or null if successful.
        /// </summary>
        public string[] Errors;

        public ErrorType ErrorType;

        public Response(long statusCode, string data, string[] errors, ErrorType errorType)
        {
            StatusCode = statusCode;
            Data = data;
            Errors = errors;
            ErrorType = errorType;
        }

        /// <summary>
        /// Indicates whether the request was successful based on the status code and error.
        /// </summary>
        public bool IsSuccess => (Errors == null || Errors.Length == 0) 
            && StatusCode >= 200 && StatusCode < 300 
            && (ErrorType == ErrorType.NoError);
    }
}