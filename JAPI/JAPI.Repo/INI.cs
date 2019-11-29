using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JAPI.Repo
{
    public class INI
    {
        public class IniFile
        {
            public string INIPath;

            [DllImport("kernel32")]
            private static extern long WritePrivateProfileString(string section,
                string key, string val, string filePath);
            [DllImport("kernel32")]
            private static extern int GetPrivateProfileString(string section,
                     string key, string def, StringBuilder retVal,
                int size, string filePath);


            public IniFile(string INIPath)
            {
                this.INIPath = INIPath;
            }

            public void IniWriteValue(string Section, string Key, string Value)
            {
                WritePrivateProfileString(Section, Key, Value, this.INIPath);
            }

            public string IniReadValue(string Section, string Key)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp,
                                                255, this.INIPath);
                return temp.ToString();

            }
        }
    }
}
