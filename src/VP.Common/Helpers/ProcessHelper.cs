using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Text;

namespace VP.Common.Helpers
{
    /// <summary>
    /// 进程相关帮助方法
    /// </summary>
    public static class ProcessHelper
    {
        /// <summary>
        /// 查找其他同名进程 忽略vs调试会查找到的一些空线程进程
        /// </summary>
        /// <param name="process">源进程</param>
        /// <param name="uniqueProcess">查找到的进程</param>
        /// <returns>是否获取到不同的进程</returns>
        public static bool TryFindNonDuplicateProcess(Process process, out Process uniqueProcess)
        {
            uniqueProcess = Process.GetProcessesByName(process.ProcessName)
                .Where(row => row.Id != process.Id
                              && !row.HasExited
                              && row.Threads.Count > 0).FirstOrDefault()??process;
            return uniqueProcess!=process;
        }

        public static IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter)
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

        public static int GetParentProcessId(Process process) => GetParentProcessId(process.Id);
        public static int GetParentProcessId(int processId)
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
                using var reader = new StreamReader($"/proc/{processId}/stat");
                var result = reader.ReadToEnd();
                if (result.IndexOf("No such file or directory")>-1)
                    throw new ArgumentNullException($"Can't Find Process Where Id={processId}");
                string[] fields = result.Split(' ');
                parentId = Convert.ToInt32(fields[3]);
            }
            return parentId;
        }

        public static IDictionary<int, int> GetProcessIdAndParentProcessIdList(IEnumerable<int> processIdList)
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

        public static IDictionary<int, int> GetProcessIdAndParentProcessIdList(IEnumerable<Process> processList) => GetProcessIdAndParentProcessIdList(processList.Select(row => row.Id));

        public static List<int> GetProcessesTree(params int[] parentIdArray)
        {
            var processDic = new Dictionary<int, int>();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                using var searcher = new ManagementObjectSearcher($"SELECT ProcessId,ParentProcessId FROM Win32_Process");
                foreach (ManagementObject obj in searcher.Get().Cast<ManagementObject>())
                {
                    var ppid = Convert.ToInt32(obj["ParentProcessId"]);
                    var pid = Convert.ToInt32(obj["ProcessId"]);
                    processDic.Add(pid, ppid);
                }

            }
            else
            {
                using var reader = new StreamReader($"ps -elf");
                _= reader.ReadLine();//skip titleLine
                var result = reader.ReadToEnd();
                string[] fields = result.Split('\n');
                foreach (string field in fields)
                {
                    var row = field.Split(' ');
                    var pid = Convert.ToInt32(row[3]);
                    var ppid = Convert.ToInt32(row[4]);
                    processDic.Add(pid, ppid);
                }
            }
            var ret = new List<int>();
            foreach (var parentId in parentIdArray)
            {
                if (!processDic.TryGetValue(parentId, out _)) continue;

                List<int> pPId = [parentId];
                ret.Add(parentId);
                while (true)
                {
                    var list = processDic.Where(r => pPId.Contains(r.Value)).ToDictionary();
                    if (list.Count==0) break;
                    ret.AddRange(list.Keys);
                    pPId= [.. list.Keys];
                }
            }
            return ret.Distinct().ToList();
        }

    }
}
