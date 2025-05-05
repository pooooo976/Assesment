using Microsoft.AspNetCore.Mvc;
using SystemMonitor;

namespace Assesment_Soroco.API
{
    [ApiController]
    [Route("checkSystemUsages")]
    public class MetricsController : ControllerBase
    {
        // GET api/metrics
        [HttpGet]
        public IActionResult GetMetrics()
        {
            // Collect system metrics
            double cpuUsage =ConfigManager.cpuUsage;
            double memoryInfo =ConfigManager.UsedMemory;
            double diskInfo = ConfigManager.UsedDisk;

            // Create the payload as per the required format
            var payload = new
            {
                cpu = $"{cpuUsage}%",
                ram_used = $"{memoryInfo} MB",
                disk_used = $"{diskInfo} MB"
            };

         
            return Ok(payload);
        }
    }
}
