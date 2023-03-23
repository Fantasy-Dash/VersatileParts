namespace VP.Common.Helpers
{
    /// <summary>
    /// 延迟相关方法
    /// </summary>
    public static class DelayHelper
    {//todo 注释
        /// <summary>
        /// 线程等待指定时间
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="rate">指定随机倍率 延时为10 倍率为0.5 结果会在5-15之间</param>
        /// <param name="lastRunTime"></param>
        public static void TimeDelay(ref DateTime lastRunTime, TimeSpan delay, double? rate = null, bool isUpdateLastRunTime = true, CancellationTokenSource? tokenSource = null)
        {
            var span = GetDelay(delay, rate);
            span = lastRunTime + span - DateTime.Now;
            if (span.TotalMilliseconds > 0)
            {
                if (tokenSource is null)
                    Task.Delay(Convert.ToInt32(span.TotalMilliseconds)).GetAwaiter().GetResult();
                else
                    try
                    {
                        Task.Delay(Convert.ToInt32(span.TotalMilliseconds), tokenSource.Token).GetAwaiter().GetResult();
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException is not TaskCanceledException)
                        {
                            throw ex.InnerException ?? new Exception(ex.Message);
                        }
                    }
                    catch (TaskCanceledException) { }
            }
            if (isUpdateLastRunTime) lastRunTime = DateTime.Now;
        }

        /// <summary>
        /// 线程等待指定时间
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="rate">指定随机倍率 延时为10 倍率为0.5 结果会在5-15之间</param>
        /// <param name="lastRunTime"></param>
        public static void TimeDelay(TimeSpan delay, double? rate = null, CancellationTokenSource? tokenSource = null)
        {
            if (GetDelay(delay, rate).TotalMilliseconds > 0)
            {
                if (tokenSource is null)
                    Task.Delay(GetDelay(delay, rate)).GetAwaiter().GetResult();
                else
                    try
                    {
                        Task.Delay(GetDelay(delay, rate), tokenSource.Token).GetAwaiter().GetResult();
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.InnerException is not TaskCanceledException)
                            throw ex.InnerException ?? new Exception(ex.Message);
                    }
            }
        }

        /// <summary>
        /// 根据倍率获取随机延迟
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static TimeSpan GetDelay(TimeSpan delay, double? rate = null)
        {
            if (rate is null) return delay;
            return TimeSpan.FromMilliseconds((new Random().NextDouble() * delay.TotalMilliseconds) + (delay.TotalMilliseconds * rate.Value));
        }
    }
}
