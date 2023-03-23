using System.Diagnostics;

namespace VP.Common.Extensions
{
    /// <summary>
    /// 进程扩展类
    /// </summary>
    public static class ProcessExtensions
    {
        //todo 注释
        /// <summary>
        /// 检查其他同名进程是否存在
        /// </summary>
        /// <param name="switchToOtherProcess"></param>
        /// <returns></returns>
        public static bool IsProcessUnique(this Process process, out Process? otherProcess)
        {
            //查找其他进程 忽略vs调试会查找到的一些空线程进程
            otherProcess = Process.GetProcessesByName(process.ProcessName)
                .Where(row => row.Id != process.Id
                              && !row.HasExited
                              && row.Threads.Count > 0).FirstOrDefault();
            return otherProcess is null;
        }
    }
}
