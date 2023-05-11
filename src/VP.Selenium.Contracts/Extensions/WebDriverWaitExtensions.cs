using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

namespace VP.Selenium.Contracts.Extensions
{
    /// <summary>
    /// Web驱动等待扩展
    /// </summary>
    public static class WebDriverWaitExtensions
    {
        /// <inheritdoc cref="IWebElementExtensions.WaitElement(IWebElement, WebDriverWait, By)"/>
        public static IWebElement WaitElement(this WebDriverWait wait, By elementIdentifier) =>
            wait.Until(d => d.FindElement(elementIdentifier));

        /// <inheritdoc cref="IWebElementExtensions.WaitElementList(IWebElement, By, WebDriverWait, int)"/>
        public static IEnumerable<IWebElement> WaitElementList(this WebDriverWait wait, By elementIdentifier, int needCount = 1)
        {
            return wait.Until(d =>
            {
                //todo 等待个数待调试 调试完成后 重新编写方法注释
                if (d.FindElements(elementIdentifier).Count >= needCount)
                {
                    return d.FindElements(elementIdentifier);
                }
                throw new NoSuchElementException();
            });
        }

        /// <inheritdoc cref="IWebElementExtensions.WaitElementList(IWebElement, By, WebDriverWait, int)"/>
        public static IEnumerable<IWebElement> WaitDisplayedElementList(this WebDriverWait wait, By elementIdentifier, int needCount = 1)
        {
            return wait.Until(d =>
            {
                //todo 等待个数待调试 调试完成后 重新编写方法注释
                if (d.FindElements(elementIdentifier).Where(row=>row.Displayed).Count() >= needCount)
                {
                    return d.FindElements(elementIdentifier).Where(row => row.Displayed);
                }
                throw new NoSuchElementException();
            });
        }

        //todo 注释
        public static IWebElement WaitDisplayedElement(this WebDriverWait wait, By by)
        {
            return wait.Until(d =>
            {
                var targetElements = wait.WaitElementList(by);
                while (!targetElements.Where(row => row.Displayed).Any())
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    targetElements=wait.WaitElementList(by);
                }
                return targetElements.Where(row => row.Displayed).First();
            });

        }
    }
}
