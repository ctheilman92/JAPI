using JAPI.Repo.Extensions;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JAPI.Repo.Repositories
{
    /// <summary>
    /// Basic authentication is per single request and does not maintain a session. 
    /// JClient 
    /// For quick queries regarding particular organization resources this will suffice however, issues arise when running persistant executions like reportExecutions Service operations.
    /// </summary>
    public class JAPIBasicAuthRepository : JAPIRepositoryBase, IJAPIAuthenticateRepository
    {
        #region DI CONSTRUCTOR
        public JAPIBasicAuthRepository(JClient client) : base(client) { }
        public JAPIBasicAuthRepository(IJAPIAuthenticateRepository iRepository) : base(iRepository.GetJClient()) { }
        public JAPIBasicAuthRepository(JAPIRepositoryBase repositoryBase = null) : base(repositoryBase.jClient) { }
        #endregion

        public JClient GetJClient()
        {
            if (jClient == null)
                jClient = new JClient();

            jClient.AuthType = JAuthtype.BasicAuthentication;
            return jClient;
        }

        public RestClient GetRestClient(Uri uri, HttpBasicAuthenticator authenticator = null)
            => new RestClient()
            {
                Authenticator = authenticator,
                BaseUrl = uri
            };

        public RestClient GetRestClient(string url, HttpBasicAuthenticator authenticator = null)
            => new RestClient()
            {
                Authenticator = authenticator,
                BaseUrl = new Uri(url)
            };

        public RestClient GetRestClient(string url) => (jClient == null)
            ? new RestClient()
            : new RestClient()
            {
                Authenticator = (string.IsNullOrEmpty(jClient.Organization))
                            ? new HttpBasicAuthenticator(jClient.Username, jClient.Password)
                            : new HttpBasicAuthenticator($"{jClient.Username}|{jClient.Organization}", jClient.Password),
                BaseUrl = new Uri(url)
            };

        public RestClient GetRestClient(string url, string username = null, string password = null, string organization = null)
            => new RestClient()
            {
                Authenticator = (string.IsNullOrEmpty(organization))
                        ? new HttpBasicAuthenticator(username, password)
                        : new HttpBasicAuthenticator($"{username}|{organization}", password),
                BaseUrl = new Uri(url)
            };

        public RestRequest GetRestRequest(Dictionary<RequestParamKey, string> requestParams, Method method = Method.GET, object requestObject = null)
        {
            var req = new RestRequest()
            {
                Timeout = jClient.Timeout,
                Method = method
            };

            if (requestObject != null && method == Method.POST)
            {
                var jsonObject = JsonConvert.SerializeObject(requestObject);
                req.AddParameter("application/json", jsonObject, ParameterType.RequestBody);
            }

            if (requestParams != null)
            {
                foreach (var param in requestParams)
                {
                    if (!string.IsNullOrEmpty(param.Value))
                        req.AddParameter(param.Key.GetEnumDescriptor(), param.Value);
                }
            }
            return req;
        }

    }
}
