using JAPI.Repo.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JAPI.Repo
{
    public class ResourceService : JAPIService
    {
        public ResourceService(IJAPIAuthenticateRepository repository = null) 
            : base(repository) { TAG = "/resources"; }

        public async Task<ResourceLookup<Resource>> GetResourcesAsync(params KeyValuePair<string, string>[] requestParams)
            =>  await GetJasperObjectAsync<ResourceLookup<Resource>>(requestParams: requestParams);
    }
}
