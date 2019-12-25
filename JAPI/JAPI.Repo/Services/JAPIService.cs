using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace JAPI.Repo
{


    /// <summary>
    /// Injected Dependency Repository should NEVER be empty, so neither should the constructor or exceptions are guarenteed.
    /// <see cref="IJAPIAuthenticateRepository"/>
    /// <seealso cref="JAPIRepositoryBase"/>
    /// </summary>
    public class JAPIService : IRESTRepository
    {
        private Dictionary<RequestParamKey, string> _requestParams { get; set; }

        public Dictionary<RequestParamKey, string> requestParams { get { return _requestParams; } set { _requestParams = value; } }

        public IJAPIAuthenticateRepository jRepository { get; set; }

        #region DI Constructor
        public JAPIService(IJAPIAuthenticateRepository repository) 
            => jRepository = repository ?? jRepository;
        #endregion

        public string TAG { get; set; } = "/serverInfo";

        public async Task<Server> GetInfo()
        {
            var server = await GetJasperObjectAsync<Server>();
            return server;
        }

        public void AddUpdateRequestParams(RequestParamKey paramKey, string value)
        {
            if (requestParams.ContainsKey(paramKey))
                requestParams[paramKey] = value;
            else
                requestParams.Add(paramKey, value);
        }

        private string GetBaseURL() 
            => jRepository.GetJClient().BaseURL;
        #region OLD IMP
        public async Task<IRestResponse> GetJasperResponseAsync(string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null)
        {
            var baseUrl = GetBaseURL();
            var client = jRepository.GetRestClient(GetBaseURL() + TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams);

            var res = await client.ExecuteTaskAsync(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res;
        }

        public async Task<IRestResponse> PostJasperResponseAsync<T>(T requestObject, string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null)
        {
            var client = jRepository.GetRestClient(GetBaseURL() + TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams, Method.POST, requestObject);

            var res = await client.ExecutePostTaskAsync(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res;
        }

        public async Task<Byte[]> GetJasperContentAsync(string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null)
        {
            var client = jRepository.GetRestClient(GetBaseURL() + TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams);

            var res = await client.ExecuteTaskAsync<Byte[]>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res.RawBytes;
        }

        public async Task<T> GetJasperObjectAsync<T>(string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null)
            where T : new()
        {
            var client = jRepository.GetRestClient(GetBaseURL() + TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams);

            var res = await client.ExecuteTaskAsync<T>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return (T)res.Data;
        }

        public async Task<Byte[]> PostJasperContentAsync<T>(T requestObject, string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null)
        {
            var client = jRepository.GetRestClient(GetBaseURL() + TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams, Method.POST, requestObject);

            var res = await client.ExecuteTaskAsync<Byte[]>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res.RawBytes;
        }

        public async Task<TOUT> PostJasperObjectAsync<TIN, TOUT>(TIN requestObject = default, string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null)
            where TOUT : new()
        {
            var client = jRepository.GetRestClient(GetBaseURL() + TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams, Method.POST, requestObject);

            var res = await client.ExecutePostTaskAsync<TOUT>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return (TOUT)res.Data;
        }
        #endregion


    }
}
