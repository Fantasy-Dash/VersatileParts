using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;

namespace VP.Windows.Services
{
    [SupportedOSPlatform("windows")]
    public class ProcessService : Common.Services.ProcessService
    {
        public override string GetCommandLine(Process process)
        {
            string commandLine = string.Empty;
            try
            {
                // 在 Windows 平台上使用 WMI 获取命令行参数
                using var searcher = new ManagementObjectSearcher($"SELECT CommandLine FROM Win32_Process WHERE ProcessId = {process.Id}");
                using var QueryList = searcher.Get();
                foreach (var item in QueryList)
                {
                    commandLine = item["CommandLine"]?.ToString()??string.Empty;
                    break;
                }
            }
            catch (NullReferenceException) { }
            return commandLine;
        }
    }
}
