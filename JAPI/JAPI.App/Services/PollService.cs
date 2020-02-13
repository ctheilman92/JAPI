using JAPI.App.Extensions;
using JAPI.Repo;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JAPI.App
{
    public class PollService
    {
        public event Action<ReportExecutionResultSet> RequestUpdated;
        public event Action<ReportExecutionResultSet> RequestCancelled;
        public event Action AllRequestsCancelled;

        public event Action ConnectionReconnecting;
        public event Action ConnectionReconnected;
        public event Action ConnectionClosed;

        public int pollingSlots = 5;
        public IHubProxy hubProxy;
        private HubConnection hubConnection;
        private string url = "http://localhost:4444/jpollr";

        public async Task ConnectAsync() 
        {
            hubConnection = new HubConnection(url);
            hubProxy = hubConnection.CreateHubProxy("pollHub");

            //client event handlers
            hubProxy.On<ReportExecutionResultSet>("RequestUpdating", (resultSet) => 
                RequestUpdated?.Invoke(resultSet));
            hubProxy.On<ReportExecutionResultSet>("requestCancelling", (resultSet) => RequestCancelled?.Invoke(resultSet));
            hubProxy.On("requestCancelled", () => AllRequestsCancelled?.Invoke());

            hubConnection.Reconnecting += Reconnecting;
            hubConnection.Reconnected += Reconnected;
            hubConnection.Closed += ConnectionClosed;
            await hubConnection.Start();
        }

        #region SIGNALR HUB INVOCATIONS
        public async Task SendExecutionRequests(IEnumerable<ReportExecutionWPFRecord> executeRequests, int batch = 5)
        {
            foreach (var request in executeRequests)
            {
                await SendExecutionRequest(request.resultSet);
            }
        }

        public Task SendExecutionRequest(ReportExecutionResultSet executeRequest)
        {
            return hubProxy.Invoke("BeginRequestExecution", executeRequest);
        }

        public Task CancelRequest(ReportExecutionResultSet executeRequest)
        {
            return hubProxy.Invoke("CancelRequestExecution", executeRequest);
        }

        private void Disconnected()
        {
            ConnectionClosed?.Invoke();
        }

        private void Reconnected()
        {
            ConnectionReconnected?.Invoke();
        }

        private void Reconnecting()
        {
            ConnectionReconnecting?.Invoke();
        }

        #endregion
    }
}
