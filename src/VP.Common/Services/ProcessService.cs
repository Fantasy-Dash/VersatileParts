﻿using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;
using VP.Common.Services.Interface;

namespace VP.Common.Services
{
    [UnsupportedOSPlatform("windows")]
    public class ProcessService : IProcessService
    {

        public virtual IEnumerable<Process> GetFiltedByCommandLine(IEnumerable<Process> processes, string filter)
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
}