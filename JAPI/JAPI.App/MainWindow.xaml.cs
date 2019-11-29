using JAPI.Repo;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JAPI.App
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly string[] defaultOrgList = { "X2RDATA_DEV", "X2RDATA_QA", "X2RDATA_US_UAT", "X2RDATA_ARINEO_TST" };
        public RangeObservableCollection<Org> organizations { get; set; }
        public RangeObservableCollection<Resource> reportsCollection { get; set; }
        public RangeObservableCollection<Resource> selectedReportsCollection { get; set; }

        private JAPISessionRepository _repositoryInjector;
        public JAPISessionRepository repositoryInjector
        {
            get
            {
                if (_repositoryInjector == null)
                {
                    _repositoryInjector = new JAPISessionRepository(
                        new JClient()
                        {
                            Username = ConfigurationManager.AppSettings["USERNAME"],
                            Password = ConfigurationManager.AppSettings["PASSWORD"],
                            Organization = ConfigurationManager.AppSettings["DEFAULT_ORG"],
                            BaseURL = ConfigurationManager.AppSettings["BASE_SERVER_URL"],
                            Timeout = 30 * 1000
                        });
                }
                return _repositoryInjector;
            }
        }

        public MainWindow()
        {
            InitEmptyCollections();
            InitOrgDataAsync();

            DataContext = this;
            InitializeComponent();
            //reportsSelectionEnable(false);

        }

        #region ASYNC SANITY CHECK
        //private async void InitDoWork()
        //{
        //    var T = await DoWork();
        //    var tt = T;
        //}

        //private Task<int> DoWork()
        //{
        //    return Task.Run(() =>
        //    {
        //        for (int i = 0; i < 10; i++)
        //        {
        //            Thread.Sleep(500);
        //        };
        //        return 500;
        //    });
        //}
        #endregion

        private void InitEmptyCollections()
        {
            organizations = new RangeObservableCollection<Org>() { };
            reportsCollection = new RangeObservableCollection<Resource>() { };
            selectedReportsCollection = new RangeObservableCollection<Resource>() { };
        }

        private void reportsSelectionEnable(bool isEnable)
        {
            lboxAvailableReports.IsEnabled = lboxSelectedReports.IsEnabled =
            btnAvReportsSelectAll.IsEnabled = btnAvReportsSelectNone.IsEnabled =
            btnSelReportsSelectAll.IsEnabled = btnSelReportsSelectNone.IsEnabled =
            btnUnselectItem.IsEnabled = btnSelectItem.IsEnabled = isEnable;
        }

        private async void InitOrgDataAsync()
        {
            var newOrgList = new List<Org>();
            foreach (var orgId in defaultOrgList)
            {
                try
                {
                    //FIX THIS IF YOUF WANT TO CONTINUE
                    var jOrg = await new OrganizationService().GetOrg(orgId);
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
                    DisplayErrorMessage(msg);
                }
            }

            organizations.Clear();
            organizations.AddRange(newOrgList);
        }

        private async Task FetchReportsAsync(string selectedOrgId)
        {
            var reportsList = new List<Resource>();
            var rService = new ResourceService();
            var requestArgs = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("type", "reportUnit"),
                new KeyValuePair<string, string>("folderUri", "/reports"),
            };

            try
            {
                var reportsLookup = await rService.GetResourcesAsync(requestArgs);
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
                DisplayErrorMessage(msg);
            }

            reportsCollection.Clear();
            reportsCollection.AddRange(reportsList);

        }

        private void DisplayErrorMessage(string message)
        {
            lblDisplayError.Content = message;
            lblDisplayError.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LboxAvailableReports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnSelectItem_Click(object sender, RoutedEventArgs e)
        {
            var newSelectedList = new List<Resource>(selectedReportsCollection);
            var newReportsList = new List<Resource>(reportsCollection);

            var selectedItems =
                lboxAvailableReports.SelectedItems.Cast<Resource>()
                .Where(x => !newSelectedList.Any(y => x == y)).ToList();

            newSelectedList.AddRange(selectedItems);

            foreach (var rItem in selectedItems)
                newReportsList.Remove(rItem);

            reportsCollection.Clear();
            selectedReportsCollection.Clear();
            reportsCollection.AddRange(newReportsList);
            selectedReportsCollection.AddRange(newSelectedList);
        }

        private void BtnUnselectItem_Click(object sender, RoutedEventArgs e)
        {
            var newSelectedList = new List<Resource>(selectedReportsCollection);
            var newReportsList = new List<Resource>(reportsCollection);

            var selectedItems =
                lboxSelectedReports.SelectedItems.Cast<Resource>()
                .Where(x => !newReportsList.Any(y => x == y)).ToList();

            newReportsList.AddRange(selectedItems);

            foreach (var rItem in selectedItems)
                newSelectedList.Remove(rItem);

            reportsCollection.Clear();
            selectedReportsCollection.Clear();
            reportsCollection.AddRange(newReportsList);
            selectedReportsCollection.AddRange(newSelectedList);
        }

        private void BtnAvReportsSelectAll_Click(object sender, RoutedEventArgs e)
        {
            lboxAvailableReports.SelectAll();
        }

        private void BtnAvReportsSelectNone_Click(object sender, RoutedEventArgs e)
        {
            lboxAvailableReports.UnselectAll();
        }

        private void BtnSelReportsSelectAll_Click(object sender, RoutedEventArgs e)
        {
            lboxSelectedReports.SelectAll();
        }

        private void BtnSelReportsSelectNone_Click(object sender, RoutedEventArgs e)
        {
            lboxSelectedReports.UnselectAll();
        }

        private async void CbOrg_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedOrg = (Org)e.AddedItems[0] ?? new Org { id = defaultOrgList[0] };

            await FetchReportsAsync(selectedOrg.id);
            reportsSelectionEnable(true);

        }

        private void CbOrg_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }
    }
}