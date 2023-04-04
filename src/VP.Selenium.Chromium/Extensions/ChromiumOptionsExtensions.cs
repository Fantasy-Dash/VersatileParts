using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace VP.Selenium.Chromium.Extensions
{
    //todo 注释
    public static class ChromiumOptionsExtensions
    {

        public static ChromeOptions GetDefaultChromiumOptions()
        {
            var opt = new ChromeOptions();
            opt.AddArgument("--disable-popup-blocking");//禁用弹出拦截
            opt.AddArgument("--no-sandbox");//沙盒模式运行
            opt.AddArgument("disable-extensions");//禁用扩展程序
            opt.AddArgument("no-default-browser-check");//不检查默认浏览器
            return opt;
        }

        public static ChromeOptions AddHeadlessArgument(this ChromeOptions opt)
        {
            opt.AddArgument("--headless");//无界面
            return opt;
        }

        public static ChromeOptions Add1080ResolutionArgument(this ChromeOptions opt)
        {
            opt.AddArgument("window-size=1920,1080");//设置窗口尺寸
            return opt;
        }

        public static ChromeOptions AddHardDriveCache(this ChromeOptions opt)
        {
            opt.AddArgument("--disable-dev-shm-usage");
            return opt;
        }

        public static ChromeOptions AddAutomationArgument(this ChromeOptions opt)
        {
            opt.AddExcludedArgument("enable-automation");//通知用户其浏览器正由自动化测试控制
            opt.AddArgument("--disable-blink-features=AutomationControlled");//移除模拟器特征
            return opt;
        }
    }
}
