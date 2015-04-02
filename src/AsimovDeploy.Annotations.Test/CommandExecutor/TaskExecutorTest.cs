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
using AsimovDeploy.Annotations.Agent.Framework.Domain.Handlers;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Web.Commands;
using FluentAssertions;
using NUnit.Framework;

namespace AsimovDeploy.Annotations.Test.CommandExecutor
{
    [TestFixture]
    public class TaskExecutorTest
    {
        [Test]
        public void Trigger_null_command()
        {
            var queue = new ConcurrentCommandQueue();
            var annotationService = new AnnotationServiceFake();
            var schedulerFake = new ExecutorSchedulerFake();
            CreateTaskExecutor(queue, annotationService, schedulerFake);

            AsimovCommand command = null;
            queue.EnqueueCommand(command);
            schedulerFake.TriggerElapsed();

            annotationService.Current.Should().Be(null);
        }

        [Test]
        public void Trigger_single_command()
        {
            var queue = new ConcurrentCommandQueue();
            var annotationService = new AnnotationServiceFake();
            var schedulerFake = new ExecutorSchedulerFake();
            CreateTaskExecutor(queue, annotationService, schedulerFake);

            const string id = "id";
            queue.EnqueueCommand(CreateDeployStartedCommand(id));
            schedulerFake.TriggerElapsed();

            annotationService.Current.Id.Should().Be(id);
            annotationService.Current.Version.Should().Be(1);
            annotationService.Current.state.Events.Should().HaveCount(1);
        }

        [Test]
        public void Trigger_multiple_commands()
        {
            var queue = new ConcurrentCommandQueue();
            var annotationService = new AnnotationServiceFake();
            var schedulerFake = new ExecutorSchedulerFake();
            CreateTaskExecutor(queue, annotationService, schedulerFake);

            const string id = "id";
            queue.EnqueueCommand(CreateDeployStartedCommand(id));
            queue.EnqueueCommand(CreateDeployCompletedCommand(id));
            queue.EnqueueCommand(CreateDeployFinishedCommand(id));
            schedulerFake.TriggerElapsed();

            annotationService.Current.Id.Should().Be(id);
            annotationService.Current.Version.Should().Be(3);
            annotationService.Current.state.Events.Should().HaveCount(3);
        }

        private AsimovCommand CreateDeployFinishedCommand(string id)
        {
            return new DeployFinishedCommand
            {
                correlationId = id,
                timestamp = new DateTime(2000, 1, 1)
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
                timestamp = new DateTime(2000, 1, 1)
            };
        }

        private void CreateTaskExecutor(ConcurrentCommandQueue queue, AnnotationServiceFake annotationService, ExecutorSchedulerFake schedulerFake)
        {
            var executor = new TaskExecutor(
                queue,
                new AsimovCommandExecutor(
                    CreateHandlerList(),
                    annotationService,
                    new AnnotationDistributorServiceFake()
                    ),
                schedulerFake
                );
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
}
