using CommunityToolkit.WinUI.Notifications;
using VP.Notifications.Toast.Models;
using Windows.UI.Notifications;

namespace VP.Notifications.Toast.Services
{
    /// <summary>
    /// 空通知服务实现
    /// </summary>
    public class NullToastService : IToastService
    {
        public ToastNotificationHistoryCompat? History => null;

        public ToastNotifierCompat? Notifier => null;

        public void Add(ToastContentModel toastContent, ToastScenario scenario, ToastHeaderModel? toastHeader = null) { }

        public void ClearAll(bool IsSkipExpireable) { }

        public void Remove(string id, string group) { }

        public void RemoveGroup(string group) { }

        public NotificationUpdateResult UpdateProgress(string id, string group, string title, string status, string percent, string valueString) => NotificationUpdateResult.Succeeded;
    }
}
