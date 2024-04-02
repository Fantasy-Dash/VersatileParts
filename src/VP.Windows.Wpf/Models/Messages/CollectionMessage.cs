using CommunityToolkit.Mvvm.Messaging.Messages;

namespace VP.Windows.Wpf.Models.Messages
{
    public abstract class CollectionMessage<T> : CollectionRequestMessage<T>
    {
        public CollectionMessage(IEnumerable<T> list)
        {
            foreach (T item in list) Reply(item);
        }
    }
}
