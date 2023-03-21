using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VP.Windows.Wpf.Helper
{
    /// <summary>
    /// 窗体相关
    /// </summary>
    public static partial class WindowHelper
    {//todo 注释
        ///<summary>
        /// 该函数设置由不同线程产生的窗口的显示状态
        /// </summary>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零</returns>
        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        /// <summary>
        ///  该函数将创建指定窗口的线程设置到前台，并且激活该窗口。
        /// </summary>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零</returns>
        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetForegroundWindow(IntPtr hWnd);
        [LibraryImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [LibraryImport("User32.dll")]
        private static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public static bool WindowActive(Process process)
        {
            var windowIdList = EnumWindow(process);
            if (windowIdList.Count>0)
            {
                ShowWindowAsync(windowIdList.First(), 8);//SW_SHOWNA
                return SetForegroundWindow(windowIdList.First());
            }
            return false;
        }

        public static List<IntPtr> EnumWindow(Process process)
        {
            List<IntPtr> hwnds = new();
            uint processId = Convert.ToUInt32(process.Id);
            EnumWindows((IntPtr hWnd, IntPtr lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid == processId)
                    hwnds.Add(hWnd);
                return true;
            }, IntPtr.Zero);
            return hwnds;
        }

    }
}
