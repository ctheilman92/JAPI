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
            string configPath = @"C:\Users\cameron.heilman\Documents\WR\JAPI_REPO\japi\JAPI\JAPI.Repo\JAPI.ini";
            //string startupPath = Path.Combine(Environment.CurrentDirectory, "JAPI.ini");
            var config = new INI.IniFile(configPath);

            return new JClient
            {
                Username = config.IniReadValue("JasperConfiguration", "USERNAME"),
                Password = config.IniReadValue("JasperConfiguration", "PASSWORD"),
                Organization = config.IniReadValue("JasperConfiguration", "DEFAULT_ORG"),
                Timeout = Convert.ToInt32(config.IniReadValue("JasperConfiguration", "CURL_TIMEOUT")),
                BaseURL = config.IniReadValue("JasperConfiguration", "BASE_SERVER_URL")
            };
        }
    }
}
