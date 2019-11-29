using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace JAPI.Repo
{
    public class JClient
    {
        public string Organization { get; set; }
        public string BaseURL { get; set; }

        private string _Username;
        public string Username
        {
            get { return (!string.IsNullOrEmpty(Organization)) ? $"{_Username}|{Organization}" : _Username; }
            set { _Username = value; }
        }
        public string Password { get; set; }
        public string JSessionID
        {
            get
            {
                var sessionId = string.Empty;
                if (SessionCookie != null)
                    sessionId = (SessionCookie.Expired || string.IsNullOrEmpty(SessionCookie.Value))
                        ? sessionId
                        : SessionCookie.Value;
                return sessionId;
            }
        }
        public int Timeout { get; set; } = 30 * 10000;
        public RestResponseCookie SessionCookie { get; set; } = new RestResponseCookie();
        public JAuthtype AuthType { get; set; } = JAuthtype.Session;
    }

    public enum JAuthtype
    {
        Session = 0,
        BasicAuthentication = 1
    }
}
