using System;
using System.Collections.Generic;
using System.Text;

namespace JAPI.Repo
{
    public class ErrorDescriptor
    {
        public string errorCode { get; set; }
        public string message { get; set; }
        public List<ReportParameter> parameters { get; set; }
    }
}
