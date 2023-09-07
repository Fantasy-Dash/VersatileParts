﻿using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text.Json;
using VP.Selenium.Contracts.Models;

namespace VP.Selenium.Contracts.Extensions
{
    public static class IWebDriverExtensions
    {
        /// <inheritdoc cref="IWebElementExtensions.WaitElement(IWebElement, WebDriverWait, By)"/>
        /// <param name="webDriver">Web驱动</param>
        public static IWebElement WaitElement(this IWebDriver webDriver, WebDriverWait wait, By elementIdentifier)
        {
            return wait.Until(d => webDriver.FindElement(elementIdentifier));
        }

        /// <inheritdoc cref="IWebElementExtensions.WaitElementList(IWebElement, By, WebDriverWait, int)"/>
        /// <param name="webDriver">Web驱动</param>
        public static ReadOnlyCollection<IWebElement> WaitElementList(this IWebDriver webDriver, By elementIdentifier, WebDriverWait wait, int needCount = 0)
        {
            return wait.Until(d =>
            {
                //TODO 等待个数待调试 调试完成后 重新编写方法注释
                if (webDriver.FindElements(elementIdentifier).Count >= needCount)
                {
                    return webDriver.FindElements(elementIdentifier);
                }
                throw new NoSuchElementException();
            });
        }

        public static void StopPageLoading(this IWebDriver webDriver)
        {
            webDriver.ExecuteJavaScript("window.stop();");
        }

        public static bool GetPageReady(this IWebDriver webDriver)
        {
            var performanceLogs = new List<LogEntry>();
            performanceLogs.AddRange(webDriver.Manage().Logs.GetLog(LogType.Performance));
            var loadingFrameIdList = new List<string>();
            foreach (var logEntry in performanceLogs)
            {
                var log = JsonSerializer.Deserialize(logEntry.Message,
                                                     typeof(LogEntityMessageModel),
                                                     SourceGenerationContext.Default)
                                                as LogEntityMessageModel;
                if (log?.Message is null) continue;
                if (log?.Message?.Method?.Contains("Page.frameStoppedLoading")==true)
                {
                    loadingFrameIdList.RemoveAll(row => row.Equals(log?.Message?.Params?["frameId"]));
                    continue;
                }
                if (log?.Message?.Method?.Contains("Page.frameStartedLoading")==true
                    ||log?.Message?.Method?.Contains("Page.loadEventFired")==true)
                {
                    var frameId = log?.Message?.Params?["frameId"];
                    if (!string.IsNullOrWhiteSpace(frameId))
                        loadingFrameIdList.Add(frameId);
                    continue;
                }
            }
            if (loadingFrameIdList.Count>0)
                return false;
            return true;
        }

    }
}
