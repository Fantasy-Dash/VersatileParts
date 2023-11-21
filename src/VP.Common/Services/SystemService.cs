using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using VP.Common.Services.Interface;

namespace VP.Common.Services
{
    public class SystemService : ISystemService
    {
        private static readonly string[] separator = ["OS Install"];

        public virtual DateTime GetSystemInstallTime()
        {
            DateTime installTime = DateTime.MinValue;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // 获取Windows系统安装时间
                var scope = new ManagementScope(@"\\localhost\root\cimv2");
                var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
                using var searcher = new ManagementObjectSearcher(scope, query);
                using var queryCollection = searcher.Get();
                foreach (var m in queryCollection)
                {
                    var dateTimeStr = m["InstallDate"].ToString();
                    if (!string.IsNullOrWhiteSpace(dateTimeStr))
                        return ManagementDateTimeConverter.ToDateTime(dateTimeStr);
                }
                return installTime;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                // 获取Linux系统安装时间
                var logFile = "/var/log/installer/syslog";
                if (File.Exists(logFile))
                {
                    using var reader = new StreamReader(logFile);
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("install completed") && DateTime.TryParse(line[..15], out installTime))
                        {
                            break;
                        }
                    }
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // 获取macOS系统安装时间
                var logFile = "/var/log/install.log";
                if (File.Exists(logFile))
                {
                    using var reader = new StreamReader(logFile);
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("OS Install")) // macOS安装日志中包含"OS Install"关键字
                        {
                            var parts = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                            if (DateTime.TryParse(parts[0], out installTime))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return installTime;
        }

        public virtual string GetUUID()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string uuid = string.Empty;
                var query = new SelectQuery("Win32_ComputerSystemProduct");
                using var searcher = new ManagementObjectSearcher(query);
                using var queryCollection = searcher.Get();
                foreach (var item in queryCollection)
                    if (item["UUID"] != null)
                        uuid= item["UUID"].ToString()??string.Empty;
                if (string.IsNullOrWhiteSpace(uuid))
                    throw new ManagementException();
                return uuid;
            }
            else
            {
                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    startInfo.FileName = "/usr/sbin/dmidecode";
                    startInfo.Arguments = "-s system-uuid";
                }
                else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    startInfo.FileName = "/usr/sbin/system_profiler";
                    startInfo.Arguments = "SPHardwareDataType | awk '/UUID/ { print $3; }'";
                }
                process.StartInfo = startInfo;
                process.Start();
                return process.StandardOutput.ReadToEnd().Trim();
            }
        }
    }
}
