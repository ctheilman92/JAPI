using System;
using System.Collections.Generic;
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
