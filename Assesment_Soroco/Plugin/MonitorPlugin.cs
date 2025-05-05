using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assesment_Soroco.Plugin
{
    public interface MonitorPlugin
    {

        string Name { get; }
        bool IsEnabled { get; }

        void Initialize();
        void ProcessUpdate(double cpuUsage, double memoryUsedMB, double memoryTotalMB,
                          double diskUsedMB, double diskTotalMB);
        void Shutdown();
    }
}