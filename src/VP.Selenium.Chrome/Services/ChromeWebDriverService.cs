using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VP.Common.Services.Interface;
using VP.Selenium.Contracts.Services;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace VP.Selenium.Chrome.Services
{
    /// <summary>
    /// 浏览器驱动服务
    /// </summary>
    public class ChromeWebDriverService : IWebDriverService<ChromeDriver, ChromeDriverService, ChromeOptions>, IDisposable
    {
        public Dictionary<string, ChromeDriver> Drivers { get; } = new();
        private static readonly object _lock = new();
        private readonly Dictionary<ChromeDriver, ChromeDriverService> _driverDic = new();
        private readonly IProcessService _processService;

        public ChromeWebDriverService(IProcessService processService)
        {
            _processService=processService;
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    _ = new DriverManager(Path.GetFullPath("WebDriver",
                                                           AppDomain.CurrentDomain.BaseDirectory))
                  .SetUpDriver(new ChromeConfig(),
                               VersionResolveStrategy.MatchingBrowser);
                    break;
                }
                catch { }
                Task.Delay(500).GetAwaiter().GetResult();
            }
        }

        public Task<ChromeDriver> CreateAsync(string browserName, DriverOptions driverOptions, DriverService? driverService = null, bool isHideCommandWindow = true)
        {
            //使用第三方包配置驱动
            return Task.Run(() =>
               {
                   lock (_lock)
                   {
                       var service = ChromeDriverService.CreateDefaultService();
                       service.HideCommandPromptWindow = isHideCommandWindow;
                       if (driverOptions is not ChromeOptions)
                           throw new ArgumentException($"参数:{nameof(driverOptions)}的类型必须为:{nameof(ChromeOptions)}");
                       var driver = new ChromeDriver(service, (ChromeOptions)driverOptions);
                       var capabilitity = (Dictionary<string, object>)driverOptions.ToCapabilities().GetCapability("goog:chromeOptions");
                       _=capabilitity.TryGetValue("args", out var args);
                       _=capabilitity.TryGetValue("prefs", out var prefs);
                       if (args!=null && ((ReadOnlyCollection<string>)args).Contains("--headless"))
                           if (prefs!=null&&((Dictionary<string, object>)prefs).TryGetValue("download.default_directory", out var downloadPath))
                               driver.ExecuteCdpCommand("Page.setDownloadBehavior", new() { { "behavior", "allow" }, { "downloadPath", downloadPath } });
                       try
                       {
                           Drivers.Add(browserName, driver);
                           _driverDic.Add(driver, service);
                           return driver;
                       }
                       catch
                       {
                           driver.Quit();
                           service.Dispose();
                           throw;
                       }
                   }
               });
        }

        public ChromeDriver? GetDriver(string browserName)
        {
            if (Drivers.TryGetValue(browserName, out var driver))
                return driver;
            return null;
        }

        public ChromeDriverService? GetService(ChromeDriver driver)
        {
            if (_driverDic.TryGetValue(driver, out var driverService))
                return driverService;
            return null;
        }

        public ChromeDriver? ChangeDriverName(string oldBrowserName, string newBrowserName)
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
            var processList = Process.GetProcesses();
            var chromeTestProcessList = _processService.GetFiltedByCommandLine(Process.GetProcesses(), "--test-type=webdriver");
            var processIdAndParentProcessIdList = _processService.GetProcessIdAndParentProcessIdList(chromeTestProcessList).ToList();
            foreach (var item in processIdAndParentProcessIdList)
            {
                var parentProcess = processList.FirstOrDefault(p => p.Id.Equals(item.Value));
                if (parentProcess is null||parentProcess.Id<=4 ||parentProcess.HasExited)
                {
                    var process = processList.FirstOrDefault(p => p.Id.Equals(item.Key));

                    if (process!=null)
                    {
                        process.Refresh();
                        if (!process.HasExited)
                            process.Kill();
                    }
                }
            };
            processIdAndParentProcessIdList=null;
            chromeTestProcessList=null;
            processList=null;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _driverDic.Keys.Distinct().AsParallel().ForAll(item => item.Quit());
                _driverDic.Values.Distinct().AsParallel().ForAll(item => item.Dispose());
                _driverDic.Clear();
                Drivers.Clear();
                ClearExceptionProcess();
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
                    var serviceCount = _driverDic.Count(row => row.Value.Equals(_driverDic[driver]));
                    if (serviceCount<=1)
                        _driverDic[driver].Dispose();
                    _driverDic.Remove(driver);
                    Drivers.Remove(browserName);
                    ClearExceptionProcess();
                }
            }
        }
    }
}
