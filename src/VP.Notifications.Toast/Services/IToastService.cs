using CommunityToolkit.WinUI.Notifications;
using VP.Notifications.Toast.Models;
using Windows.UI.Notifications;

namespace VP.Notifications.Toast.Services
{
    /// <summary>
    /// Toast通知服务
    /// </summary>
    public interface IToastService
    {
        //todo 注释
        public ToastNotificationHistoryCompat? History { get; }

        public ToastNotifierCompat? Notifier { get; }

        /// <summary>
        /// 新增一条通知
        /// </summary>
        /// <param name="toastContent"></param>
        /// <param name="scenario"></param>
        /// <param name="toastHeader"></param>
        public void Add(ToastContentModel toastContent, ToastScenario scenario, ToastHeaderModel? toastHeader = null);

        /// <summary>
        /// 更新进度条通知的进度
        /// </summary>
        /// <param name="id"></param>
        /// <param name="group"></param>
        /// <param name="status"></param>
        /// <param name="percent"></param>
        /// <param name="valueString"></param>
        /// <returns></returns>
        public NotificationUpdateResult UpdateProgress(string id, string group = ToastConst.DefaultGroup, string? title = "", string status = "", string percent = "0", string valueString = "");

        /// <summary>
        /// 移除通知
        /// </summary>
        /// <param name="id"></param>
        /// <param name="group"></param>
        public void Remove(string id, string group = ToastConst.DefaultGroup);

        /// <summary>
        /// 移除通知
        /// </summary>
        /// <param name="group"></param>
        public void RemoveGroup(string group);

        /// <summary>
        /// 移除所有通知
        /// </summary>
        public void ClearAll(bool IsSkipExpireable = false);
    }
}
