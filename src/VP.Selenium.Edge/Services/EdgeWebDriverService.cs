﻿using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using Serilog;
using System.Collections.ObjectModel;
using VP.Common.Services.Interface;
using VP.Selenium.Contracts.Services;

namespace VP.Selenium.Edge.Services
{
    /// <summary>
    /// 浏览器驱动服务
    /// </summary>
    public class EdgeWebDriverService : IWebDriverService<EdgeDriver, EdgeDriverService>, IDisposable
    {
        public Dictionary<string, EdgeDriver> Drivers { get; } = new();
        private static readonly object _lock = new();
        private readonly Dictionary<EdgeDriver, DriverService> _driverDic = new();
        private readonly IProcessService _processService;

        public EdgeWebDriverService(IProcessService processService)
        {
            _processService =processService;
        }

        public Task<EdgeDriver> CreateAsync(string browserName, DriverOptions driverOptions, DriverService? driverService = null, bool isHideCommandWindow = true, TimeSpan? commandTimeout = null)
        {
            return Task.Run(() =>
               {
                   lock (_lock)
                   {
                       driverService??=EdgeDriverService.CreateDefaultService();
                       driverService.HideCommandPromptWindow=isHideCommandWindow;
                       if (driverOptions is not EdgeOptions)
                           throw new ArgumentException($"参数:{nameof(driverOptions)}的类型必须为:{nameof(EdgeOptions)}");
                       commandTimeout??=TimeSpan.FromMinutes(1);
                       var driver = new EdgeDriver((EdgeDriverService)driverService, (EdgeOptions)driverOptions, commandTimeout.Value);
                       Log.Information("浏览器初始化完成");
                       var capabilitity = (Dictionary<string, object>)driverOptions.ToCapabilities().GetCapability("ms:edgeOptions");
                       _=capabilitity.TryGetValue("args", out var args);
                       _=capabilitity.TryGetValue("prefs", out var prefs);
                       if (args!=null && ((ReadOnlyCollection<string>)args).Contains("--headless"))
                           if (prefs!=null&&((Dictionary<string, object>)prefs).TryGetValue("download.default_directory", out var downloadPath))
                               driver.ExecuteCdpCommand("Page.setDownloadBehavior", new() { { "behavior", "allow" }, { "downloadPath", downloadPath } });
                       Drivers.Add(browserName, driver);
                       _driverDic.Add(driver, driverService);
                       return driver;
                   }
               });
        }

        public EdgeDriver? GetDriver(string browserName)
        {
            if (Drivers.TryGetValue(browserName, out var driver))
                return driver;
            return null;
        }

        public EdgeDriverService? GetService(EdgeDriver driver)
        {
            if (_driverDic.TryGetValue(driver, out var driverService))
                return driverService as EdgeDriverService;
            return null;
        }

        public EdgeDriver? ChangeDriverName(string oldBrowserName, string newBrowserName)
        {
            lock (_lock)
            {
                if (!Drivers.TryGetValue(oldBrowserName, out _)) return null;
                Drivers.Add(newBrowserName, Drivers[oldBrowserName]);
                Drivers.Remove(oldBrowserName);
                return GetDriver(newBrowserName);
            }
        }

        public void ClearExceptionProcess()
        {

        }

        public void Dispose()
        {
            lock (_lock)
            {
                _driverDic.Keys.Distinct().AsParallel().ForAll(item => item.Quit());
                _driverDic.Values.Distinct().AsParallel().ForAll(item => item.Dispose());
                _driverDic.Clear();
                Drivers.Clear();
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose(string browserName)
        {
            lock (_lock)
            {
                if (Drivers.TryGetValue(browserName, out var driver))
                {
                    driver.Quit();
                    driver.Dispose();
                    _driverDic[driver].Dispose();
                    _driverDic.Remove(driver);
                    Drivers.Remove(browserName);
                }
            }
        }
    }
}
