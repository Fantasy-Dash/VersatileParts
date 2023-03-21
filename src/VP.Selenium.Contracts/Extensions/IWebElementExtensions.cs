using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Collections.ObjectModel;

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
        /// <returns>返回找到的单个 <see cref="IWebElement"/></returns>
        public static IWebElement WaitElement(this IWebElement element, WebDriverWait wait, By elementIdentifier)
        {
            return wait.Until(d => element.FindElement(elementIdentifier));
        }

        /// <summary>
        /// 等待列表元素
        /// </summary>
        /// <param name="element">以此元素开始查询子元素</param>
        /// <param name="wait">等待器</param>
        /// <param name="elementIdentifier">筛选器</param>
        /// <param name="needCount">需要的元素最小数量</param>
        /// <returns></returns>
        public static ReadOnlyCollection<IWebElement> WaitElementList(this IWebElement element, By elementIdentifier, WebDriverWait wait, int needCount = 0)
        {
            return wait.Until(d =>
            {
                //TODO 等待个数待调试 调试完成后 重新编写方法注释
                if (element.FindElements(elementIdentifier).Count >= needCount)
                {
                    return element.FindElements(elementIdentifier);
                }
                throw new NoSuchElementException();
            });
        }
    }
}
