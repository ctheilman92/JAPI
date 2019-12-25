using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace JAPI.Repo
{
    internal static class RestSharpExtensions
    {
        public static bool IsScuccessStatusCode(this HttpStatusCode responseCode)
        {
            var numericResponse = (int)responseCode;
            const int statusCodeOk = (int)HttpStatusCode.OK;
            const int statusCodeBadRequest = (int)HttpStatusCode.BadRequest;

            return numericResponse >= statusCodeOk &&
                   numericResponse < statusCodeBadRequest;
        }

        public static bool IsSuccessful(this IRestResponse response)
        {
            return response.StatusCode.IsScuccessStatusCode() &&
                   response.ResponseStatus == ResponseStatus.Completed;
        }

        public static void EnsureResponseWasSuccessful(this IRestClient client, IRestRequest request, IRestResponse response)
        {
            if (response.IsSuccessful())
            {
                return;
            }

            var requestUri = client.BuildUri(request);
            throw RestException.CreateException(requestUri, response);
        }
    }
}
