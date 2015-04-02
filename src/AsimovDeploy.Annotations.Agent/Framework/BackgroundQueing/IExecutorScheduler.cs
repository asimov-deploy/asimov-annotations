using System;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public interface IExecutorScheduler
    {
        event EventHandler<SchedulerEventArgs> Elapsed;
        void Start();
        void Stop();
    }
}