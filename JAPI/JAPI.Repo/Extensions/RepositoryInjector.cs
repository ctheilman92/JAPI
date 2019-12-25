using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo.Extensions
{
    public static class RepositoryInjector
    {
        private static JClient _jClient
        {
            get
            {
                return Utility.GetJClientINI();
            }
        }

        public static T GetInjector<T>(JClient jclient) where T : JAPIRepositoryBase
            => (T)Activator.CreateInstance(typeof(T), new object[] { jclient });

        public static T GetInjector<T>() where T : JAPIRepositoryBase
            => (T)Activator.CreateInstance(typeof(T), new object[] { _jClient });
    }
}
