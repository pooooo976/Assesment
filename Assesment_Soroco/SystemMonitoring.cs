using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using log4net;

namespace SystemMonitor
{
    public class SystemMonitoring
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(SystemMonitoring));

        public static double GetCpuUsage()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) // for windows
                {
                    using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                    {
                        cpuCounter.NextValue();
                        Thread.Sleep(1000);
                        return cpuCounter.NextValue();
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) //for Linux - RHEL, Ubuntu, etc.
                {
                    string[] initialStats = File.ReadAllText("/proc/stat").Split('\n')[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    long initialIdle = long.Parse(initialStats[4]);
                    long initialTotal = 0;
                    for (int i = 1; i < initialStats.Length; i++)
                        initialTotal += long.Parse(initialStats[i]);

                    Thread.Sleep(1000);

                    string[] currentStats = File.ReadAllText("/proc/stat").Split('\n')[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    long currentIdle = long.Parse(currentStats[4]);
                    long currentTotal = 0;
                    for (int i = 1; i < currentStats.Length; i++)
                        currentTotal += long.Parse(currentStats[i]);

                    double idleDelta = currentIdle - initialIdle;
                    double totalDelta = currentTotal - initialTotal;

                    return 100.0 * (1.0 - idleDelta / totalDelta);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) // for Mac
                {
                    _logger.Warn("CPU usage measurement not supported for mac now. will add soon.");
                    return 0;
                }

                _logger.Warn("CPU usage measurement not supported on this platform. Returning 0.");
                return 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting CPU usage: {ex.Message}", ex);
                return 0;
            }
        }


        public static (double UsedMB, double TotalMB, double UsagePercent) GetMemoryUsage()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var memStatus = new MEMORYSTATUSEX();
                    if (GlobalMemoryStatusEx(memStatus))
                    {
                        double totalMemoryMB = memStatus.ullTotalPhys / 1024 / 1024;
                        double availableMemoryMB = memStatus.ullAvailPhys / 1024 / 1024;
                        double usedMemoryMB = totalMemoryMB - availableMemoryMB;
                        double usagePercent = (usedMemoryMB / totalMemoryMB) * 100;

                        return (usedMemoryMB, totalMemoryMB, usagePercent);
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string[] lines = File.ReadAllLines("/proc/meminfo");
                    double totalMem = 0;
                    double freeMem = 0;
                    double buffers = 0;
                    double cached = 0;

                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length >= 2)
                        {
                            if (line.StartsWith("MemTotal"))
                                totalMem = double.Parse(parts[1]);
                            else if (line.StartsWith("MemFree"))
                                freeMem = double.Parse(parts[1]);
                            else if (line.StartsWith("Buffers"))
                                buffers = double.Parse(parts[1]);
                            else if (line.StartsWith("Cached") && !line.Contains("SwapCached"))
                                cached = double.Parse(parts[1]);
                        }
                    }

                    // Memory values in /proc/meminfo are in KB
                    double totalMemoryMB = totalMem / 1024;
                    double usedMemoryMB = (totalMem - freeMem - buffers - cached) / 1024;
                    double usagePercent = (usedMemoryMB / totalMemoryMB) * 100;

                    return (usedMemoryMB, totalMemoryMB, usagePercent);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    _logger.Warn("Memory usage measurement not supported for mac now. will add soon.");
                    return (0,0,0);
                }

                _logger.Warn("Memory usage measurement not fully supported on this platform. Estimating values.");

                long managedMemoryBytes = GC.GetTotalMemory(false);
                double managedMemoryMB = managedMemoryBytes / 1024.0 / 1024.0;

                return (managedMemoryMB, 0, 0);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting memory usage: {ex.Message}", ex);
                return (0, 0, 0);
            }
        }


        public static (double UsedMB, double TotalMB, double UsagePercent) GetDiskUsage()
        {
            try
            {
                // Get system drive
                string systemDrive;

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    systemDrive = Path.GetPathRoot(Environment.SystemDirectory);
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    systemDrive = "/";
                }
                else
                {
                    _logger.Warn("Disk usage measurement not supported on this platform. Returning 0.");
                    return (0, 0, 0);
                }
               
                var driveInfo = new DriveInfo(systemDrive);

                if (driveInfo.IsReady)
                {
                    double totalDiskMB = driveInfo.TotalSize / 1024.0 / 1024.0;
                    double freeDiskMB = driveInfo.AvailableFreeSpace / 1024.0 / 1024.0;
                    double usedDiskMB = totalDiskMB - freeDiskMB;
                    double usagePercent = (usedDiskMB / totalDiskMB) * 100;

                    return (usedDiskMB, totalDiskMB, usagePercent);
                }

                _logger.Warn($"Drive {systemDrive} is not ready");
                return (0, 0, 0);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting disk usage: {ex.Message}", ex);
                return (0, 0, 0);
            }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}