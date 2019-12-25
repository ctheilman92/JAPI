using System;
using System.Threading.Tasks;
using JAPI.Repo;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JAPI.UnitTests
{

    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public async Task TestReportExecutionService()
        {
            var sessionService = 
                new JAPIService(RepositoryInjector.GetInjector<JAPISessionRepository>());
            
        }

        public async Task TestReportDetails()
        {
            var reportService = 
                new ReportService(RepositoryInjector.GetInjector<JAPIBasicAuthRepository>());

        }
    }
}
