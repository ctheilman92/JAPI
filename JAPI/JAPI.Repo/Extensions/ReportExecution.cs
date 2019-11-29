using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace JAPI.Repo
{
    public class ReportExecutionRequest : RequestObject
    {
        public string reportUnitUri { get; set; }
        public bool async { get; set; } = true;
        public bool freshData { get; set; } = false;
        public bool saveDataSnapshot { get; set; } = false;
        public string outputFormat { get; set; } = "html";
        public bool interactive { get; set; } = true;
        public bool ignorePagination { get; set; } = false;
        public string pages { get; set; } = "1";
        public List<ReportParameter> parameters { get; set; }

    }

    public class ReportExecutionResponse : ResponseObject
    {
        public string reportURI { get; set; }
        public string requestId { get; set; }
        public string status { get; set; }
        public List<Export> exports { get; set; }
        public int totalPages { get; set; }
    }

    public class Export
    {
        public string id { get; set; }
        public Options options { get; set; }
        public string status { get; set; }
        public OutputResource outputResource { get; set; }
        public List<Attachments> attachments { get; set; }
    }

    public class OutputResource
    {
        public string contentType { get; set; }
    }

    public class Attachments
    {
        public string contentType { get; set; }
        public string fileName { get; set; }
    }

    public class Options
    {
        public string outputFormat { get; set; }
        public string attachmentsPrefix { get; set; }
        public bool allowInlineScripts { get; set; }
        public string baseUrl { get; set; }
    }

    public class ReportParameter
    {
        [XmlAttribute]
        public string name { get; set; }
        [XmlElement]
        public string value { get; set; }
    }

    public class ExecutionStatus : ResponseObject
    {
        public string value { get; set; }
    }

}
