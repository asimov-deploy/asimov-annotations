using AsimovDeploy.Annotations.Agent.Framework.Events;
using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public interface ICommandExecutor
    {
        bool Handles(AsimovCommand command);
        IEvent Execute(AsimovCommand command);
    }
}