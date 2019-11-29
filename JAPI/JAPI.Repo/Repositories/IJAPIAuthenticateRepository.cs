using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JAPI.Repo.Repositories
{
    public interface IJAPIAuthenticateRepository
    {
        RestClient GetRestClient(Uri uri, HttpBasicAuthenticator authenticator = null);
        RestClient GetRestClient(string url, HttpBasicAuthenticator authenticator = null);
        RestClient GetRestClient(string url);
        RestClient GetRestClient(string url, string username = null, string password = null, string organization = null);
        RestRequest GetRestRequest(KeyValuePair<string, string>[] requestParams, Method method = Method.GET, object requestObject = null);
        JClient GetJClient();
    }

    public abstract class JAPIRepositoryBase
    {
        public JClient jClient { get; set; }

        public JAPIRepositoryBase(JClient client) => jClient = client;
    }
}
