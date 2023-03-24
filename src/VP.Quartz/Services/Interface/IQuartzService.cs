using Quartz;

namespace VP.Quartz.Services.Interface
{//todo 注释
    public interface IQuartzService : IDisposable
    {
        public Task<IScheduler> CreateSchedulerAsync();

        public Task<IScheduler?> GetSchedulerAsync(string name);

        public Task<IReadOnlyCollection<JobKey>> GetJobKeysByGroupNameAsync(IScheduler scheduler, string jobGroupName);

        public Task<IReadOnlyCollection<TriggerKey>> GetTriggerKeysByGroupNameAsync(IScheduler scheduler, string triggerGroupName);

        public Task<bool> DeleteTriggerAsync(IScheduler scheduler, TriggerKey triggerKey);

        public Task<bool> DeleteJobAsync(IScheduler scheduler, JobKey jobKey);

        public Task<bool> StopJobAsync(IScheduler scheduler, JobKey jobKey);

        public IJobDetail CreateJob(Type jobType, JobKey jobKey, IDictionary<string, object>? jobData = null);

        public ITrigger CreateTrigger(TriggerKey triggerKey, JobKey jobKey, TimeSpan timeSpan, IDictionary<string, object>? jobData = null, bool startNow = true, int misfire = MisfireInstruction.SimpleTrigger.RescheduleNowWithRemainingRepeatCount, int totalExecuteCount = 0);

        public Task ScheduleJobAsync(IScheduler scheduler, IJobDetail job, IReadOnlyCollection<ITrigger> triggerList);

        public Task ScheduleJobsAsync(IScheduler scheduler, IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> jobsAndTriggerList);

        public Task<DateTimeOffset> ScheduleTriggerAsync(IScheduler scheduler, IJobDetail job, ITrigger trigger);

        public void Dispose(bool waitForJobsToComplete);
    }
}
