using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using JAPI.Repo;
using JAPI.Repo.Repositories;
using JAPI.Repo.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JAPI.UnitTests
{


    [TestClass]
    public class UnitTest1
    {
        private JAPIBasicAuthRepository _repositoryInjector  { get;set;}
        public JAPIBasicAuthRepository repositoryInjector
        {
            get
            {
                if (_repositoryInjector == null)
                {
                    _repositoryInjector = new JAPIBasicAuthRepository(Utility.GetJClientINI());
                }
                return _repositoryInjector;
            }
        }

        [TestMethod]
        public async Task GetServerInfoAsync()
        {
            var japiService = new JAPIService(repositoryInjector);
            var serverInfo = await japiService.GetInfo();

            if (serverInfo.version == null)
                Debug.Assert(false);
            else
                Debug.Assert(true);
        }

        [TestMethod]
        public async Task GetOrganization()
        {
            var orgName = "X2RDATA_DEV";
            var orgService = new OrganizationService(repositoryInjector);
            var org = await orgService.GetOrg(orgName);

            if (string.IsNullOrEmpty(org.id))
                Debug.Assert(false);
            else
                Debug.Assert(true);
        }

        [TestMethod]
        public async Task SearchOrganizations()
        {
            var paramList = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("q", "X2RDATA_DEV"),
                new KeyValuePair<string, string>("includeParents", ""),
                new KeyValuePair<string, string>("rootTenantId", ""),
                new KeyValuePair<string, string>("offset", ""),
                new KeyValuePair<string, string>("limit", ""),
                new KeyValuePair<string, string>("maxdepth", ""),
            };

            var orgService = new OrganizationService(repositoryInjector);
            var orgList = await orgService.SearchOrgs(paramList);

            if (orgList.Count > 0)
                Debug.Assert(true);
            else
                Debug.Assert(false);
        }

        [TestMethod]
        public async Task RunSingleReport()
        {
            var paramList = new KeyValuePair<string, string>[]
            {
                //new KeyValuePair<string, string>("page", "1")
            };
            var reportService = new ReportService(repositoryInjector);
            var reportPath = "/reports/AP_Invoice_Report.html";
            var downloadPath = System.AppContext.BaseDirectory + "report.html";

            if (System.IO.File.Exists(downloadPath))
                System.IO.File.Delete(downloadPath);

            var rawBytes = await reportService.RunReportAsync(reportPath, paramList);

            System.IO.File.WriteAllBytes(downloadPath, rawBytes);
            if ((System.IO.File.Exists(downloadPath)) && (System.IO.File.ReadAllBytes(downloadPath).Length > 0))
                Debug.Assert(true);
            else
                Debug.Assert(false);
        }

        [TestMethod]
        public async Task ExecuteReportReturnDetails()
        {
            var service = new ReportExecutionService(repositoryInjector);
            var reportPath = "/reports/X2R_User_Report";
            try
            {
                var requestObject = new ReportExecutionRequest
                {
                    reportUnitUri = reportPath,
                    async = true,
                    outputFormat = "pdf",
                };

                var reportJob = await service.ExecuteReportAsync(requestObject);
                if (reportJob != null && !string.IsNullOrEmpty(reportJob.requestId))
                {
                    if (!service.cancelStatuses.Contains(reportJob.status))
                    {
                        Task<ExecutionStatus> pollTask = service.PollReport(reportJob.requestId);
                        pollTask.Wait();
                    }

                    var jobDetails = await service.GetExecutionDetails(reportJob.exports[0].id);


                    Debug.Assert(true);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }

        [TestMethod]
        public async Task GetExecutedReportContentShouldNotHappenThisWay()
        {
            // f7bebd78-965c-4e08-ac27-6ade43f75163/exports/383b0c75-551d-43b0-9736-06af2fa4610d/outputResource
            var service = new ReportExecutionService(repositoryInjector);
            var reportPath = "/reports/X2R_User_Report";
            var reportName = "X2R_User_Report.pdf";

            var requestObject = new ReportExecutionRequest
            {
                reportUnitUri = reportPath,
                async = false,
                outputFormat = "pdf",
            };

            try
            {
                Task<ReportExecutionResponse> execTask = service.ExecuteReportAsync(requestObject);
                execTask.Wait();

                var execResponse = execTask.Result;
                if (!string.IsNullOrEmpty(execResponse.requestId) && execResponse.status.Equals("ready", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var export in execResponse.exports)
                    {
                        var downloadPath = System.AppContext.BaseDirectory + reportName + ".pdf";

                        if (System.IO.File.Exists(downloadPath))
                            System.IO.File.Delete(downloadPath);

                        var rawBytes = await service.GetExecutionOuput(execResponse.requestId, export.id);
                        System.IO.File.WriteAllBytes(downloadPath, rawBytes);
                    }
                }

                Debug.Assert(true);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod]
        public async Task GetReportUnits()
        {
            var requestParams = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("type", "reportUnit"),
                new KeyValuePair<string, string>("folderUri", "/reports"),
            };

            try
            {
                var service = new ResourceService(repositoryInjector);
                var reportUnits = await service.GetResourcesAsync(requestParams);

                Debug.Assert(true);

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }

        }
    }
}
