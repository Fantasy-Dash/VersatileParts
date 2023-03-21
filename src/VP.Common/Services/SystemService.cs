using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using VP.Common.Services.Interface;

namespace VP.Common.Services
{
    [UnsupportedOSPlatform("windows")]
    public class SystemService : ISystemService
    {
        public virtual DateTime GetSystemInstallTime()
        {
            DateTime installTime = DateTime.MinValue;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
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
                            var parts = line.Split(new[] { "OS Install" }, StringSplitOptions.RemoveEmptyEntries);
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
