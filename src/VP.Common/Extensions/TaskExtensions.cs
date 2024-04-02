namespace VP.Common.Extensions
{
    /// <summary>
    /// Task扩展
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// 忽略指定Task的异常 触发忽略后可以执行指定操作
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="task"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task Catch<TException>(this Task task, Action? action = null) where TException : Exception
        {
            try
            {
                await task;
            }
            catch (TException)
            {
                action?.Invoke();
            }
        }

        /// <summary>
        /// 忽略指定Task的异常 触发忽略后执行指定操作
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="defaultValue"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task<TResult> Catch<TException, TResult>(this Task<TResult> task, Func<TResult> action) where TException : Exception
        {
            try
            {
                return await task;
            }
            catch (TException)
            {
                return action.Invoke();
            }
        }
    }
}
