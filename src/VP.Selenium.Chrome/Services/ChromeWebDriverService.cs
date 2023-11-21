using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VP.Common.Services.Interface;
using VP.Selenium.Contracts.Services;

namespace VP.Selenium.Chrome.Services
{
    /// <summary>
    /// 浏览器驱动服务
    /// </summary>
    public class ChromeWebDriverService(IProcessService _processService) : IWebDriverService<ChromeDriver, ChromeDriverService>, IDisposable
    {
        public Dictionary<string, ChromeDriver> Drivers { get; } = [];
        private static readonly object _lock = new();
        private readonly Dictionary<ChromeDriver, DriverService> _driverDic = [];
        private readonly string _processName = "chrome";
        private readonly string _driverName = "chromedriver";

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

        public void ClearExceptionProcess(bool isDisposing = false)
        {
            var needKillProcessIList = new List<Process>();
            var currentProcessList = Process.GetProcesses();
            var processIdTree = new List<Process>();
            var browserProcessList = currentProcessList.Where(row => row.ProcessName.Equals(_processName)||row.ProcessName.Equals(_driverName));
            foreach (var process in browserProcessList)
            {
                var cProcess = process;
            reScanProcess:
                var parentProcessId = _processService.GetParentProcessId(cProcess!.Id);
                var parentProcessName = currentProcessList.FirstOrDefault(r => r.Id.Equals(parentProcessId))?.ProcessName??string.Empty;
                if (parentProcessId<=4)
                {
                    processIdTree.Clear();
                    continue;
                }
                else if (parentProcessName.Equals(_processName))
                {
                    if (process!=null&&!process.HasExited)
                        processIdTree.Add(process);
                    var parentProcess = currentProcessList.FirstOrDefault(r => r.Id.Equals(parentProcessId));
                    if (parentProcess is null)
                    {
                        needKillProcessIList.AddRange(processIdTree);
                        processIdTree.Clear();
                        continue;
                    }
                    else
                    {
                        processIdTree.Add(parentProcess);
                        cProcess = parentProcess;
                        goto reScanProcess;
                    }
                }
                else if (parentProcessName.Equals(_driverName))
                {
                    if (process!=null&&!process.HasExited)
                        processIdTree.Add(process);
                    var pProcessId = _processService.GetParentProcessId(parentProcessId);
                    var parentProcess = currentProcessList.FirstOrDefault(r => r.Id.Equals(pProcessId));
                    if (parentProcess is null||isDisposing)
                    {
                        needKillProcessIList.AddRange(processIdTree);
                        processIdTree.Clear();
                        continue;
                    }
                }
                else if (string.IsNullOrWhiteSpace(parentProcessName))
                {
                    var parentProcess = currentProcessList.FirstOrDefault(r => r.Id.Equals(parentProcessId));
                    if ((parentProcess is null ||parentProcess.HasExited)&&process!=null&&!process.HasExited)
                    {
                        processIdTree.Add(process);
                        needKillProcessIList.AddRange(processIdTree);
                    }
                    processIdTree.Clear();
                }
                else
                    processIdTree.Clear();
            };
            needKillProcessIList.DistinctBy(r => r.Id).ToList().ForEach(row => row.Kill());
            processIdTree=null;
            needKillProcessIList =null;
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
                ClearExceptionProcess(true);
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
