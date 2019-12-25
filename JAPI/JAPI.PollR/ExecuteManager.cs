using JAPI.Repo;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using JAPI.Repo.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.PollR
{
    /// <summary>
    /// when this server client receives message from signalR polling or execution messages then execute the unit of work
    /// </summary>
    public static class ExecuteManager
    {

        public static JAPISessionRepository _SessionRepository { get; set; }
        public static JAPISessionRepository SessionRepository
        {
            get
            {
                if (_SessionRepository == null)
                    _SessionRepository = RepositoryInjector.GetInjector<JAPISessionRepository>();
                return _SessionRepository;
            }
        }

        public static async Task<string> ExecuteRequest(ReportExecutionResultSet executionSet)
        {
            var requestId = string.Empty;
            var execService = new ReportExecutionService(SessionRepository);

            var reportJob = await execService.ExecuteReportAsync(executionSet.GetDefaultRequestObject());
            return reportJob.requestId;
        }

        public static async Task<KeyValuePair<string, ExecutionStatus>> PollRequest(string requestId)
        {
            //continuously polls until exit condition is met
            var execService = new ReportExecutionService(SessionRepository);
            return await execService.PollReportKVP(requestId);
        }

        public static async Task<ReportExecutionResponse> GetExecutionDetails(KeyValuePair<string, ExecutionStatus> pollResult)
        {
            var execService = new ReportExecutionService(SessionRepository);
            return await execService.GetExecutionDetails(pollResult.Key);
        }
    }

}
