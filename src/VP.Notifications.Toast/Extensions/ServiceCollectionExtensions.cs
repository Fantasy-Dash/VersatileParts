using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VP.Notifications.Toast.Services;

namespace VP.Notifications.Toast.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVPToast(this IServiceCollection services)
        {
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763, 0))//1809
                services.TryAddSingleton<IToastService, ToastService>();
            else
                services.TryAddSingleton<IToastService, NullToastService>();
            return services;
        }
    }
}
