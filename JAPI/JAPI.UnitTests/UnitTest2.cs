using System;
using System.Threading.Tasks;
using JAPI.Repo;
using JAPI.Repo.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JAPI.UnitTests
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


    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public async Task TestSessionTypeJAPI()
        {
            var sessionService = 
                new JAPIService(RepositoryInjector.GetInjector<JAPISessionRepository>());
            
        }

        public async Task TestBasicAuthTypeJAPI()
        {
            var basicAuthService = 
                new JAPIService(RepositoryInjector.GetInjector<JAPIBasicAuthRepository>());
        }
    }
}
