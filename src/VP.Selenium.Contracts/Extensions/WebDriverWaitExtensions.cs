using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Text.Json;
using VP.Selenium.Contracts.Models;

namespace VP.Selenium.Contracts.Extensions
{
    /// <summary>
    /// Web驱动等待扩展
    /// </summary>
    public static class WebDriverWaitExtensions
    {
        /// <inheritdoc cref="IWebElementExtensions.WaitElement(IWebElement, WebDriverWait, By, CancellationToken)"/>
        public static IWebElement WaitElement(this WebDriverWait wait, By elementIdentifier, CancellationToken token = default) =>
            wait.Until(d => d.FindElement(elementIdentifier), token);

        /// <inheritdoc cref="IWebElementExtensions.WaitElementList(IWebElement, By, WebDriverWait, int, CancellationToken)"/>
        public static IEnumerable<IWebElement> WaitElementList(this WebDriverWait wait, By elementIdentifier, int needCount = 1, CancellationToken token = default)
        {
            return wait.Until(d =>
            {
                //todo 等待个数待调试 调试完成后 重新编写方法注释
                if (d.FindElements(elementIdentifier).Count >= needCount)
                {
                    return d.FindElements(elementIdentifier);
                }
                throw new NoSuchElementException();
            }, token);
        }

        /// <inheritdoc cref="IWebElementExtensions.WaitElementList(IWebElement, By, WebDriverWait, int, CancellationToken)"/>
        public static IEnumerable<IWebElement> WaitDisplayedElementList(this WebDriverWait wait, By elementIdentifier, int needCount = 1, CancellationToken token = default)
        {
            return wait.Until(d =>
            {
                //todo 等待个数待调试 调试完成后 重新编写方法注释
                if (d.FindElements(elementIdentifier).Where(row => row.Displayed).Count() >= needCount)
                {
                    return d.FindElements(elementIdentifier).Where(row => row.Displayed);
                }
                throw new NoSuchElementException();
            }, token);
        }

        //todo 注释
        public static IWebElement WaitDisplayedElement(this WebDriverWait wait, By by, CancellationToken token = default)
        {
            return wait.Until(d =>
            {
                var targetElements = wait.WaitElementList(by, token: token);
                while (!targetElements.Where(row => row.Displayed).Any())
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    targetElements=wait.WaitElementList(by, token: token);
                }
                return targetElements.Where(row => row.Displayed).First();
            }, token);
        }

        public static void WaitPageLoaded(this WebDriverWait wait)
        {
            var performanceLogs = new List<LogEntry>();
            wait.Until(driver =>
            {
                performanceLogs.AddRange(driver.Manage().Logs.GetLog(LogType.Performance));
                var loadingFrameIdList = new List<string>();
                foreach (var logEntry in performanceLogs)
                {
                    var log = JsonSerializer.Deserialize<LogEntityMessageModel>(logEntry.Message);
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
                    throw new Exception();
                return "页面加载完成";
            });
        }
    }
}
