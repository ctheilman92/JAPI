using System;
using System.Collections.Generic;
using System.Text;

namespace JAPI.Repo
{
    public class Org
    {
        public string id { get; set; }
        public string parentId { get; set; }
        public string alias { get; set; }
        public string tenantDesc { get; set; }
        public Uri tenantFolderUri { get; set; }
        public Uri tenantUri { get; set; }
        public string tenantName { get; set; }
        public string theme { get; set; }
    }
}
