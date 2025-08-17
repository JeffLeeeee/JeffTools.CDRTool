using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace JeffTools.CDRTool.Common.Helpers
{
    public class ConfigHelper
    {
        public ConfigHelper()
        {
            Init();
        }
        private string  TempPath { get; set; }
        private readonly string ConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + "config.ini";


        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(
            string section, string key, string defaultValue,
            StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(
            string section, string key, string value, string filePath);

        public void SetTempPath(string path)
        {
            if (path == null || path == "")
            {
                MessageBox.Show("Temp path cannot be empty!", "提示"); 
            }
            else
            {
                SetConfig("tempPath", path);
            }
        }

        public string GetTempPath()
        {
            return GetConfig("tempPath");
        }

        public void Init()
        {
            if(GetTempPath()=="")
            {
                TempPath = Path.Combine(Path.GetTempPath(), "JeffTools-CDRTool");
                if(!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }
                if (!Directory.Exists(ConfigFilePath))
                {
                    using (FileStream configFile = File.Create(ConfigFilePath)){
                        Console.WriteLine("Config file created.");
                    }
                    SetConfig("tempPath", TempPath);
                }
            }
        }

        public void SetConfig( string key, string value, string section = "default")
        {
            WritePrivateProfileString(section, key, value, ConfigFilePath);
        }

        public string GetConfig(string key, string section = "default")
        {
            StringBuilder temp = new StringBuilder(255);
            GetPrivateProfileString(section, key, "", temp, 255, ConfigFilePath);
            return temp.ToString();
        }
    }
}
