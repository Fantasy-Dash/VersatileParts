using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WindowsAPICodePack.Taskbar;
using VP.Common.Extensions;
using VP.Common.Services.Interface;
using VP.Windows.Services;

namespace VP.Windows.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVPWindowsService(this IServiceCollection services)
        {
            if (OperatingSystem.IsWindows())
            {
                services.TryAddSingleton<ISystemService, SystemService>();
                services.TryAddSingleton<IProcessService, ProcessService>();
                services.TryAddSingleton(TaskbarManager.Instance);
            }
            else
                services.AddVPOSPlatformService();
            return services;
        }
    }
}
