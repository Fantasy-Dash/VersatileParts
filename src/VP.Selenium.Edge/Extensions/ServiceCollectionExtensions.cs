using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenQA.Selenium.Edge;
using VP.Selenium.Contracts.Services;
using VP.Selenium.Edge.Services;

namespace VP.Selenium.Edge.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVPEdgeDriver(this IServiceCollection services)
        {
            services.TryAddSingleton<IWebDriverService<EdgeDriver, EdgeDriverService>, EdgeWebDriverService>();
            return services;
        }
    }
}
