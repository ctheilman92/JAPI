using JAPI.Repo;
using JAPI.Repo.DTO;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using JAPI.Repo.Services;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.PollR
{
    public class PollHub : Hub
    {
        private readonly static ClientRepositoryMapping<JAPISessionRepository> _clientMappings 
            = new ClientRepositoryMapping<JAPISessionRepository>();

        public override Task OnConnected()
        {
            Console.WriteLine($"Welcome new Client!!");
            Console.WriteLine($"ConnectionId : {Context.ConnectionId}");

            //get JAPI session connect it to connectionId
            var repository = RepositoryInjector.GetInjector<JAPISessionRepository>();
            _clientMappings.Add(Context.ConnectionId, repository);
            Console.WriteLine($"JAPI Repository session established: {repository.jClient.JSessionID}");

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _clientMappings.Remove(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public async Task CancelRequestExecution(string requestId)
        {

        }

        public async Task UpdateCallingClient(ReportExecutionResultSet executionSet, bool isException = false)
        {
            Console.WriteLine($"Poll Item Updated : [{executionSet.guid} - {executionSet.resource.label}] : STATUS = [{executionSet.status}]");
            Console.WriteLine($"ConnectionId : {Context.ConnectionId}");

            if (isException)
            {
                Console.WriteLine($"Poll Item Internal ERR : {executionSet.internalError}");
                await Clients.All.ServerRequestFailed(executionSet);
            }
            else
            {
                await Clients.Caller.RequestUpdating(executionSet);
            }
        }

        public async Task BeginRequestExecution(ReportExecutionResultSet executionSet)
        {
            try
            {
                var reportUnit = executionSet.resource;
                if (reportUnit != null)
                {
                    executionSet.successful = false;
                    executionSet.requestId = await ExecuteManager.ExecuteRequest(executionSet);

                    if (string.IsNullOrEmpty(executionSet.requestId))
                        throw new Exception("request failed upstream [Event: ExecuteRequest]");

                    //send client message resultset
                    executionSet.status = "In Progress";
                    await UpdateCallingClient(executionSet);

                    var pollResult = await ExecuteManager.PollRequest(executionSet.requestId);
                    if (pollResult.Key == null)
                        throw new Exception("request failed upstream - unable to poll executing request [Event: PollRequest]");

                    var exportDetails = await ExecuteManager.GetExecutionDetails(pollResult);
                    if (exportDetails == null)
                        throw new Exception("request failed upstream - unable to fetch executing request details [Event: GetExecutionDetails]");

                    executionSet.export = exportDetails.exports.FirstOrDefault();
                    executionSet.status = executionSet.export.status;
                    executionSet.successful = (string.IsNullOrEmpty(executionSet.internalError) && !executionSet.status.Equals("failed", StringComparison.OrdinalIgnoreCase));
                    executionSet.internalError = (executionSet.export.errorDescriptor != null)
                        ? $"[{executionSet.export.errorDescriptor.errorCode}] - {executionSet.export.errorDescriptor.message}"
                        : string.Empty;

                    //send final update to client 
                    await UpdateCallingClient(executionSet);
                }
            }
            catch (Exception ex)
            {
                //send requestFailed message to sender
                executionSet.status = "Internal Failure";
                executionSet.internalError = ex.Message;
                await UpdateCallingClient(executionSet, true);
            }
        }
    }
}
