using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo
{

    public class OrganizationService : JAPIService
    {

        public OrganizationService(IJAPIAuthenticateRepository repository = null) : base(repository)
        {
            TAG = "/organizations";
        }


        public async Task<List<Org>> SearchOrgs(Dictionary<RequestParamKey, string> requestParams = null)
        {
            var orgs = await GetJasperObjectAsync<List<Org>>(string.Empty, requestParams);
            return orgs;
        }

        public async Task<Org> GetOrg(string orgId)
        {
            if (!orgId.StartsWith("/"))
                orgId = "/" + orgId;

            var org = await GetJasperObjectAsync<Org>(orgId);
            return org;
        }

        public async Task<Org> GetClientOrg()
        {
            var _org = jRepository.GetJClient().Organization;
            if (!_org.StartsWith("/"))
                _org = "/" + _org;

            var org = await GetJasperObjectAsync<Org>(_org);
            return org;
        }
    }
}
