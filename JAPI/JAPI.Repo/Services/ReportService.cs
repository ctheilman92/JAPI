﻿using JAPI.Repo.Extensions;
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
        }

        public async Task<Byte[]> RunReportAsync(string reportPath, Dictionary<RequestParamKey, string> requestParams = null)
        {
            var res = await GetJasperResponseAsync(reportPath, requestParams);
            return res.RawBytes;
        }
    }
}
