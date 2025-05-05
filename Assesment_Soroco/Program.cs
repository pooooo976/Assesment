using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Assesment_Soroco.Plugin;
using System.Threading.Tasks;
using log4net;
using System;
using System.Threading;
using SystemMonitor;

namespace Assesment_Soroco
{
    internal class Program
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(Program));
        private static PluginManager _pluginManager = new PluginManager();
        private static bool _isRunning = true;
        public static string hostUrl = ConfigManager.HostUrl;

        static async Task Main(string[] args)
        {
            Task webTask = Task.CompletedTask; 

            if (ConfigManager.EnableApiIntegration)
            {
                var webHost = CreateHostBuilder(args).Build();
                webTask = webHost.RunAsync();
            }

            var monitorTask = Task.Run(() => RunSystemMonitor());

            await Task.WhenAll(webTask, monitorTask);
        }

       
        private static void RunSystemMonitor()
        {
            var systemMonitor = new SystemMonitoring();
            _pluginManager.LoadPlugins();

            while (_isRunning)
            {
                double cpuUsage = SystemMonitoring.GetCpuUsage();
                var memoryInfo = SystemMonitoring.GetMemoryUsage();
                var diskInfo = SystemMonitoring.GetDiskUsage();

                cpuUsage = Math.Round(cpuUsage, 2);
                double memoryUsedMB = Math.Round(memoryInfo.UsedMB, 2);
                double totalMemoryMB = Math.Round(memoryInfo.TotalMB, 2);
                double memoryUsagePercent = Math.Round(memoryInfo.UsagePercent, 2);

                double diskUsedMB = Math.Round(diskInfo.UsedMB, 2);
                double totalDiskMB = Math.Round(diskInfo.TotalMB, 2);
                double diskUsagePercent = Math.Round(diskInfo.UsagePercent, 2);
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Console.WriteLine($"{timestamp} CPU: {cpuUsage:F2}%, Disk: {diskUsedMB}/{totalDiskMB}({diskUsagePercent:F2}%), RAM: {memoryUsedMB}/{totalMemoryMB}({memoryUsagePercent:F2}%)");

                // Update ConfigManager with rounded values
                ConfigManager.cpuUsage = cpuUsage;
                ConfigManager.UsedMemory = memoryUsedMB;
                ConfigManager.TotalMemory = totalMemoryMB;
                ConfigManager.memoryUsage = memoryUsagePercent;

                ConfigManager.UsedDisk = diskUsedMB;
                ConfigManager.TotalDisk = totalDiskMB;
                ConfigManager.DiskUsage = diskUsagePercent;

              

                _pluginManager.ProcessUpdate(
                    cpuUsage,
                    memoryInfo.UsedMB,
                    memoryInfo.TotalMB,
                    diskInfo.UsedMB,
                    diskInfo.TotalMB
                );

                Thread.Sleep(ConfigManager.MonitoringInterval); 
            }
        }

    
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(ConfigManager.HostUrl);
                    webBuilder.ConfigureServices(services =>
                    {
                        services.AddControllers();
                    });
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
    }
}