﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenQA.Selenium.Chrome;
using VP.Selenium.Chrome.Services;
using VP.Selenium.Contracts.Services;

namespace VP.Selenium.Chrome.Extensions
{
    //todo 注释
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVPChromeDriver(this IServiceCollection services)
        {
            services.TryAddSingleton<IWebDriverService<ChromeDriver, ChromeDriverService>, ChromeWebDriverService>();
            return services;
        }
    }
}
