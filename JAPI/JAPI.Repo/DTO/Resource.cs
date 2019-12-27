using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace JAPI.Repo
{
    public class Resource
    {
        public string uri { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public int permissionMask { get; set; }
        public string creationDate { get; set; }
        public string updateDate { get; set; }
        public int version { get; set; }
        public string resourceType { get; set; }
    }


    public class ResourceLookup<T> where T : Resource
    {
        public List<T> resourceLookup { get; set; }
    }


    public class ReportExecutionResultSet : INotifyPropertyChanged
    {
        private Guid _guid;
        public Guid guid
        {
            get { return _guid; }
            set { _guid = value; RaisePropertyChange(); }
        }

        private ReportUnit _resource;
        public ReportUnit resource
        {
            get { return _resource; }
            set { _resource = value; RaisePropertyChange(); }
        }

        private string _status;
        public string status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChange(); }
        }

        private string _internalError;
        public string internalError
        {
            get { return _internalError; }
            set { _internalError = value; RaisePropertyChange(); }
        }

        public string _requestId { get; set; }
        public string requestId
        {
            get { return _requestId; }
            set { _requestId = value; RaisePropertyChange(); }
        }

        private Export _export;
        public Export export
        {
            get { return _export; }
            set { _export = value; RaisePropertyChange(); }
        }

        private bool _successful;
        public bool successful
        {
            get { return _successful; }
            set { _successful = value; RaisePropertyChange(); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChange([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public ReportExecutionRequest GetDefaultRequestObject()
        {
            return new ReportExecutionRequest
            {
                reportUnitUri = (this.resource == null) ? string.Empty : this.resource.uri,
                async = false,
                outputFormat = "pdf",
                freshData = false,
                saveDataSnapshot = false,
                ignorePagination = false
            };
        }
    }

    //build these up to be more generic later. for now don't use ResourceSearchParams
    public class ResourceSearchParams<T> where T : Resource
    {
        public List<KeyValuePair<string,string>> requestParams { get; set; }
    }


    public class Folder : Resource
    {

    }

    public class ResourceFiles
    {
        public string name { get; set; }
        public File file { get; set; }
    }

    public class JDBCDataSource : Resource
    {
        public DataSourceReference datasourceReference { get; set; }
    }

    public class Query : Resource
    {
        public QueryReference queryReference { get; set; }
    }


    public class InputControl : Resource
    {
        public InputControlReference inputControlRerference { get; set; }
    }

    public class File : Resource
    {
        public string type { get; set; }
        public string content { get; set; }
    }

    public class JRXML
    {
        public JRXMLReference jrxmlReference { get; set; }
    }

    public class ReportUnit : Resource
    {
        public bool alwaysPromptControls { get; set; }
        public string controlsLayout { get; set; }
        public string inputControlRenderingView { get; set; }
        public string reportRenderingView { get; set; }
        public JDBCDataSource dataSource { get; set; }
        public Query query { get; set; }
        public JRXML jrxml { get; set; }
        public InputControl inputControl { get; set; }
        public List<ResourceFiles> resources { get; set; }
    }

    //public class ReportParameter
    //{
    //    public string name { get; set; }
    //    public List<string> values { get; set; }
    //}

    public class ReportOptions : Resource
    {
        public string reportUri { get; set; }
    }
}
