using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using VP.Common.Services.Interface;
using VP.Selenium.Contracts.Services;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace VP.Selenium.Chrome.Services
{//todo 注释
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

        public async Task<ChromeDriver> CreateAsync(string browserName, DriverOptions driverOptions, DriverService? driverService = null, bool isHideCommandWindow = true)
        {
            //使用第三方包配置驱动
            return await Task.Run(() =>
               {
                   lock (_lock)
                   {
                       var service = ChromeDriverService.CreateDefaultService();
                       service.HideCommandPromptWindow = isHideCommandWindow;
                       if (driverOptions is not ChromeOptions)
                           throw new ArgumentException($"参数:{nameof(driverOptions)}的类型必须为:{nameof(ChromeOptions)}");
                       var driver = new ChromeDriver(service, (ChromeOptions)driverOptions);
                       Drivers.Add(browserName, driver);
                       _driverDic.Add(driver, service);
                       return driver;
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
            var exceptionProcessList = _processService.GetFiltedByCommandLine("--test-type=webdriver");
            exceptionProcessList.ForEach(row => row.Kill());
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
