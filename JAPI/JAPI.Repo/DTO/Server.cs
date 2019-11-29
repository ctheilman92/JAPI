using System;
using System.Collections.Generic;
using System.Text;

namespace JAPI.Repo
{
    public class Server
    {
        public string dateFormatPattern { get; set; }

        public string datetimeFormatPattern { get; set; }

        public string version { get; set; }

        public string edition { get; set; }

        public string editionName { get; set; }

        public string build { get; set; }

        public string licenseType { get; set; }

        public string expiration { get; set; }

        public string features { get; set; }
    }
}
