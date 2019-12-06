using JAPI.Repo.DTO;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo.Services
{
    public class UserService : JAPIService
    {
        public UserService(IJAPIAuthenticateRepository repository)
            : base(repository) { TAG = "/users"; }


        public async Task<List<User>> SearchUsers(Dictionary<RequestParamKey, string> requestParams = null)
        {
            throw new NotImplementedException();
        }
    }
}
