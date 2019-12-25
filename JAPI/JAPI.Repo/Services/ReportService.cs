using JAPI.Repo.Extensions;
using JAPI.Repo.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JAPI.Repo
{
    public class ReportService : JAPIService
    {

        public ReportService(IJAPIAuthenticateRepository repository = null) : base(repository)
        {
            TAG = "/reports";
            SetDefaultRequestParams();
        }

        public void SetDefaultRequestParams()
        {
            requestParams = new Dictionary<RequestParamKey, string>
            {
                { RequestParamKey.Async, "true" },
                { RequestParamKey.ReturnPages, "false" },
                { RequestParamKey.ReturnOutputFormat, "html" },
                { RequestParamKey.IgnorePagination, "true" },
            };
        }

        public async Task<Byte[]> RunReportAsync(string reportPath, Dictionary<RequestParamKey, string> requestParams = null)
        {
            var res = await GetJasperResponseAsync(reportPath, requestParams);
            return res.RawBytes;
        }
    }
}
