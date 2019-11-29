using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace JAPI.Repo
{
    public class JobService : JAPIService
    {
        public JobService(IJAPIAuthenticateRepository repository = null) : base(repository)
        {
            TAG = "/Jobs";
        }
    }
}
