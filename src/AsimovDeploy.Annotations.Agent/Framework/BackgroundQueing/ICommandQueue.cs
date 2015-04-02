using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public interface ICommandQueue
    {
        void EnqueueCommand(AsimovCommand command);
        AsimovCommand Dequeue();
        int Count { get; }
    }
}