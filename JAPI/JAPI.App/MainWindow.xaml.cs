using JAPI.Repo;
using JAPI.Repo.DTO;
using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using JAPI.Repo.Services;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace JAPI.App
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public JAPIViewModel ViewModel { get; set; }

        public MainWindow()
        {
            try
            {
                ViewModel = new JAPIViewModel();
                DataContext = ViewModel;
                InitializeComponent();
                reportsSelectionEnable(false);
            }
            catch (Exception ex)
            {
                DisplayErrorMessage(ex.Message);
            }

        }

        private void reportsSelectionEnable(bool isEnable)
        {
            lboxAvailableReports.IsEnabled = lboxSelectedReports.IsEnabled =
            btnAvReportsSelectAll.IsEnabled = btnAvReportsSelectNone.IsEnabled =
            btnSelReportsSelectAll.IsEnabled = btnSelReportsSelectNone.IsEnabled =
            btnUnselectItem.IsEnabled = btnSelectItem.IsEnabled = isEnable;
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
            var newSelectedList = new List<ReportUnit>(ViewModel.selectedReportsCollection);
            var newReportsList = new List<ReportUnit>(ViewModel.reportsCollection);

            var selectedItems =
                lboxAvailableReports.SelectedItems.Cast<ReportUnit>()
                .Where(x => !newSelectedList.Any(y => x == y)).ToList();

            newSelectedList.AddRange(selectedItems);

            foreach (var rItem in selectedItems)
                newReportsList.Remove(rItem);

            ViewModel.reportsCollection.Clear();
            ViewModel.selectedReportsCollection.Clear();
            ViewModel.reportsCollection.AddRange(newReportsList);
            ViewModel.selectedReportsCollection.AddRange(newSelectedList);

            btnExecuteSelected.IsEnabled = (ViewModel.selectedReportsCollection.Count > 0);
        }

        private void BtnUnselectItem_Click(object sender, RoutedEventArgs e)
        {
            var newSelectedList = new List<ReportUnit>(ViewModel.selectedReportsCollection);
            var newReportsList = new List<ReportUnit>(ViewModel.reportsCollection);

            var selectedItems =
                lboxSelectedReports.SelectedItems.Cast<ReportUnit>()
                .Where(x => !newReportsList.Any(y => x == y)).ToList();

            newReportsList.AddRange(selectedItems);

            foreach (var rItem in selectedItems)
                newSelectedList.Remove(rItem);

            ViewModel.reportsCollection.Clear();
            ViewModel.selectedReportsCollection.Clear();
            ViewModel.reportsCollection.AddRange(newReportsList);
            ViewModel.selectedReportsCollection.AddRange(newSelectedList);

            btnExecuteSelected.IsEnabled = (ViewModel.selectedReportsCollection.Count > 0);
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
            var selectedOrg = (Org)e.AddedItems[0] ?? new Org { id = ViewModel.defaultOrgList[0] };

            await ViewModel.FetchReportsAsync(selectedOrg.id);
            reportsSelectionEnable(true);
        }

        private void CbOrg_SourceUpdated(object sender, DataTransferEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnExecuteSelected_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.executeReportsCollection.Clear();
            var execList = new List<ReportExecutionResultSet>();

            if (ViewModel.selectedReportsCollection.Count > 0)
            {
                var execService = new ReportExecutionService(RepositoryInjector.GetInjector<JAPISessionRepository>());
                execList = ViewModel.selectedReportsCollection.Select(r =>
                    new ReportExecutionResultSet
                    {
                        guid = Guid.NewGuid(),
                        resource = r,
                        status = "waiting"
                    }).ToList();

                ViewModel.executeReportsCollection.AddRange(execList);
                Task.Factory.StartNew(async() => { await ViewModel.pollService.SendExecutionRequests(execList); });
            }

            tabResults.IsEnabled = (ViewModel.executeReportsCollection.Count > 0);
            tabResults.IsSelected = true;
        }

        private void SendCancelExecution(ReportExecutionResultSet resultSet)
        {
            throw new NotImplementedException();
        }

        private void SendCancelAllExecutions()
        {
            throw new NotImplementedException();
        }

        #region SIGNALR CLIENT CALLBACKS

        private void UpdateExecutionSet(ReportExecutionResultSet resultSet)
        {
            var item = ViewModel.executeReportsCollection.FirstOrDefault(r => r.guid == resultSet.guid);
            if (item != null)
            {
                var i = ViewModel.executeReportsCollection.IndexOf(item);
                ViewModel.executeReportsCollection[i] = resultSet;
            }
        }

        private void AllExecutionsCancelled()
        {
            var newReportList = new List<ReportExecutionResultSet>();
            foreach (var reportUnit in ViewModel.executeReportsCollection)
            {
                reportUnit.status = "Cancelled";
                newReportList.Add(reportUnit);
            }

            ViewModel.executeReportsCollection.Clear();
            ViewModel.executeReportsCollection.AddRange(newReportList);
        }

        private void ExecutionCancelled(ReportExecutionResultSet resultSet)
        {
            var item = ViewModel.executeReportsCollection.FirstOrDefault(r => r.guid == resultSet.guid);
            resultSet.status = "Cancelled";
            if (item != null)
            {
                var i = ViewModel.executeReportsCollection.IndexOf(item);
                ViewModel.executeReportsCollection[i] = resultSet;
            }
        }

        #endregion
    }
}