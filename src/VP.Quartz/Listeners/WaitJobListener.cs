using Quartz;
using Quartz.Listener;

namespace VP.Quartz.Listeners
{
    public class WaitJobListener : JobListenerSupport, IDisposable
    {
        private readonly AutoResetEvent re = new(false);

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
            re.Set();
            return Task.CompletedTask;
        }

        public void WaitForJobToComplete()
        {
            re.WaitOne();
        }

        public void Dispose()
        {
            re.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
