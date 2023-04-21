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
            opt.AddArgument("--disable-popup-blocking");
            return opt;
        }

        public static ChromeOptions AddHeadlessArgument(this ChromeOptions opt)
        {
            opt.AddArgument("--headless");//无界面
            return opt;
        }

        public static ChromeOptions SetResolutionArgument(this ChromeOptions opt, int x, int y)
        {
            opt.AddArgument($"window-size={x},{y}");//设置窗口尺寸
            return opt;
        }

        public static ChromeOptions AddHardDriveCacheArgument(this ChromeOptions opt)
        {
            opt.AddArgument("--disable-dev-shm-usage");
            return opt;
        }

        public static ChromeOptions SetIgnoreProcessingPDFArgument(this ChromeOptions opt)
        {
            opt.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            return opt;
        }

        public static ChromeOptions SetIgnoreProcessingVideoArgument(this ChromeOptions opt)
        {
            opt.AddUserProfilePreference("plugins.always_open_video_externally", true);
            return opt;
        }

        public static ChromeOptions SetForbiddenDownloadArgument(this ChromeOptions opt)
        {
            opt.AddUserProfilePreference("download_restrictions", 3);
            return opt;
        }

        public static ChromeOptions SetDownloadFileNeverAskArgument(this ChromeOptions opt)
        {
            opt.AddUserProfilePreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream");
            return opt;
        }

        public static ChromeOptions SetDownloadFilePathArgument(this ChromeOptions opt, string downloadPath)
        {
            var path = Path.GetFullPath(downloadPath, AppDomain.CurrentDomain.BaseDirectory);
            opt.AddUserProfilePreference("download.default_directory", path);
            opt.AddUserProfilePreference("download.directory_upgrade", true);
            return opt;
        }

        public static ChromeOptions SetDownloadFilePromptArgument(this ChromeOptions opt, bool isNeedUserConfirm)
        {
            opt.AddUserProfilePreference("download.prompt_for_download", isNeedUserConfirm);
            opt.AddUserProfilePreference("profile.default_content_settings.popups", isNeedUserConfirm?1:0);
            return opt;
        }

        public static ChromeOptions SetSafeBrowsing(this ChromeOptions opt, bool isSafeBrowsing = true)
        {
            if (isSafeBrowsing)
            {
                opt.AddUserProfilePreference("safebrowsing.enabled", true);
                opt.AddUserProfilePreference("safebrowsing.disable_download_protection", false);
            }
            else
            {
                opt.AddUserProfilePreference("safebrowsing.enabled", false);
                opt.AddUserProfilePreference("safebrowsing.disable_download_protection", true);
            }
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
