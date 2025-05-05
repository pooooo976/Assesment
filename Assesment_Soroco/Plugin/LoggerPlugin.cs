using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.IO;
using log4net;


namespace Assesment_Soroco.Plugin
{
    public class LoggerPlugin : MonitorPlugin
    {

        private static readonly ILog _logger = LogManager.GetLogger(typeof(LoggerPlugin));
        private string _logFilePath="";
        private StreamWriter _writer;

        public string Name => "File Logger Plugin";
        public bool IsEnabled => ConfigManager.EnableFileLogging;

        public void Initialize()
        {
            if (!IsEnabled) return;

            try
            {
                _logFilePath = ConfigManager.LogFilePath;
                _writer = new StreamWriter(_logFilePath, true);
                _writer.Flush();

                _logger.Info($"FileLoggerPlugin initialized. Writing to {_logFilePath}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to initialize FileLoggerPlugin: {ex.Message}");
            }
        }

        public void ProcessUpdate(double cpuUsage, double memoryUsedMB, double memoryTotalMB,
                                double diskUsedMB, double diskTotalMB)
        {
            if (!IsEnabled || _writer == null) return;

            try
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string logEntry = $"{timestamp}, CPU: {cpuUsage:F1}%, Memory: {memoryUsedMB:F0}/{memoryTotalMB:F0} MB ({memoryUsedMB / memoryTotalMB * 100:F1}%), " +
                                 $"Disk: {diskUsedMB / 1024:F1}/{diskTotalMB / 1024:F1} GB ({diskUsedMB / diskTotalMB * 100:F1}%)";

                _writer.WriteLine(logEntry);
                _writer.Flush();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in FileLoggerPlugin: {ex.Message}");
            }
        }

        public void Shutdown()
        {
            if (_writer != null)
            {
                try
                {
                    _writer.WriteLine($"--- System Monitoring Ended at {DateTime.Now} ---");
                    _writer.Close();
                    _writer.Dispose();
                    _logger.Info("FileLoggerPlugin shut down successfully.");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error shutting down FileLoggerPlugin: {ex.Message}");
                }
            }
        }
    }
}