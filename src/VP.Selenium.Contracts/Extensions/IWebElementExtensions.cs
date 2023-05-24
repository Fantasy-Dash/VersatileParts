﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;
using System.Text;

namespace VP.Selenium.Contracts.Extensions
{
    /// <summary>
    /// web元素扩展
    /// </summary>
    public static class IWebElementExtensions
    {
        /// <summary>
        /// 等待 <see cref="IWebElement"/> 就绪
        /// </summary>
        /// <param name="element">以此元素开始查询子元素</param>
        /// <param name="wait">等待器</param>
        /// <param name="elementIdentifier">筛选器</param>
        /// <param name="token">可供取消等待的Token</param>
        /// <returns>返回找到的单个 <see cref="IWebElement"/></returns>
        public static IWebElement WaitElement(this IWebElement element, WebDriverWait wait, By elementIdentifier, CancellationToken token = default)
        {
            return wait.Until(d => element.FindElement(elementIdentifier), token);
        }
        /// <inheritdoc cref="WaitElement(IWebElement, WebDriverWait, By, CancellationToken)"/>
        public static IWebElement WaitElement(this IWebElement element, By elementIdentifier, WebDriverWait wait, CancellationToken token = default)
        {
            return element.WaitElement(wait, elementIdentifier, token);
        }

        //todo注释
        public static string GetText(this IWebElement element)
        {
            if (element is IWrapsDriver e)
            {
                e.WrappedDriver.ExecuteJavaScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'end', inline: 'nearest'});", element);
                if (string.IsNullOrWhiteSpace(element.Text))
                {
                    var sb = new StringBuilder();
                    element.FindElements(By.XPath("./*")).ToList().ForEach(row => sb.Append(row.Text));
                    return sb.ToString();
                }
                else
                    return element.Text;
            }
            else
                throw new NotImplementedException($"{nameof(element)}不支持获取WrappedDriver");
        }

        /// <summary>
        /// 使用JavaScript方法点击元素
        /// </summary>
        /// <param name="element">页面元素</param>
        /// <param name="driver">web驱动</param>
        public static void ClickSafely(this IWebElement element)
        {
            if (element is IWrapsDriver e)
            {
                e.WrappedDriver.ExecuteJavaScript("arguments[0].scrollIntoView({behavior: 'instant', block: 'end', inline: 'nearest'});", element);
                e.WrappedDriver.ExecuteJavaScript("arguments[0].click();", element);
            }
            else
                throw new NotImplementedException($"{nameof(element)}不支持获取WrappedDriver");
        }

        /// <summary>
        /// 等待列表元素
        /// </summary>
        /// <param name="element">以此元素开始查询子元素</param>
        /// <param name="wait">等待器</param>
        /// <param name="elementIdentifier">筛选器</param>
        /// <param name="needCount">需要的元素最小数量</param>
        /// <param name="token">可供取消等待的Token</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> WaitElementList(this IWebElement element, By elementIdentifier, WebDriverWait wait, int needCount = 0, CancellationToken token = default)
        {
            return wait.Until(d =>
            {
                //TODO 等待个数待调试 调试完成后 重新编写方法注释
                if (element.FindElements(elementIdentifier).Count >= needCount)
                {
                    return element.FindElements(elementIdentifier);
                }
                throw new NoSuchElementException();
            }, token);
        }
    }
}
