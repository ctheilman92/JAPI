using IniParser;
using IniParser.Model;
using JAPI.Repo.Repositories;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JAPI.Repo
{
    public static class Utility
    {
        public const string LOGFILE = @"C:\JAPI\Logs\";

        public static void WriteLog(string logMessage, TextWriter w)
        {
            System.IO.File.AppendAllText(LOGFILE, logMessage);
        }

        public static async Task<string> GetLog()
        {
            byte[] result;
            using (FileStream fs = System.IO.File.Open(LOGFILE, FileMode.Open))
            {
                result = new byte[fs.Length];
                await fs.ReadAsync(result, 0, (int)fs.Length);
            }

            return System.Text.Encoding.ASCII.GetString(result);
        }

        public static JClient GetJClientINI()
        {
            string configPath = @"C:\Users\cameron.heilman\Documents\WR\GIT_JAPI\japi\JAPI\JAPI.Repo\JASPER.ini";
            //string startupPath = Path.Combine(Environment.CurrentDirectory, "JAPI.ini");

            using (var sr = new StreamReader(configPath))
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadData(sr);

                return new JClient
                {
                    Username = data["JasperConfiguration"]["USERNAME"],
                    Password = data["JasperConfiguration"]["PASSWORD"],
                    Organization = data["JasperConfiguration"]["DEFAULT_ORG"],
                    Timeout = Convert.ToInt32(data["JasperConfiguration"]["CURL_TIMEOUT"]),
                    BaseURL = data["JasperConfiguration"]["BASE_SERVER_URL"].TrimEnd('/')
                };
            }
        }
    }
}
