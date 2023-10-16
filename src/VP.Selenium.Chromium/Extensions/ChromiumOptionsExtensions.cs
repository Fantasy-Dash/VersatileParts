using OpenQA.Selenium;
using OpenQA.Selenium.Chromium;

namespace VP.Selenium.Chromium.Extensions
{
    //todo 注释
    public static class ChromiumOptionsExtensions
    {

        public static ChromiumOptions AddDefaultArgument(this ChromiumOptions opt)
        {
            opt.AddArgument("--disable-popup-blocking");//禁用弹出拦截
            opt.AddArgument("--no-sandbox");//沙盒模式运行
            opt.AddArgument("no-default-browser-check");//不检查默认浏览器
            return opt;
        }

        public static ChromiumOptions AddHeadlessArgument(this ChromiumOptions opt, bool isSimulateNotHeadLessArgument = false)
        {
            if (isSimulateNotHeadLessArgument)
                opt.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/119.0.0.0 Safari/537.36");
            opt.AddArgument("--headless");//无界面
            return opt;
        }

        public static ChromiumOptions SetResolutionArgument(this ChromiumOptions opt, int x, int y)
        {
            opt.AddArgument($"window-size={x},{y}");//设置窗口尺寸
            return opt;
        }

        public static ChromiumOptions AddHardDriveCacheArgument(this ChromiumOptions opt)
        {
            opt.AddArgument("--disable-dev-shm-usage");
            return opt;
        }

        public static ChromiumOptions AddFullscreenArgument(this ChromiumOptions opt)
        {
            opt.AddArgument("--start-fullscreen");
            return opt;
        }

        public static ChromiumOptions SetIgnoreProcessingPDFArgument(this ChromiumOptions opt)
        {
            opt.AddUserProfilePreference("plugins.always_open_pdf_externally", true);
            return opt;
        }

        public static ChromiumOptions SetIgnoreProcessingVideoArgument(this ChromiumOptions opt)
        {
            opt.AddUserProfilePreference("plugins.always_open_video_externally", true);
            return opt;
        }

        public static ChromiumOptions SetForbiddenDownloadArgument(this ChromiumOptions opt)
        {
            opt.AddUserProfilePreference("download_restrictions", 3);
            return opt;
        }

        public static ChromiumOptions SetLogPerformance(this ChromiumOptions opt, LogLevel logLevel)
        {
            opt.SetLoggingPreference(LogType.Performance, logLevel);
            return opt;
        }

        public static ChromiumOptions SetDownloadFileNeverAskArgument(this ChromiumOptions opt)
        {
            opt.AddUserProfilePreference("browser.helperApps.neverAsk.saveToDisk", "application/octet-stream");
            return opt;
        }

        public static ChromiumOptions SetDownloadFilePathArgument(this ChromiumOptions opt, string downloadPath)
        {
            var path = Path.GetFullPath(downloadPath, AppDomain.CurrentDomain.BaseDirectory);
            opt.AddUserProfilePreference("download.default_directory", path);
            opt.AddUserProfilePreference("download.directory_upgrade", true);
            return opt;
        }

        public static ChromiumOptions SetDownloadFilePromptArgument(this ChromiumOptions opt, bool isNeedUserConfirm)
        {
            opt.AddUserProfilePreference("download.prompt_for_download", isNeedUserConfirm);
            opt.AddUserProfilePreference("profile.default_content_settings.popups", isNeedUserConfirm ? 1 : 0);
            opt.AddUserProfilePreference("profile.default_content_setting_values.automatic_downloads", isNeedUserConfirm ? 0 : 1);
            return opt;
        }

        public static ChromiumOptions SetSafeBrowsingArgument(this ChromiumOptions opt, bool isSafeBrowsing = true)
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

        public static ChromiumOptions AddDisableAutomationArgument(this ChromiumOptions opt)
        {
            opt.AddExcludedArgument("enable-automation");//通知用户其浏览器正由自动化测试控制
            opt.AddArgument("--disable-blink-features=AutomationControlled");//移除模拟器特征
            return opt;
        }
    }
}
