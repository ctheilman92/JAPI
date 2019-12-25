using JAPI.Repo;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.App
{

    public class JAPIViewModel
    {
        //public event PropertyChangedEventHandler PropertyChanged;


        public readonly string[] defaultOrgList = { "X2RDATA_DEV", "X2RDATA_QA", "X2RDATA_US_UAT", "X2RDATA_ARINEO_TST" };
        public PollService pollService { get; set; } = new PollService();
        public RangeObservableCollection<Org> organizations { get; set; }
        public RangeObservableCollection<ReportUnit> reportsCollection { get; set; }
        public RangeObservableCollection<ReportUnit> selectedReportsCollection { get; set; }
        public RangeObservableCollection<ReportExecutionResultSet> executeReportsCollection { get; set; }

        public JAPIViewModel()
        {
            organizations = new RangeObservableCollection<Org>() { };
            reportsCollection = new RangeObservableCollection<ReportUnit>() { };
            selectedReportsCollection = new RangeObservableCollection<ReportUnit>() { };
            executeReportsCollection = new RangeObservableCollection<ReportExecutionResultSet>() { };

            pollService.RequestUpdated += UpdateExecutionSet;
            pollService.RequestCancelled += ExecutionCancelled;
            pollService.AllRequestsCancelled += AllExecutionsCancelled;
            Task.Factory.StartNew(async() => { await pollService.ConnectAsync(); });
            InitOrgDataAsync();
        }


        #region MAIN METHODS
        public async void InitOrgDataAsync()
        {
            var newOrgList = new List<Org>();
            var orgService = new OrganizationService(RepositoryInjector.GetInjector<JAPISessionRepository>());

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

        private void UpdateExecutionSet(ReportExecutionResultSet resultSet)
        {
            var item = executeReportsCollection.FirstOrDefault(r => r.guid == resultSet.guid);
            if (item != null)
            {
                item = resultSet;
            }
        }

        private void AllExecutionsCancelled()
        {
            foreach (var reportUnit in executeReportsCollection)
            {
                reportUnit.status = "Cancelled";
            }
        }

        private void ExecutionCancelled(ReportExecutionResultSet resultSet)
        {
            var item = executeReportsCollection.FirstOrDefault(r => r.guid == resultSet.guid);
            if (item != null)
            {
                item.status = "Cancelled";
            }
        }

        #endregion
    }
}
