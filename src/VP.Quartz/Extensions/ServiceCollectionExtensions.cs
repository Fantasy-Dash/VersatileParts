using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using VP.Quartz.Services;
using VP.Quartz.Services.Interface;

namespace VP.Quartz.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollection AddVPQuartz(this ServiceCollection services, QuartzOptions? options = null)
        {
            if (!services.Any(row => row.ServiceType.Equals(typeof(ISchedulerFactory))))
            {
                if (options is not null)
                    services.Configure<QuartzOptions>(option => option= options);
                services.AddQuartz(q => q.UseMicrosoftDependencyInjectionJobFactory());
            }
            services.TryAddSingleton<IQuartzService, QuartzService>();
            return services;
        }
    }
}
