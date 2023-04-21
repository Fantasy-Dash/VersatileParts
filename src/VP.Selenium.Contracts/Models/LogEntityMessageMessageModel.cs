namespace VP.Selenium.Contracts.Models
{
    public class LogEntityMessageMessageModel
    {
        /// <summary>
        /// 方法
        /// </summary>
        public string? Method { get; set; }

        /// <summary>
        /// 参数
        /// </summary>
        public Dictionary<string, string>? Params { get; set; }

        /// <summary>
        /// web视图
        /// </summary>
        public string? WebView { get; set; }
    }
}
