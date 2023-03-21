using CommunityToolkit.WinUI.Notifications;
using System.Runtime.InteropServices;
using VP.Notifications.Toast.Model;
using Windows.UI.Notifications;

namespace VP.Notifications.Toast.Services
{
    public class ToastService : IToastService
    {
        public ToastNotifierCompat Notifier { get; }

        public ToastService()
        {
            Notifier=ToastNotificationManagerCompat.CreateToastNotifier();
        }

        public ToastNotificationHistoryCompat History => ToastNotificationManagerCompat.History;

        public void Add(ToastContentModel toastContent, ToastScenario scenario, ToastHeaderModel? toastHeader = null)
        {
            var toast = new ToastContentBuilder();
            if (toastContent.CustomTimeStamp != null)
                toast.AddCustomTimeStamp(toastContent.CustomTimeStamp.Value);
            if (toastHeader != null)
                toast.AddHeader(toastHeader.Id, toastHeader.Title, toastHeader.GetArgumentString());
            toast.AddArgument(ToastConst.Id, toastContent.Id);
            toast.AddArgument(ToastConst.Group, toastContent.Group);
            if (toastContent.Args != null)
                foreach (KeyValuePair<string, string> item in toastContent.Args)
                    toast.AddArgument(item.Key, item.Value);
            toast.AddText(toastContent.Title);
            if (!string.IsNullOrWhiteSpace(toastContent.Content))
                toast.AddText(toastContent.Content);
            if (toastContent.VisualChild != null)
                toast.AddVisualChild(toastContent.VisualChild);
            if (!string.IsNullOrWhiteSpace(toastContent.AttributionText))
                toast.AddAttributionText(toastContent.AttributionText);
            if (toastContent.HeroImageUri != null)
                toast.AddHeroImage(toastContent.HeroImageUri);
            if (toastContent.AppLogoOverrideUri != null)
            {
                if (toastContent.AppLogoCrop != null)
                    toast.AddAppLogoOverride(toastContent.AppLogoOverrideUri, toastContent.AppLogoCrop);
                else
                    toast.AddAppLogoOverride(toastContent.AppLogoOverrideUri);
            }
            if (toastContent.ImageUri != null)
                toast.AddInlineImage(toastContent.ImageUri);

            if (toastContent.AudioUri != null)
                toast.AddAudio(toastContent.AudioUri);
            if (toastContent.SelectionBox != null)
                toast.AddToastInput(toastContent.SelectionBox);
            if (!string.IsNullOrWhiteSpace(toastContent.InputTextBoxText))
                toast.AddInputTextBox("tbReply", placeHolderContent: toastContent.InputTextBoxText);
            if (toastContent.InputTextBoxButtonList != null)
                foreach (var button in toastContent.InputTextBoxButtonList)
                    toast.AddButton(button.SetTextBoxId("tbReply"));
            if (toastContent.ButtonList != null)
                foreach (var button in toastContent.ButtonList)
                    toast.AddButton(button);
            if (toastContent.ScheduleDateTime != null)
                toast.Schedule(toastContent.ScheduleDateTime.Value);
            toast.SetToastScenario(scenario);


            var notification = new ToastNotification(toast.GetXml())
            {
                Tag=toastContent.Id,
                Group = toastContent.Group,
                ExpirationTime = toastContent.ExpirationTime
            };
            if (toastContent.Data!=null)
                notification.Data=toastContent.Data;

            Notifier.Show(notification);
        }

        public NotificationUpdateResult UpdateProgress(string id, string group, string title, string status, string percent, string valueString)
        {
            var data = new NotificationData();
            data.Values[ToastConst.ProgressBar.Value] = percent;
            data.Values[ToastConst.ProgressBar.Title]=title;
            data.Values[ToastConst.ProgressBar.Status] = status;
            data.Values[ToastConst.ProgressBar.ValueString] = valueString;
            return Notifier.Update(data, id, group);
        }

        public void ClearAll(bool IsSkipExpireable)
        {
            if (!IsSkipExpireable)
                History.Clear();
            History.GetHistory().ToList().ForEach(row =>
            {
                if (row.ExpirationTime>=DateTimeOffset.Now.AddHours(1))
                {
                    try { Notifier.Hide(row); }
                    catch (COMException) { }
                    History.Remove(row.Tag, row.Group);
                }
            });
        }

        public void Remove(string id, string group)
        {
            var scheduledToast = Notifier.GetScheduledToastNotifications().Where(i => i.Tag.Contains(id) && i.Group.Contains(group)).ToList();
            if (scheduledToast.Count > 0)
                scheduledToast.AsParallel().ForAll(Notifier.RemoveFromSchedule);
            History.Remove(id, group);
        }

        public void RemoveGroup(string group)
        {
            var scheduledToast = Notifier.GetScheduledToastNotifications().Where(i => i.Group.Contains(group)).ToList();
            if (scheduledToast.Count > 0)
                scheduledToast.AsParallel().ForAll(Notifier.RemoveFromSchedule);
            History.RemoveGroup(group);
        }
    }
}