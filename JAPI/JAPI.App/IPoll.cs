using JAPI.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.App
{
    public interface IPoll
    {
        event Action<ReportExecutionResultSet> RequestUpdated;
        event Action AllRequestsCancelled;
        event Action<ReportExecutionResultSet> RequestCancelled;
        event Action ConnectionReconnecting;
        event Action ConnectionReconnected;
        event Action ConnectionClosed;

        Task ConnectAsync();
        Task SendExecutionRequests(List<ReportExecutionResultSet> executeRequests, int batch = 5);
        Task SendExecutionRequest(ReportExecutionResultSet executeRequest);
        Task SendExecutionSingleRequest(Resource resource);
        Task CancelRequest(ReportExecutionResultSet resultSet);
    }
}
