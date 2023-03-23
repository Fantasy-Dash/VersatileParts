using Quartz;
using Quartz.Impl.Matchers;
using VP.Quartz.Services.Interface;

namespace VP.Quartz.Services
{
    public class QuartzService : IQuartzService, IDisposable
    {
        private readonly ISchedulerFactory _schedulerFactory;
        public QuartzService(ISchedulerFactory schedulerFactory)
        {
            _schedulerFactory=schedulerFactory;
        }

        public async Task<IScheduler> CreateSchedulerAsync()
        {
            return await _schedulerFactory.GetScheduler();
        }

        public async Task<IScheduler?> GetSchedulerAsync(string name)
        {
            return await _schedulerFactory.GetScheduler(name);
        }

        public async Task<IReadOnlyCollection<JobKey>> GetJobKeysByGroupNameAsync(IScheduler scheduler, string jobGroupName)
        {
            if (string.IsNullOrWhiteSpace(jobGroupName))
                throw new ArgumentException($"{nameof(jobGroupName)}的值不能为空");
            return await scheduler.GetJobKeys(GroupMatcher<JobKey>.GroupContains(jobGroupName));
        }

        public async Task<IReadOnlyCollection<TriggerKey>> GetTriggerKeysByGroupNameAsync(IScheduler scheduler, string triggerGroupName)
        {
            if (string.IsNullOrWhiteSpace(triggerGroupName))
                throw new ArgumentException($"{nameof(triggerGroupName)}的值不能为空");
            return await scheduler.GetTriggerKeys(GroupMatcher<TriggerKey>.GroupContains(triggerGroupName));
        }

        public async Task<bool> DeleteTriggerAsync(IScheduler scheduler, TriggerKey triggerKey)
        {
            await scheduler.PauseTrigger(triggerKey);
            return await scheduler.UnscheduleJob(triggerKey);
        }

        public async Task<bool> DeleteJobAsync(IScheduler scheduler, JobKey jobKey)
        {
            await scheduler.PauseJob(jobKey);
            var triggers = await scheduler.GetTriggersOfJob(jobKey);
            triggers.AsParallel().ForAll(async row => await DeleteTriggerAsync(scheduler, row.Key));
            return await scheduler.DeleteJob(jobKey);
        }

        public async Task<bool> StopJobAsync(IScheduler scheduler, JobKey jobKey) => await scheduler.Interrupt(jobKey);

        public IJobDetail CreateJob(Type jobType, JobKey jobKey, IDictionary<string, object>? jobData)
        {
            if (!jobType.GetInterfaces().Contains(typeof(IJob)))
                throw new ArgumentException($"参数类型错误 参数{nameof(jobType)}的类型应该继承{nameof(IJob)}");
            var job = JobBuilder.Create(jobType)
                .WithIdentity(jobKey);
            if (jobData is not null)
                job.SetJobData(new JobDataMap(jobData));
            return job.Build();
        }

        public ITrigger CreateTrigger(TriggerKey triggerKey, JobKey jobKey, TimeSpan timeSpan, IDictionary<string, object>? jobData, bool startNow, int misfire, int totalExecuteCount)
        {
            var trigger = TriggerBuilder.Create()
               .WithIdentity(triggerKey)
               .ForJob(jobKey)
               .WithSimpleSchedule(x =>
               {
                   x.WithInterval(timeSpan);
                   switch (misfire)
                   {
                       case MisfireInstruction.SimpleTrigger.FireNow:
                           x.WithMisfireHandlingInstructionFireNow();
                           break;
                       case MisfireInstruction.SimpleTrigger.RescheduleNowWithExistingRepeatCount:
                           x.WithMisfireHandlingInstructionNowWithExistingCount();
                           break;
                       case MisfireInstruction.SimpleTrigger.RescheduleNowWithRemainingRepeatCount:
                           x.WithMisfireHandlingInstructionNowWithRemainingCount();
                           break;
                       case MisfireInstruction.SimpleTrigger.RescheduleNextWithRemainingCount:
                           x.WithMisfireHandlingInstructionNextWithRemainingCount();
                           break;
                       case MisfireInstruction.SimpleTrigger.RescheduleNextWithExistingCount:
                           x.WithMisfireHandlingInstructionNextWithExistingCount();
                           break;
                       default:
                           break;
                   }
                   if (totalExecuteCount<0)
                       throw new ArgumentException($"参数错误 参数{nameof(totalExecuteCount)}的值应该大于等于0");
                   if (totalExecuteCount==0)
                       x.RepeatForever();
                   else
                       x.WithRepeatCount(totalExecuteCount-1);
               });
            if (jobData is not null)
                trigger.UsingJobData(new JobDataMap(jobData));
            if (startNow)
                trigger.StartNow();
            else
                trigger.StartAt(DateTimeOffset.Now.Add(timeSpan));
            return trigger.Build();
        }

        public async Task ScheduleJobAsync(IScheduler scheduler, IJobDetail job, IReadOnlyCollection<ITrigger> triggerList) => await scheduler.ScheduleJobs(new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>() { { job, triggerList } }, true);

        public async Task ScheduleJobsAsync(IScheduler scheduler, IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> jobsAndTriggerList) => await scheduler.ScheduleJobs(jobsAndTriggerList, true);

        public async Task<DateTimeOffset> ScheduleTriggerAsync(IScheduler scheduler, IJobDetail job, ITrigger trigger)
        {
            return await scheduler.ScheduleJob(trigger);
        }


#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
        public void Dispose() => Dispose(false);

        public void Dispose(bool waitForJobsToComplete)
        {
            _schedulerFactory.GetAllSchedulers()
               .ConfigureAwait(false).GetAwaiter().GetResult()
               .ToList().ForEach(row =>
                                row.Shutdown(waitForJobsToComplete)
                                .GetAwaiter().GetResult());
            GC.SuppressFinalize(this);
        }
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }
}
