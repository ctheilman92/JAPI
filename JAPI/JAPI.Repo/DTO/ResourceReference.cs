using System;
using System.Collections.Generic;
using System.Text;

namespace JAPI.Repo
{
    public class ResourceReference
    {
        public string uri { get; set; }
    }

    public class QueryReference : ResourceReference
    {
        public ResourceRefType refType { get; set; } = ResourceRefType.QueryRef;
    }

    public class JRXMLReference : ResourceReference
    {
        public ResourceRefType refType { get; set; } = ResourceRefType.JRXMLRef;
    }

    public class DataSourceReference : ResourceReference
    {
        public ResourceRefType refType { get; set; } = ResourceRefType.DatasourceRef;
    }

    public class InputControlReference : ResourceReference
    {
        public ResourceRefType refType { get; set; } = ResourceRefType.InputControlRef;
    }


    public enum ResourceRefType
    {
        None = 0,
        DatasourceRef = 1,
        QueryRef = 2,
        JRXMLRef = 3,
        InputControlRef = 4,
    }
}
