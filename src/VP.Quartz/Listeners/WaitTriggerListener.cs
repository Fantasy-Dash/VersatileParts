using Quartz;
using Quartz.Listener;

namespace VP.Quartz.Listeners
{
    public class WaitTriggerListener : TriggerListenerSupport, IDisposable
    {
        private readonly AutoResetEvent re = new(false);

        private readonly string name = Guid.NewGuid().ToString();

        public override string Name { get => name; }

        public override Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
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
