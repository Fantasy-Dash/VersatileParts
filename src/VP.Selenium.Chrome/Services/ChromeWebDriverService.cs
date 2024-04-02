using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VP.Common.Helpers;
using VP.Selenium.Contracts.Services;

namespace VP.Selenium.Chrome.Services
{
    /// <summary>
    /// 浏览器驱动服务
    /// </summary>
    public class ChromeWebDriverService() : IWebDriverService<ChromeDriver, ChromeDriverService>, IDisposable
    {
        public Dictionary<string, ChromeDriver> Drivers { get; } = [];
        private static readonly object _lock = new();
        private readonly Dictionary<ChromeDriver, DriverService> _driverDic = [];

        public Task<ChromeDriver> CreateAsync(string browserName, DriverOptions driverOptions, DriverService? driverService = null, bool isHideCommandWindow = true, TimeSpan? commandTimeout = null)
        {
            return Task.Run(() =>
               {
                   lock (_lock)
                   {
                       driverService??=ChromeDriverService.CreateDefaultService();
                       driverService.HideCommandPromptWindow=isHideCommandWindow;
                       if (driverOptions is not ChromeOptions)
                           throw new ArgumentException($"参数:{nameof(driverOptions)}的类型必须为:{nameof(ChromeOptions)}");
                       commandTimeout??=TimeSpan.FromMinutes(1);
                       var driver = new ChromeDriver((ChromeDriverService)driverService, (ChromeOptions)driverOptions, commandTimeout.Value);
                       var capabilitity = (Dictionary<string, object>)driverOptions.ToCapabilities().GetCapability("goog:chromeOptions");
                       _=capabilitity.TryGetValue("args", out var args);
                       _=capabilitity.TryGetValue("prefs", out var prefs);
                       if (args!=null && ((ReadOnlyCollection<string>)args).Where(r => r.StartsWith("--headless")).Any())
                           if (prefs!=null&&((Dictionary<string, object>)prefs).TryGetValue("download.default_directory", out var downloadPath))
                               driver.ExecuteCdpCommand("Page.setDownloadBehavior", new() { { "behavior", "allow" }, { "downloadPath", downloadPath } });
                       try
                       {
                           Drivers.Add(browserName, driver);
                           _driverDic.Add(driver, driverService);
                           return driver;
                       }
                       catch
                       {
                           driver.Quit();
                           driverService.Dispose();
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
                return driverService as ChromeDriverService;
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

        public void Dispose()
        {
            lock (_lock)
            {
                var tree = new List<int>();
                _driverDic.Values.ToList().ForEach(r =>
                {
                    if (r.ProcessId!=0)
                        tree.AddRange(ProcessHelper.GetProcessesTree(r.ProcessId));
                });
                _driverDic.Keys.Distinct().AsParallel().ForAll(item => item.Quit());
                _driverDic.Values.Distinct().AsParallel().ForAll(item => item.Dispose());
                tree.AddRange(ProcessHelper.GetProcessesTree([.. tree]).Distinct().ToList());
                foreach (var item in tree)
                    try
                    {
                        using var proc = Process.GetProcessById(item);
                        proc.Kill();
                    }
                    catch
                    {
                    }
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
                    var pid = _driverDic[driver].ProcessId;
                    var tree = ProcessHelper.GetProcessesTree(pid);
                    driver.Quit();
                    tree.AddRange(ProcessHelper.GetProcessesTree([.. tree]).Distinct().ToList());
                    foreach (var item in tree)
                        try
                        {
                            using var proc = Process.GetProcessById(item);
                            proc.Kill();
                        }
                        catch
                        {
                        }
                    _driverDic[driver].Dispose();
                    _driverDic.Remove(driver);
                    Drivers.Remove(browserName);
                }
            }
        }
    }
}
