using CommunityToolkit.WinUI.Notifications;
using Windows.UI.Notifications;

namespace VP.Notifications.Toast.Models
{
    public class ToastContentModel
    {
        public ToastContentModel(string id, string title)
        {
            Id = id;
            Title = title;
        }

        /// <summary>
        /// 通知主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 通知分组
        /// </summary>
        public string? Group { get; set; } = ToastConst.DefaultGroup;

        /// <summary>
        /// 通知存储的数据 一般用于绑定
        /// </summary>
        public NotificationData? Data { get; set; }

        /// <summary>
        /// 自定义时间戳
        /// </summary>
        public DateTime? CustomTimeStamp { get; set; }

        /// <summary>
        /// 替换通知Logo
        /// </summary>
        public Uri? AppLogoOverrideUri { get; set; }

        /// <summary>
        /// 通知Logo裁剪方式
        /// </summary>
        public ToastGenericAppLogoCrop? AppLogoCrop { get; set; }

        /// <summary>
        /// 通知标题(加粗显示的文本)
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 通知内容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 署名 字号比内容稍小 但是显示在图片上方
        /// </summary>
        public string? AttributionText { get; set; }

        /// <summary>
        /// 主图
        /// </summary>
        public Uri? HeroImageUri { get; set; }

        /// <summary>
        /// 内插图片Uri
        /// </summary>
        public Uri? ImageUri { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        /// <summary>
        /// 激活通知传递参数
        /// </summary>
        public Dictionary<string, string>? Args { get; set; }

        /// <summary>
        /// 音频地址
        /// </summary>
        public Uri? AudioUri { get; set; }

        /// <summary>
        /// 自适应内容
        /// </summary>
        public IToastBindingGenericChild? VisualChild { get; set; }

        /// <summary>
        /// 文本输入框文本
        /// </summary>
        public string? InputTextBoxText { get; set; }

        /// <summary>
        /// 文本输入框按钮
        /// </summary>
        public List<ToastButton>? InputTextBoxButtonList { get; set; }

        /// <summary>
        /// 多选框
        /// </summary>
        public ToastSelectionBox? SelectionBox { get; set; }

        /// <summary>
        /// 按钮 最多五个
        /// </summary>
        public List<IToastButton>? ButtonList { get; set; }

        /// <summary>
        /// 定时显示时间
        /// </summary>
        public DateTime? ScheduleDateTime { get; set; }
    }
}
