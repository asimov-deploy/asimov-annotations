namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public class TaskExecutor : IStartable
    {
        private readonly ICommandQueue _queue;
        private readonly IAsimovCommandExecutor _commandExecutor;
        private readonly IExecutorScheduler _scheduler;

        public TaskExecutor(ICommandQueue queue, IAsimovCommandExecutor commandExecutor, IExecutorScheduler scheduler)
        {
            _queue = queue;
            _commandExecutor = commandExecutor;
            _scheduler = scheduler;
            _scheduler.Elapsed += HandleElapsed;
        }

        private void HandleElapsed(object sender, SchedulerEventArgs e)
        {
            while (_queue.Count > 0)
            {
                var command = _queue.Dequeue();
                _commandExecutor.Handle(command);
            }
            _scheduler.Start();
        }

        public void Start()
        {
            _scheduler.Start();
        }

        public void Stop()
        {
            _scheduler.Stop();
        }
    }
}