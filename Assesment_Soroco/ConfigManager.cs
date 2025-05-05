using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assesment_Soroco
{
    public class ConfigManager
    {

        public static int MonitoringInterval => GetIntSetting("MonitoringInterval", 3000);
        public static string HostUrl => GetStringSetting("HostUrl", "");
        public static bool EnableFileLogging => GetBoolSetting("EnableFileLogging", true);
        public static bool EnableApiIntegration => GetBoolSetting("EnableApiIntegration", false);
        public static string LogFilePath => GetStringSetting("LogFilePath", "system_metrics.log");
       

        public static double cpuUsage;
        public static double UsedMemory;
        public static double TotalMemory;
        public static double UsedDisk;
        public static double TotalDisk;
        public static double memoryUsage;
        public static double DiskUsage;
             
        private static string GetStringSetting(string key, string defaultValue)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static int GetIntSetting(string key, int defaultValue)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return int.TryParse(value, out int result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static bool GetBoolSetting(string key, bool defaultValue)
        {
            try
            {
                string value = ConfigurationManager.AppSettings[key];
                return bool.TryParse(value, out bool result) ? result : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
