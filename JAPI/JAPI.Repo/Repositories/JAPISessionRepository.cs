using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JAPI.Repo.Repositories
{
    /// <summary>
    /// The session authentication method is used best with a singleton class of JClient. Thereby using the same session cookie for all requests.
    /// </summary>
    public class JAPISessionRepository : JAPIRepositoryBase, IJAPIAuthenticateRepository
    {
        #region DI CONSTRUCTOR
        public JAPISessionRepository(JClient client) : base(client) { }
        public JAPISessionRepository(IJAPIAuthenticateRepository iRepository) : base(iRepository.GetJClient()) { }
        public JAPISessionRepository(JAPIRepositoryBase repositoryBase) : base(repositoryBase.jClient) { }
        #endregion

        #region SESSION SPECIFIC AUTH
        private const string AUTHENTICATE_TAG = "/login";

        private RestClient GetAuthenticateRestClient()
            => new RestClient(new Uri($"{jClient.BaseURL}{AUTHENTICATE_TAG}"));

        private RestResponseCookie GetSessionCookie()
        {
            var rClient = GetAuthenticateRestClient();
            var authParams = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("j_username", jClient.Username),
                new KeyValuePair<string, string>("j_password", jClient.Password),
            };

            var req = GetRestRequest(authParams, Method.POST);
            var res = rClient.Execute(req);
            rClient.EnsureResponseWasSuccessful(req, res);

            var sessionCookie = res.Cookies
                .FirstOrDefault(x => x.Name.Equals("JSESSIONID", StringComparison.OrdinalIgnoreCase));

            return sessionCookie;
        }
        #endregion

        /// <summary>
        /// Session Implementation attempts to create jasper session
        /// <see cref="GetSessionCookie(JClient)"/>
        /// </summary>
        /// <returns></returns>
        public JClient GetJClient()
        {
            if (jClient == null)
                jClient = new JClient();

            jClient.AuthType = JAuthtype.Session;

            if (jClient.SessionCookie.Expired || string.IsNullOrEmpty(jClient.JSessionID))
                jClient.SessionCookie = GetSessionCookie();

            return jClient;
        }

        public RestClient GetRestClient(Uri uri, HttpBasicAuthenticator authenticator = null)
            => new RestClient(uri);

        public RestClient GetRestClient(string url, HttpBasicAuthenticator authenticator = null)
            => new RestClient(new Uri(url));

        public RestClient GetRestClient(string url)
            => new RestClient(new Uri(url));

        public RestClient GetRestClient(string url, string username = null, string password = null, string organization = null)
            => new RestClient(new Uri(url));

        public RestRequest GetRestRequest(KeyValuePair<string, string>[] requestParams, Method method = Method.GET, object requestObject = null)
        {
            var req = new RestRequest
            {
                Timeout = jClient.Timeout,
                Method = method
            };

            if (jClient.SessionCookie != null && !string.IsNullOrEmpty(jClient.SessionCookie.Value))
                req.AddParameter("JSESSIONID", jClient.SessionCookie.Value, ParameterType.Cookie);

            if (requestObject != null && method == Method.POST)
            {
                var jsonObject = JsonConvert.SerializeObject(requestObject);
                req.AddParameter("application/json", jsonObject, ParameterType.RequestBody);
            }

            if (requestParams.Length > 0)
            {
                foreach (var param in requestParams)
                {
                    if (!string.IsNullOrEmpty(param.Value))
                        req.AddParameter(param.Key, param.Value, ParameterType.QueryString);
                }
            }

            return req;
        }
    }

}
