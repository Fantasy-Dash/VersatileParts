using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using VP.Common.Services;
using VP.Common.Services.Interface;

namespace VP.Common.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        [UnsupportedOSPlatform("windows")]
        public static ServiceCollection AddVPOSPlatformService(this ServiceCollection services)
        {
            services.TryAddSingleton<ISystemService, SystemService>();
            services.TryAddSingleton<IProcessService, ProcessService>();
            return services;
        }
    }
}
