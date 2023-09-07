using AngleSharp.Attributes;
using AngleSharp.Common;
using AngleSharp.Dom;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V85.Profiler;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Reflection;

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

        public static void StopPageLoading(this WebDriverWait wait)
        {

            wait.Until(driver =>
            {
                driver.StopPageLoading();
                return true;
            });
        }

        public static async Task WaitPageLoaded(this WebDriverWait wait)
        {
            await wait.Until(async driver =>
            {
                while (!driver.GetPageReady())
                    await Task.Delay(250);
                return true;
            });
        }
    }
}
