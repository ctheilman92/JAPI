using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JAPI.Repo
{

    public class ResourceService : JAPIService
    {

        public ResourceService(IJAPIAuthenticateRepository repository = null) 
            : base(repository) { TAG = "/resources"; }

        public async Task<ResourceLookup<Resource>> GetResourcesAsync(Dictionary<RequestParamKey, string> requestParams = null)  
            =>  await GetJasperObjectAsync<ResourceLookup<Resource>>(requestParams: requestParams);

        public async Task<ResourceLookup<T>> GetResourcesAsync<T>(Dictionary<RequestParamKey, string> requestParams = null) where T : Resource
            => await GetJasperObjectAsync<ResourceLookup<T>>(requestParams: requestParams);
    }
}
