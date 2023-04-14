using System.Diagnostics;
using System.Management;
using System.Runtime.Versioning;
using System.Text;
using VP.Common.Services.Interface;

namespace VP.Windows.Services
{
    [SupportedOSPlatform("windows")]
    public class ProcessService : IProcessService
    {
        public IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter)
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

        public int GetParentProcessId(Process process) => GetParentProcessId(process.Id);
        public int GetParentProcessId(int processId)
        {
            int parentId = 0;
            using var searcher = new ManagementObjectSearcher($"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}");
            foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
            {
                parentId = Convert.ToInt32(obj["ParentProcessId"]);
                break;
            }
            return parentId;
        }

        public IEnumerable<KeyValuePair<int, int>> GetProcessIdAndParentProcessIdList(IEnumerable<int> processIdList)
        {
            var ret = new Dictionary<int, int>();
            if (processIdList.Any())
            {
                var sb = new StringBuilder(" ProcessId ="+processIdList.First());
                _ = processIdList.Skip(1).Select(row => sb.Append(" OR ProcessId ="+row));
                using var searcher = new ManagementObjectSearcher($"SELECT ProcessId,ParentProcessId FROM Win32_Process WHERE {sb}");
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                    ret.Add(Convert.ToInt32(obj["ProcessId"]), Convert.ToInt32(obj["ParentProcessId"]));
            }
            return ret;
        }

        public IEnumerable<KeyValuePair<int, int>> GetProcessIdAndParentProcessIdList(IEnumerable<Process> processList) => GetProcessIdAndParentProcessIdList(processList.Select(row => row.Id));
    }
}
