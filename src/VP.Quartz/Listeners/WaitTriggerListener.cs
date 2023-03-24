using Quartz;
using Quartz.Listener;

namespace VP.Quartz.Listeners
{
    public class WaitTriggerListener : TriggerListenerSupport, IDisposable
    {
        private readonly ManualResetEvent manualResetEvent = new(false);

        private readonly string name = Guid.NewGuid().ToString();

        public override string Name { get => name; }

        public override Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
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
