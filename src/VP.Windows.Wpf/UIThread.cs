using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Threading;

namespace VP.Windows.Wpf
{
    public static class UiThread
    {
        public static TResult Invoke<TResult>(Func<TResult> action)
        {
            if (Application.Current.Dispatcher.CheckAccess())
                return Application.Current.Dispatcher.Invoke(action);
            else if (Dispatcher.CurrentDispatcher.CheckAccess())
                return Dispatcher.CurrentDispatcher.Invoke(action);
            else
                return action();
        }

        //todo rewrite choose better way to find True Dispatcher
        public static void Invoke(Action action)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(action);
            }
            catch
            {
                try
                {
                    Dispatcher.CurrentDispatcher.Invoke(action);
                }
                catch
                {
                    action();
                }
            }
        }

        public static bool? ShowWindowDialogWithSTAThread(Window window)
        {
            var taskCompletionSource = new TaskCompletionSource<bool?>();
            var thread = new Thread(() => taskCompletionSource.SetResult(window.ShowDialog()));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return taskCompletionSource.Task.Result;
        }

        public static bool? ShowWindowDialogWithSTAThread<TWindowType>(IServiceProvider serviceProvider) where TWindowType : Window
        {
            var taskCompletionSource = new TaskCompletionSource<bool?>();
            var thread = new Thread(() => taskCompletionSource.SetResult(serviceProvider.GetRequiredService<TWindowType>().ShowDialog()));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            return taskCompletionSource.Task.Result;
        }

        public static void Shutdown()
        {
            Application.Current.Dispatcher.BeginInvoke(Application.Current.Shutdown);
        }
    }
}
