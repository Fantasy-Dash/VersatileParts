using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VP.Windows.Wpf.Models.Messages
{
    public class Message<T> : RequestMessage<T>
    {
        public Message(T msg)
        {
            Reply(msg);
        }
    }
}
