﻿using OpenQA.Selenium;

namespace VP.Selenium.Contracts.Services
{//todo 注释
    /// <summary>
    /// 浏览器驱动服务
    /// </summary>
    public interface IWebDriverService<TWebDriver, TWebDriverService, TDriverOptions> : IDisposable
    {
        public Dictionary<string, TWebDriver> Drivers { get; }

        public Task<TWebDriver> CreateAsync(string browserName, DriverOptions driverOptions, DriverService? driverService = null, bool isHideCommandWindow = true);

        public TWebDriver? GetDriver(string browserName);

        public TWebDriverService? GetService(TWebDriver driver);

        public TWebDriver? ChangeDriverName(string oldBrowserName, string newBrowserName);

        public void ClearExceptionProcess();

        public void Dispose(string browserName);
    }
}