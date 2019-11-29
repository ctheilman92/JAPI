using System;
using System.Net;
using System.Text;
using RestSharp;

namespace JAPI.Repo
{
    public class RestException : Exception
    {
        public RestException(HttpStatusCode httpStatusCode, Uri requestUri, string content, string message, Exception innerException)
          : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
            RequestUri = requestUri;
            Content = content;
        }

        public HttpStatusCode HttpStatusCode { get; private set; }

        public Uri RequestUri { get; private set; }

        public string Content { get; private set; }

        public static RestException CreateException(Uri requestUri, IRestResponse response)
        {
            Exception innerException = null;

            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine($"REST Error completing response from URI {requestUri}. Check inner details for more info..");
            if (!response.StatusCode.IsScuccessStatusCode())
            {
                messageBuilder.AppendLine($"- REST Response Status Code [{ response.StatusDescription}]");
            }

            if (response.ErrorException != null)
            {
                messageBuilder.AppendLine($"- An exception occurred while processing REST Request: {response.ErrorMessage}");
                innerException = response.ErrorException;
            }

            return new RestException(response.StatusCode, requestUri, response.Content, messageBuilder.ToString(), innerException);
        }
    }

}
