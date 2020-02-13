using JAPI.App.Extensions;
using JAPI.Repo;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace JAPI.App
{

    public class JAPIViewModel
    {

        public PollService pollService { get; set; } = new PollService();
        public AsyncRangeObservableCollection<Org> organizations { get; set; }
        public AsyncRangeObservableCollection<ReportUnit> reportsCollection { get; set; }
        public AsyncRangeObservableCollection<ReportUnit> selectedReportsCollection { get; set; }
        public AsyncPropertyTrackingObservableCollection<ReportExecutionWPFRecord> executeReportsCollection { get; set; }
        public JAPISessionRepository _SessionRepository { get; set; }
        public JAPISessionRepository SessionRepository
        {
            get
            {
                if (_SessionRepository == null)
                    _SessionRepository = RepositoryInjector.GetInjector<JAPISessionRepository>();
                return _SessionRepository;
            }
        }



        public readonly string[] defaultOrgList = { "X2RDATA_DEV", "X2RDATA_QA", "X2RDATA_US_UAT", "X2RDATA_ARINEO_TST" };

        public JAPIViewModel()
        {
            organizations = new AsyncRangeObservableCollection<Org>() { };
            reportsCollection = new AsyncRangeObservableCollection<ReportUnit>() { };
            selectedReportsCollection = new AsyncRangeObservableCollection<ReportUnit>() { };
            executeReportsCollection = new AsyncPropertyTrackingObservableCollection<ReportExecutionWPFRecord>() { };

            pollService.RequestUpdated += UpdateExecutionSet;
            pollService.RequestCancelled += ExecutionCancelled;
            pollService.AllRequestsCancelled += AllExecutionsCancelled;

            Task.Factory.StartNew(async() => { await pollService.ConnectAsync(); });
            Task.Factory.StartNew(async () => { await InitOrgDataAsync(); });
        }


        #region MAIN METHODS
        public async Task InitOrgDataAsync()
        {
            var newOrgList = new List<Org>();
            var orgService = new OrganizationService(SessionRepository);

            //for now test on x2r_data_dev
            var orgId = defaultOrgList[0];
            try
            {
                //FIX THIS IF YOUF WANT TO CONTINUE
                var jOrg = await orgService.GetOrg(orgId);
                newOrgList.Add(jOrg);

            }
            catch (Exception ex)
            {
                var msg = $"exception Occurred - attempting to load default list";
                if (ex is RestException)
                {
                    var restex = (RestException)ex;
                    if (restex.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized || restex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        msg = $"REST Operations error occurred with status code {restex.HttpStatusCode.ToString()}. Message: {restex.Message} \n....attempting to load defaults";
                        newOrgList.Add(
                            new Org
                            {
                                id = orgId,
                                alias = orgId,
                                tenantName = orgId
                            });
                    }
                }
            }

            organizations.Clear();
            organizations.AddRange(newOrgList);
        }

        public async Task FetchReportsAsync(string selectedOrgId)
        {
            var reportsList = new List<ReportUnit>();
            var rService = new ResourceService(RepositoryInjector.GetInjector<JAPISessionRepository>());
            var requestParams = new Dictionary<RequestParamKey, string>()
            {
                { RequestParamKey.ResourceType, "reportUnit" },
                { RequestParamKey.FolderURI, "/reports" },
                { RequestParamKey.Limit, "0" }
            };

            try
            {
                var reportsLookup = await rService.GetResourcesAsync<ReportUnit>(requestParams);
                reportsList = reportsLookup.resourceLookup;
            }
            catch (Exception ex)
            {
                var msg = $"exception Occurred - message {ex.Message}";
                if (ex is RestException restex)
                {
                    if (restex.HttpStatusCode == System.Net.HttpStatusCode.Unauthorized || restex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        msg = $"REST Operations error occurred with status code {restex.HttpStatusCode.ToString()} - attempting to load defaults";
                    }
                }
            }

            reportsCollection.Clear();
            reportsCollection.AddRange(reportsList);

        }
        #endregion

        #region SIGNALR CLIENT CALLBACKS

        private void UpdateExecutionSet(ReportExecutionResultSet updatedResultSet)
        {
            var item = executeReportsCollection.FirstOrDefault(r => r.resultSet.guid == updatedResultSet.guid);
            if (item != null)
            {
                var indexOf = executeReportsCollection.IndexOf(item);
                Uri srcUri = null;
                if (updatedResultSet.successful)
                {
                    srcUri = new Uri(@"C:\Users\cameron.heilman\Documents\WR\GIT_JAPI\japi\JAPI\JAPI.App\Resources\check-mark-11-32.png");
                    item.rowStatus = WPFRecrodStatus.Success;
                }
                else
                {
                    srcUri = (string.IsNullOrEmpty(updatedResultSet.internalError) && !updatedResultSet.status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                        ? new Uri(@"C:\Users\cameron.heilman\Documents\WR\GIT_JAPI\japi\JAPI\JAPI.App\Resources\clock-8-32.png")
                        : new Uri(@"C:\Users\cameron.heilman\Documents\WR\GIT_JAPI\japi\JAPI\JAPI.App\Resources\x-mark-3-32.png");

                    item.rowStatus = (string.IsNullOrEmpty(updatedResultSet.internalError) && !updatedResultSet.status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                        ? WPFRecrodStatus.InProgress
                        : WPFRecrodStatus.Failed;
                }

                item.Image = new BitmapImage(srcUri);
                item.resultSet = updatedResultSet;
                executeReportsCollection[indexOf] = item;

                //var nextCount = pollService.pollingSlots - executeReportsCollection.Count(x => x.rowStatus == WPFRecrodStatus.InProgress);
                //if (nextCount > 0 && executeReportsCollection.Count(x => x.rowStatus == WPFRecrodStatus.Waiting) > 0)
                //{
                //    Task.Factory.StartNew(async () => 
                //    {
                //        await pollService.SendExecutionRequests(executeReportsCollection.Where(x => x.rowStatus == WPFRecrodStatus.Waiting), nextCount);
                //    });
                //}
            }
        }

        private void AllExecutionsCancelled()
        {
            //var newReportList = new List<ReportExecutionResultSet>();
            //foreach (var reportUnit in executeReportsCollection)
            //{
            //    reportUnit.resultSet.status = "Cancelled";
            //    newReportList.Add(reportUnit.resultSet);
            //}

            //executeReportsCollection.Clear();
            //executeReportsCollection.AddRange(newReportList);
        }

        private void ExecutionCancelled(ReportExecutionResultSet cancelResultSet)
        {
            var item = executeReportsCollection.FirstOrDefault(r => r.resultSet.guid == cancelResultSet.guid);
            cancelResultSet.status = "Cancelled";
            if (item != null)
            {
                var i = executeReportsCollection.IndexOf(item);
                executeReportsCollection[i].resultSet = cancelResultSet;
            }
        }

        #endregion
    }
}
