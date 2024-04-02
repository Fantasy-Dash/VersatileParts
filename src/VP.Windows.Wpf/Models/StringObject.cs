using CommunityToolkit.Mvvm.ComponentModel;

namespace VP.Windows.Wpf.Models
{
    public partial class StringObject : ObservableObject
    {
        [ObservableProperty]
        private string? value;

        public StringObject()
        {
        }

        public StringObject(string? value) => Value=value;
    }
}
