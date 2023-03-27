using Quartz;
using Quartz.Listener;

namespace VP.Quartz.Listeners
{
    public class WaitJobListener : JobListenerSupport, IDisposable
    {
        private readonly ManualResetEvent manualResetEvent = new(false);

        private readonly string name = Guid.NewGuid().ToString();
        private int exceptionRefireCount = 0;

        public override string Name { get => name; }

        public override Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = default)
        {
            if (jobException!=null)
            {
                if (exceptionRefireCount>=3)
                    throw jobException.GetBaseException();
                exceptionRefireCount++;
                jobException.RefireImmediately=true;
                return Task.CompletedTask;
            }
            manualResetEvent.Set();
            return Task.CompletedTask;
        }

        public void WaitForJobToComplete()
        {
            manualResetEvent.WaitOne();
        }

        public void Dispose()
        {
            manualResetEvent.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
