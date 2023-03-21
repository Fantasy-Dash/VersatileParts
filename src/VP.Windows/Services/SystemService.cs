using System.Management;
using System.Runtime.Versioning;

namespace VP.Windows.Services
{
    /// <summary>
    /// 系统服务
    /// </summary>
    [SupportedOSPlatform("windows")]
    public class SystemService : Common.Services.SystemService
    {
        public override DateTime GetSystemInstallTime()
        {
            // 获取Windows系统安装时间
            var scope = new ManagementScope(@"\\localhost\root\cimv2");
            var query = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            using var searcher = new ManagementObjectSearcher(scope, query);
            using var queryCollection = searcher.Get();
            foreach (var m in queryCollection)
            {
                if (m["InstallDate"].ToString()!=null)
                    return ManagementDateTimeConverter.ToDateTime(m["InstallDate"]?.ToString() ?? string.Empty);
            }
            return DateTime.MinValue;
        }

        public override string GetUUID()
        {
            string uuid = string.Empty;
            var query = new SelectQuery("Win32_ComputerSystemProduct");
            using var searcher = new ManagementObjectSearcher(query);
            using var queryCollection = searcher.Get();
            foreach (var item in queryCollection)
                return item["UUID"]?.ToString()??string.Empty;
            return uuid;
        }
    }
}
