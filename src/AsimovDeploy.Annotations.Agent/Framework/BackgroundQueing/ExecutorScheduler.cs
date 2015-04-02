using System;
using System.Timers;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public class ExecutorScheduler : IExecutorScheduler
    {
        private readonly Timer _timer;
        private bool _started;

        public ExecutorScheduler()
        {
            _timer = new Timer(1000)
                     {
                         AutoReset = false
                     };
            _timer.Elapsed += HandleElapsed;
            _started = false;
        }

        private void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            OnChanged(new SchedulerEventArgs());
            if (_started)
            {
                _timer.Start();                
            }
        }

        public event EventHandler<SchedulerEventArgs> Elapsed;

        protected virtual void OnChanged(SchedulerEventArgs e)
        {
            if (Elapsed != null)
                Elapsed(this, e);
        }

        public void Start()
        {
            _timer.Start();
            _started = true;
        }

        public void Stop()
        {
            _started = false;
            _timer.Stop();
        }
    }
}