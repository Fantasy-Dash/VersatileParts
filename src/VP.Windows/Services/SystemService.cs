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
                var dateTimeStr = m["InstallDate"].ToString();
                if (!string.IsNullOrWhiteSpace(dateTimeStr))
                    return ManagementDateTimeConverter.ToDateTime(dateTimeStr);
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
                if (item["UUID"] != null)
                    uuid= item["UUID"].ToString()??string.Empty;
            if (string.IsNullOrWhiteSpace(uuid))
                throw new ManagementException();
            return uuid;
        }
    }
}
