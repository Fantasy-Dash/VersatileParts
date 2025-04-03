using System.Diagnostics;
using System.Runtime.InteropServices;
using WmiLight;

namespace VP.Common.Helpers
{
    /// <summary>
    /// todo
    /// </summary>
    public static class SystemInformationHelper
    {
        public static DateTime GetUTCSystemInstallTime()
        {
            DateTime installTime = DateTime.MinValue;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // 获取Windows系统安装时间
                using var con = new WmiConnection("root\\cimv2");
                foreach (var m in con.CreateQuery("SELECT * FROM Win32_OperatingSystem"))
                {
                    var dateTimeStr = m["InstallDate"].ToString();
                    if (!string.IsNullOrWhiteSpace(dateTimeStr))
                        installTime=DateTimeHelper.DMTFToUTCDateTime(dateTimeStr);
                }
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
                            var parts = line.Split(["OS Install"], StringSplitOptions.RemoveEmptyEntries);
                            if (DateTime.TryParse(parts[0], out installTime))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return installTime.ToUniversalTime();
        }

        public static string GetUUID()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string uuid = string.Empty;
                using var con = new WmiConnection();
                foreach (var item in con.CreateQuery($"SELECT UUID FROM Win32_ComputerSystemProduct"))
                    uuid= item["UUID"].ToString()??string.Empty;
                if (string.IsNullOrWhiteSpace(uuid))
                    throw new ArgumentException();
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
