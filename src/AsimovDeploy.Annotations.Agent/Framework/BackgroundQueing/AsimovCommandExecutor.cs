using System.Collections.Generic;
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing
{
    public class AsimovCommandExecutor : IAsimovCommandExecutor
    {
        private readonly IList<ICommandExecutor> _handlers;
        private readonly IAnnotationService _annotationService;
        private readonly IAnnotationDistributorService _annotationDistributorService;

        public AsimovCommandExecutor(IList<ICommandExecutor> handlers, IAnnotationService annotationService, IAnnotationDistributorService annotationDistributorService)
        {
            _handlers = handlers;
            _annotationService = annotationService;
            _annotationDistributorService = annotationDistributorService;
        }

        public void Handle(AsimovCommand command)
        {
            if (command == null) return;

            var executor = _handlers.First(x => x.Handles(command));
            if (executor == null) return;
            
            var @event = executor.Execute(command);
            
            if (@event == null) return;
            
            var annotation = _annotationService.LoadOrCreate(command.correlationId);
            annotation.Apply(@event);
            _annotationService.SaveAnnotation(annotation);
            _annotationDistributorService.Publish(annotation);
        }
    }
}