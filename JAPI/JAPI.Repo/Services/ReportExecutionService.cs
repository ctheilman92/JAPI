using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JAPI.Repo.Services
{
    public class ReportExecutionService : JAPIService
    {
        public int pollDelay { get; set; } = 3 * 1000;
        public int failuresToCancel { get; set; } = 50;
        private CancellationTokenSource cts { get; set; }
        public List<string> cancelStatuses { get; set; } = new List<string> { "ready", "failed" };

        #region CONSTRUCTORS
        public ReportExecutionService() : base(null)
        {
            TAG = "/reportExecutions";
        }

        public ReportExecutionService(IJAPIAuthenticateRepository repository) : base(repository)
        {
            TAG = "/reportExecutions";
        }

        #endregion

        public async Task<ReportExecutionResponse> ExecuteReportAsync(ReportExecutionRequest requestObject)
        {
            var response = await PostJasperObjectAsync<ReportExecutionRequest, ReportExecutionResponse>(requestObject);
            return response;
        }

        public async Task<ExecutionStatus> PollReport(string requestId)
        {
            cts = new CancellationTokenSource();
            var status = new ExecutionStatus();

            var tries = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                status = await GetJasperObjectAsync<ExecutionStatus>($"/{requestId}/status/");
                tries++;

                if (cancelStatuses.Contains(status.value) || tries >= failuresToCancel)
                    cts.Cancel();

                await Task.Delay(pollDelay, cts.Token);

            }

            return status;
        }

        public async Task<ReportExecutionResponse> GetExecutionDetails(string requestId)
            => await GetJasperObjectAsync<ReportExecutionResponse>($"/{requestId}");

        public async Task<Byte[]> GetExecutionOuput(string requestId, string exportId)
        {
            var res = await GetJasperResponseAsync($"/{requestId}/exports/{exportId}/outputResource");
            return res.RawBytes;
        }
    }
}
