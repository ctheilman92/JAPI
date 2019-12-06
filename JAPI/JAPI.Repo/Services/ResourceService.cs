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

        public Dictionary<RequestParamKey, string> requestParamDictionary { get; set; }


        public ResourceService(IJAPIAuthenticateRepository repository = null) 
            : base(repository) { TAG = "/resources"; }

        public async Task<ResourceLookup<Resource>> GetResourcesAsync(Dictionary<RequestParamKey, string> requestParams = null)  
            =>  await GetJasperObjectAsync<ResourceLookup<Resource>>(requestParams: requestParamDictionary);
    }
}
