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
        Task<IRestResponse> GetJasperResponseAsync(string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams);
        Task<IRestResponse> PostJasperResponseAsync<T>(T requestObject, string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams);
        Task<Byte[]> GetJasperContentAsync(string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams);
        Task<T> GetJasperObjectAsync<T>(string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams) where T : new();
        Task<Byte[]> PostJasperContentAsync<T>(T requestObject, string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams);
        Task<TOUT> PostJasperObjectAsync<TIN, TOUT>(TIN requestObject = default, string singleRequestTag = "", params KeyValuePair<string, string>[] requestParams) where TOUT : new();
    }
}
