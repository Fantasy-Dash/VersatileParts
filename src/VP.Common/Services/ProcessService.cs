using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using VP.Common.Services.Interface;

namespace VP.Common.Services
{
    [UnsupportedOSPlatform("windows")]
    public class ProcessService : IProcessService
    {

        public IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter)
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

        public int GetParentProcessId(Process process) => GetParentProcessId(process.Id);
        public int GetParentProcessId(int processId)
        {
            int parentId = 0;
            try
            {
                using var reader = new StreamReader($"/proc/{processId}/stat");
                string[] fields = reader.ReadToEnd().Split(' ');
                parentId = Convert.ToInt32(fields[3]);
            }
            catch { }

            return parentId;
        }

        public IEnumerable<KeyValuePair<int, int>> GetProcessIdAndParentProcessIdList(IEnumerable<int> processIdList)
        {
            var ret = new List<KeyValuePair<int, int>>();
            foreach (var item in processIdList)
                ret.Add(new KeyValuePair<int, int>(item, GetParentProcessId(item)));
            return ret;
        }

        public IEnumerable<KeyValuePair<int, int>> GetProcessIdAndParentProcessIdList(IEnumerable<Process> processList)
        {
            var ret = new List<KeyValuePair<int, int>>();
            foreach (var item in processList)
                ret.Add(new KeyValuePair<int, int>(item.Id, GetParentProcessId(item.Id)));
            return ret;
        }
    }
}
