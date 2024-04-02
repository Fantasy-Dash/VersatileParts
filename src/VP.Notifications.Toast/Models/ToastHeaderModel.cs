using System.Text;

namespace VP.Notifications.Toast.Models
{
    /// <summary>
    /// 通知头模型 用于通知分组 设置标题
    /// </summary>
    public class ToastHeaderModel(string id, string title)
    {

        /// <summary>
        /// 通知头Id 用来分组
        /// </summary>
        public string Id { get; set; } = id;

        /// <summary>
        /// 通知标题
        /// 以最新一条通知为准
        /// 如最新一条通知被删除则使用次新通知的属性
        /// </summary>
        public string Title { get; set; } = title;

        /// <summary>
        /// 通知头参数
        /// 以最新一条通知为准
        /// 如最新一条通知被删除则使用次新通知的属性
        /// </summary>
        public Dictionary<string, string>? Arguments { get; set; }


        public string GetArgumentString()
        {
            if (Arguments != null)
            {
                var sb = new StringBuilder();
                foreach (var arg in Arguments)
                    sb.Append(arg.Key + "=" + arg.Value + "&");
                sb.Remove(sb.Length - 1, 1);
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
