using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
//using SystemMonitor.Configuration;
//using SystemMonitor.Plugins;

namespace Assesment_Soroco.Plugin
{

    public class PluginManager
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(PluginManager));
        private List<MonitorPlugin> _plugins = new List<MonitorPlugin>();

        public void LoadPlugins()
        {
            try
            {
                
                _plugins.Add(new LoggerPlugin());
                _logger.Info($"Loaded {_plugins.Count} plugins.");


                foreach (var plugin in _plugins.Where(p => p.IsEnabled))
                {
                    try
                    {
                        plugin.Initialize();
                        _logger.Info($"Initialized plugin: {plugin.Name}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Failed to initialize plugin {plugin.Name}: {ex.Message}", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading plugins: {ex.Message}", ex);
            }
        }

       public void ProcessUpdate(double cpuUsage, double memoryUsedMB, double memoryTotalMB,
                              double diskUsedMB, double diskTotalMB)
        {
            foreach (var plugin in _plugins.Where(p => p.IsEnabled))
            {
                try
                {
                    plugin.ProcessUpdate(cpuUsage, memoryUsedMB, memoryTotalMB, diskUsedMB, diskTotalMB);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error in plugin {plugin.Name}: {ex.Message}", ex);
                }
            }
        }

        public void ShutdownPlugins()
        {
            foreach (var plugin in _plugins.Where(p => p.IsEnabled))
            {
                try
                {
                    plugin.Shutdown();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error shutting down plugin {plugin.Name}: {ex.Message}", ex);
                }
            }

            _logger.Info("All plugins shut down");
        }
    }
}