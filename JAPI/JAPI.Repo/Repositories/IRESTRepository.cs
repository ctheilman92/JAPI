using JAPI.Repo.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo.Repositories
{ 
    public interface IRESTRepository
    {
        Task<IRestResponse> GetJasperResponseAsync(string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null);
        Task<IRestResponse> PostJasperResponseAsync<T>(T requestObject, string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null);
        Task<Byte[]> GetJasperContentAsync(string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null);
        Task<T> GetJasperObjectAsync<T>(string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null) where T : new();
        Task<Byte[]> PostJasperContentAsync<T>(T requestObject, string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null);
        Task<TOUT> PostJasperObjectAsync<TIN, TOUT>(TIN requestObject = default, string singleRequestTag = "", Dictionary<RequestParamKey, string> requestParams = null) where TOUT : new();
    }
}
