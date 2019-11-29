using JAPI.Repo.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
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

        #region OLD IMP
        public async Task<IRestResponse> GetJasperResponseAsync(string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams)
        {
            var client = jRepository.GetRestClient(TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams);

            var res = await client.ExecuteTaskAsync(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res;
        }

        public async Task<IRestResponse> PostJasperResponseAsync<T>(T requestObject, string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams)
        {
            var client = jRepository.GetRestClient(TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams, Method.POST, requestObject);

            var res = await client.ExecutePostTaskAsync(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res;
        }

        public async Task<Byte[]> GetJasperContentAsync(string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams)
        {
            var client = jRepository.GetRestClient(TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams);

            var res = await client.ExecuteTaskAsync<Byte[]>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res.RawBytes;
        }

        public async Task<T> GetJasperObjectAsync<T>(string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams)
            where T : new()
        {
            var client = jRepository.GetRestClient(TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams);

            var res = await client.ExecuteTaskAsync<T>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return (T)res.Data;
        }

        public async Task<Byte[]> PostJasperContentAsync<T>(T requestObject, string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams)
        {
            var client = jRepository.GetRestClient(TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams, Method.POST, requestObject);

            var res = await client.ExecuteTaskAsync<Byte[]>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return res.RawBytes;
        }

        public async Task<TOUT> PostJasperObjectAsync<TIN, TOUT>(TIN requestObject = default, string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams)
            where TOUT : new()
        {
            var client = jRepository.GetRestClient(TAG + singleRequestTag);
            var req = jRepository.GetRestRequest(requestParams, Method.POST, requestObject);

            var res = await client.ExecutePostTaskAsync<TOUT>(req);
            client.EnsureResponseWasSuccessful(req, res);

            return (TOUT)res.Data;
        }
        #endregion


    }
}
