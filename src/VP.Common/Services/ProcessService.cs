using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using VP.Common.Services.Interface;

namespace VP.Common.Services
{
    public class ProcessService : IProcessService
    {

        public IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
            else
            {
                // 筛选符合条件的进程
                return processes.Where(process =>
            {
                string commandLine = string.Empty;
                try
                {
                    // 在 Linux 和 macOS 平台上读取 /proc 文件系统获取命令行参数
                    string procFile = $"/proc/{process.Id}/cmdline";
                    if (File.Exists(procFile))
                        commandLine = File.ReadAllText(procFile);
                }
                catch (NullReferenceException) { }
                // 正则表达式匹配命令行参数
                return commandLine != null && Regex.IsMatch(commandLine, $@"\b{filter}\b", RegexOptions.IgnoreCase);
            }).ToList();
            }
        }

        public int GetParentProcessId(Process process) => GetParentProcessId(process.Id);
        public int GetParentProcessId(int processId)
        {
            int parentId = 0;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var searcher = new ManagementObjectSearcher($"SELECT ParentProcessId FROM Win32_Process WHERE ProcessId = {processId}");
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                {
                    parentId = Convert.ToInt32(obj["ParentProcessId"]);
                    break;
                }
            }
            else
            {
                try
                {
                    using var reader = new StreamReader($"/proc/{processId}/stat");
                    string[] fields = reader.ReadToEnd().Split(' ');
                    parentId = Convert.ToInt32(fields[3]);
                }
                catch { }
            }
            return parentId;
        }

        public IDictionary<int, int> GetProcessIdAndParentProcessIdList(IEnumerable<int> processIdList)
        {
            var ret = new Dictionary<int, int>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (processIdList.Any())
                {
                    var sb = new StringBuilder(" ProcessId ="+processIdList.First());
                    processIdList.Skip(1).ToList().ForEach(row => sb.Append(" OR ProcessId ="+row));
                    using var searcher = new ManagementObjectSearcher($"SELECT ProcessId,ParentProcessId FROM Win32_Process WHERE {sb}");
                    foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                        ret.Add(Convert.ToInt32(obj["ProcessId"]), Convert.ToInt32(obj["ParentProcessId"]));
                }
            }
            else
            {
                foreach (var item in processIdList)
                    ret.Add(item, GetParentProcessId(item));
            }
            return ret;
        }

        public IDictionary<int, int> GetProcessIdAndParentProcessIdList(IEnumerable<Process> processList) => GetProcessIdAndParentProcessIdList(processList.Select(row => row.Id));
    }
}
