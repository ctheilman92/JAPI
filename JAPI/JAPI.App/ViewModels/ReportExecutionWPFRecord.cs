using JAPI.Repo;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace JAPI.App.Extensions
{
    public enum WPFRecrodStatus
    {
        Waiting = 0,
        InProgress = 1,
        Success = 2,
        Failed = 3
    }

    public class ReportExecutionWPFRecord : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChange([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private WPFRecrodStatus _rowStatus {get;set;}
        public WPFRecrodStatus rowStatus
        {
            get { return _rowStatus; }
            set { _rowStatus = value; RaisePropertyChange(); }
        }

        private ReportExecutionResultSet _resultSet;
        public ReportExecutionResultSet resultSet
        {
            get { return _resultSet; }
            set { _resultSet = value; RaisePropertyChange(); }
        }

        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
            set
            {
                var newImg = new BitmapImage();
                newImg.BeginInit();
                newImg.UriSource = value.UriSource;
                newImg.EndInit();
                newImg.Freeze();

                Dispatcher.CurrentDispatcher.Invoke(() => _Image = newImg);
                RaisePropertyChange();
            }
        }
    }
}
