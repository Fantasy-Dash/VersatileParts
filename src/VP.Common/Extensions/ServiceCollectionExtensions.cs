using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VP.Common.Services;
using VP.Common.Services.Interface;

namespace VP.Common.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVPOSPlatformService(this IServiceCollection services)
        {
            services.TryAddSingleton<ISystemService, SystemService>();
            services.TryAddSingleton<IProcessService, ProcessService>();
            return services;
        }
    }
}
