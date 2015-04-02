/*******************************************************************************
* Copyright (C) 2015 eBay Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/
using System;
using System.Collections.Generic;
using AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing;
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using AsimovDeploy.Annotations.Agent.Framework.Domain;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Handlers;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Web.Commands;
using FluentAssertions;
using Xunit;

namespace AsimovDeploy.Annotations.Test.CommandExecutor
{
    public class AsimovCommandExecutorTests
    {
        [Fact]
        public void Handle_started_command()
        {
            var handlers = CreateHandlerList();
            var annotationService = new AnnotationServiceFake();
            var annotationDistributorService = new AnnotationDistributorServiceFake();
            var executor = new AsimovCommandExecutor(handlers, annotationService, annotationDistributorService);

            var id = "id";
            var command = CreateDeployStartedCommand(id);
            executor.Handle(command);

            annotationService.Current.Id.Should().Be(id);
            annotationService.Current.Version.Should().Be(1);
        }

        [Fact]
        public void Handle_started_and_first_completed_command()
        {
            var handlers = CreateHandlerList();
            var annotationService = new AnnotationServiceFake();
            var annotationDistributorService = new AnnotationDistributorServiceFake();
            var executor = new AsimovCommandExecutor(handlers, annotationService, annotationDistributorService);

            var id = "id";
            executor.Handle(CreateDeployStartedCommand(id));
            executor.Handle(CreateDeployCompletedCommand(id));

            annotationService.Current.Id.Should().Be(id);
            annotationService.Current.Version.Should().Be(2);
        }

        [Fact]
        public void Handle_started_and_first_completed_finished_command()
        {
            var handlers = CreateHandlerList();
            var annotationService = new AnnotationServiceFake();
            var annotationDistributorService = new AnnotationDistributorServiceFake();
            var executor = new AsimovCommandExecutor(handlers, annotationService, annotationDistributorService);

            var id = "id";
            executor.Handle(CreateDeployStartedCommand(id));
            executor.Handle(CreateDeployCompletedCommand(id));
            executor.Handle(CreateDeployFinishedCommand(id));

            annotationService.Current.Id.Should().Be(id);
            annotationService.Current.Version.Should().Be(3);
        }

        private AsimovCommand CreateDeployFinishedCommand(string id)
        {
            return new DeployFinishedCommand
                   {
                       correlationId = id,
                       timestamp = new DateTime(2000,1,1)
                   };
        }

        private AsimovCommand CreateDeployCompletedCommand(string id)
        {
            return new DeployCompletedCommand
                   {
                       correlationId = id
                   };
        }

        private static DeployStartedCommand CreateDeployStartedCommand(string id)
        {
            return new DeployStartedCommand
                   {
                       correlationId = id,
                       body = "body",
                       startedBy = "startedBy",
                       title = "title",
                       timestamp = new DateTime(2000,1,1)
                   };
        }

        private IList<ICommandExecutor> CreateHandlerList()
        {
            IGitService gitService = new GitServiceFake();
            return new List<ICommandExecutor>
                   {
                       new DeployStartedCommandHandler(),
                       new DeployCompletedCommandHandler(gitService),
                       new DeployFinishedCommandHandler()
                   };
        }
    }

    public class AnnotationDistributorServiceFake : IAnnotationDistributorService
    {
        public void Publish(Annotation annotation)
        {
            
        }
    }
}
