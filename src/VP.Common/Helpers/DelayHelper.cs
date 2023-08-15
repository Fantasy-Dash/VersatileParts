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
        /// <param name="baseTime">基准时间</param>
        public static void TimeDelay(ref DateTime baseTime, TimeSpan delay, double? rate = null, bool isUpdateBaseTime = true, CancellationTokenSource? tokenSource = null)
        {
            var span = GetDelay(delay, rate);
            span = baseTime + span - DateTime.Now;
            if (span.TotalMilliseconds > 0)
            {
                if (tokenSource is null)
                    Task.Delay(span).GetAwaiter().GetResult();
                else
                    try
                    {
                        Task.Delay(span, tokenSource.Token).GetAwaiter().GetResult();
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.GetBaseException() is not TaskCanceledException)
                            throw ex.GetBaseException();
                    }
                    catch (TaskCanceledException) { }
            }
            if (isUpdateBaseTime) baseTime = DateTime.Now;
        }

        /// <summary>
        /// 线程等待指定时间
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="rate">指定随机倍率 延时为10 倍率为0.5 结果会在5-15之间</param>
        /// <param name="lastRunTime"></param>
        public static void TimeDelay(TimeSpan delay, double? rate = null, CancellationTokenSource? tokenSource = null)
        {
            var span = GetDelay(delay, rate);
            if (span.TotalMilliseconds > 0)
            {
                if (tokenSource is null)
                    Task.Delay(span).GetAwaiter().GetResult();
                else
                    try
                    {
                        Task.Delay(span, tokenSource.Token).GetAwaiter().GetResult();
                    }
                    catch (AggregateException ex)
                    {
                        if (ex.GetBaseException() is not TaskCanceledException)
                            throw ex.GetBaseException();
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
