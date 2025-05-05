# System Monitor
https://github.com/pooooo976/Assesment.git

## Overview

This application is a cross-platform desktop system monitoring tool built using C#. It tracks CPU usage, memory usage, and disk space in real-time. It includes plugin support for integrations such as logging to a file and exposing a REST API for external access.
It can also be used to send alerts via email or Telegram if the usage exceeds the upper limit set by the us.(Will add this later)

### 🔗 Executable

The binary of the solution is available [here](https://github.com/pooooo976/Assesment/blob/main/Assesment_Soroco/bin/Debug/net8.0), and you can directly run the application using this [executable](https://github.com/pooooo976/Assesment/blob/main/Assesment_Soroco/bin/Debug/net8.0/Assesment_Soroco.exe).



## Configuration

All application settings are defined in the configuration file:

```xml
<add key="MonitoringInterval" value="3000" />
<add key="HostUrl" value="http://localhost:5000" />
<add key="EnableFileLogging" value="true" />
<add key="EnableApiIntegration" value="true" />
<add key="LogFilePath" value="C:\\QT\\Resource.log" />
```

## Console Output

The application prints system metrics to the console at regular intervals:

```
DateTime CPU: %, Disk: used/total(%), RAM: used/total(%)
2025-05-06 02:16:23 CPU: 8.94%, Disk: 164413.52/187055.2 (87.90%), RAM: 13824/16125 (85.73%)
```

## API Output

When the API endpoint is hit, the following JSON response is returned:

```json
{
  "cpu": "3.41%",
  "ram_used": "13839 MB",
  "disk_used": "164416.25 MB"
}
```

## Log File

The plugin system supports file logging. The log entries are stored in the path defined by the `LogFilePath` key in the configuration:

```
C:\QT\Resource.log
```

## Screenshots

Console Output:

![Console Output](https://github.com/user-attachments/assets/2222f97a-7026-47e5-ba7f-4eed55596b51)



API Response:

![API Response](https://github.com/user-attachments/assets/54033bf2-97c9-4b6b-a682-62ee0495e006)



Log File:

![Log File](https://github.com/user-attachments/assets/f10b1112-c8e6-425e-949e-209912c4bcbd)
![Log Path](https://github.com/user-attachments/assets/00ef9b4f-9c22-4a9c-ac31-60ec76287b1a)

---

For any additional details, refer to the code or contact me via Linkdin @tiwariswapnil100 (https://www.linkedin.com/in/tiwariswapnil100/)


