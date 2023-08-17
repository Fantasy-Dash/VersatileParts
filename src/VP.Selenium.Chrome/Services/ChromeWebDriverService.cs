using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chromium;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VP.Common.Helpers;
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
    public class ChromeWebDriverService : IWebDriverService<ChromeDriver, ChromeDriverService>, IDisposable
    {
        public Dictionary<string, ChromeDriver> Drivers { get; } = new();
        private static readonly object _lock = new();
        private readonly Dictionary<ChromeDriver, DriverService> _driverDic = new();
        private readonly IProcessService _processService;
        private readonly string _processName = "chrome";

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
            return Task.Run(() =>
               {
                   lock (_lock)
                   {
                       driverService??= ChromeDriverService.CreateDefaultService();
                       driverService.HideCommandPromptWindow=isHideCommandWindow;
                       if (driverOptions is not ChromeOptions)
                           throw new ArgumentException($"参数:{nameof(driverOptions)}的类型必须为:{nameof(ChromeOptions)}");
                       var driver = new ChromeDriver((ChromeDriverService)driverService, (ChromeOptions)driverOptions);
                       var capabilitity = (Dictionary<string, object>)driverOptions.ToCapabilities().GetCapability("goog:chromeOptions");
                       _=capabilitity.TryGetValue("args", out var args);
                       _=capabilitity.TryGetValue("prefs", out var prefs);
                       if (args!=null && ((ReadOnlyCollection<string>)args).Contains("--headless"))
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

        public void ClearExceptionProcess()
        {
            var needKillProcessList = new List<Process>();
            var currentProcessList = Process.GetProcesses();
            var processIdTree = new List<Process>();
            var a = currentProcessList.Where(row => row.ProcessName.Equals(_processName));
            var processIdAndParentProcessIdList = _processService.GetProcessIdAndParentProcessIdList(a).ToList();
            foreach (var item in processIdAndParentProcessIdList)
            {
                var processId = item.Key;
                var parentProcessId = item.Value;
            reScanProcess:
                var process = currentProcessList.FirstOrDefault(row => row.Id.Equals(processId));
                var parentProcess = currentProcessList.FirstOrDefault(row => row.Id.Equals(parentProcessId));
                if (parentProcess is null||parentProcess.Id<=4||parentProcess.HasExited)
                {
                    if (process!=null&&!process.HasExited)
                        needKillProcessList.Add(process);
                }
                else if (parentProcess.ProcessName.Equals(_processName))
                {
                    if (process!=null&&!process.HasExited)
                        processIdTree.Add(process);
                    processId = parentProcess.Id;

                    var parent = _processService.GetParentProcessId(parentProcess.Id);
                    if (parent!=0)
                    {
                        parentProcessId = parent;
                        processIdTree.Add(parentProcess);
                        goto reScanProcess;
                    }
                    else
                        needKillProcessList.Add(parentProcess);
                }
                else
                    processIdTree.Clear();
            };
            needKillProcessList.AddRange(processIdTree);
            needKillProcessList.Distinct().ToList().ForEach(row => row.Kill());
            processIdAndParentProcessIdList=null;
            processIdTree=null;
            needKillProcessList =null;
            currentProcessList=null;
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
                    _driverDic[driver].Dispose();
                    _driverDic.Remove(driver);
                    Drivers.Remove(browserName);
                    ClearExceptionProcess();
                }
            }
        }
    }
}
