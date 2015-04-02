using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public interface IAsimovCommandExecutor
    {
        void Handle(AsimovCommand command);
    }
}