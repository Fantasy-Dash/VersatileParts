using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;

namespace VP.Windows.Services
{
    [SupportedOSPlatform("windows")]
    public class ProcessService : Common.Services.ProcessService
    {
        public override IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter)
        {
            var processIdList = new List<int>();
            try
            {
                // 在 Windows 平台上使用 WMI 获取命令行参数
                using var searcher = new ManagementObjectSearcher($"SELECT ProcessId FROM Win32_Process WHERE CommandLine Like '%{filter}%'");
                using var QueryList = searcher.Get();
                foreach (var item in QueryList)
                    processIdList.Add(Convert.ToInt32(item["ProcessId"]));
            }
            catch (NullReferenceException) { }
            return processes.Where(row => processIdList.Contains(row.Id));
        }
    }
}
