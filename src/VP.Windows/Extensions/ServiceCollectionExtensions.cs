using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WindowsAPICodePack.Taskbar;

namespace VP.Windows.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVPWindowsService(this IServiceCollection services)
        {
            services.TryAddSingleton(TaskbarManager.Instance);
            return services;
        }
    }
}
