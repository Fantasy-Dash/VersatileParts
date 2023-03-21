namespace VP.Quartz
{
    public class TaskTimer : IDisposable
    {
        private int _runTimes = 0;
        private CancellationTokenSource _cancelTokenSource = new();
        private Task? _task;
        private bool _keepFirstInterval = false;
        private readonly TimeSpan? _interval;
        private readonly Func<CancellationToken, Task> _taskAction;

        /// <summary>
        /// 定时任务
        /// </summary>
        /// <param name="interval">执行间隔</param>
        /// <param name="action"></param>
        public TaskTimer(TimeSpan? interval, Func<CancellationToken, Task> taskAction, bool keepFirstInterval = false)
        {
            _interval=interval;
            _keepFirstInterval =keepFirstInterval;
            _taskAction=taskAction;
            _task =GetNewTask();
        }

        private Task GetNewTask()
        {
            return new(async () =>
            {
                var run = 0;
                try
                {
                    if (_runTimes==0
                    &&!_keepFirstInterval
                    && _interval != null)
                        await Task.Delay(_interval.Value, _cancelTokenSource.Token);
                    while (!_cancelTokenSource.Token.IsCancellationRequested)
                    {
                        await _taskAction(_cancelTokenSource.Token).ConfigureAwait(false);
                        _keepFirstInterval=false;
                        if (_runTimes > 0)
                        {
                            run++;
                            if (run > _runTimes - 1) break;//减去当前运行的一次
                        }
                        if (_interval != null)
                            await Task.Delay(_interval.Value, _cancelTokenSource.Token);
                    }
                }
                catch (TaskCanceledException) { }
            });
        }


        public Task Start(int runTimes = 0)
        {
            _runTimes=runTimes;
            _task??=GetNewTask();
            if (_task.IsCompleted
                || _task.IsFaulted
                ||_task.IsCanceled)
            {
                _task.Dispose();
                _task = null;
                _task = GetNewTask();
            }
            else if (_task.Status < TaskStatus.RanToCompletion)
                Stop();
            _task.Start();
            return _task;
        }

        public CancellationTokenSource GetCancellationTokenSource() => _cancelTokenSource;

        public void Stop()
        {
            if (_task != null
                &&_task.Status < TaskStatus.RanToCompletion
                &&_task.Status > TaskStatus.Created)
            {
                _cancelTokenSource.Cancel();
                while (_task.Status<TaskStatus.RanToCompletion)
                    Thread.Sleep(50);
                _cancelTokenSource.Dispose();
                _cancelTokenSource = new();
            }
        }

        public void Dispose()
        {
            Stop();
            _cancelTokenSource.Dispose();
            _task?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
