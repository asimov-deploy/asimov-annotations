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
using AsimovDeploy.Annotations.Agent.Framework.Domain;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using FluentAssertions;
using NUnit.Framework;

namespace AsimovDeploy.Annotations.Test
{
    [TestFixture]
    public class AnnotationsTests
    {
        [Test]
        public void Deploy_started()
        {
            var service = new AnnotationServiceFake();
            var id = "CorrelationId";
            var annotation = service.LoadOrCreate(id);
            var deployStartedEvent = CreateDeployStartedEvent(id);

            annotation.Apply(deployStartedEvent);

            VerifyDeployStartedState(annotation, id, deployStartedEvent);
        }

        private static void VerifyDeployStartedState(Annotation annotation, string id, DeployStartedEvent deployStartedEvent)
        {
            annotation.Id.Should().Be(id);
            annotation.Version.Should().Be(1);
            annotation.state.body.Should().Be(deployStartedEvent.Body);
            annotation.state.startedBy.Should().Be(deployStartedEvent.StartedBy);
            annotation.state.timestamp.Should().Be(deployStartedEvent.Timestamp);
            annotation.state.title.Should().Be(deployStartedEvent.Title);
            annotation.state.Id.Should().Be(deployStartedEvent.CorrelationId);
        }

        [Test]
        public void DeployStarted_DeployCompleted()
        {
            var service = new AnnotationServiceFake();
            var id = "CorrelationId";
            var annotation = service.LoadOrCreate(id);

            annotation.Apply(CreateDeployStartedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            service.Current.Version.Should().Be(2);
            service.Current.state.Events.Should().HaveCount(2);
        }

        [Test]
        public void DeployStarted_DeployCompleted_DeployCompleted()
        {
            var service = new AnnotationServiceFake();
            var id = "CorrelationId";
            var annotation = service.LoadOrCreate(id);

            annotation.Apply(CreateDeployStartedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            service.Current.Version.Should().Be(3);
            service.Current.state.Deploys.Should().HaveCount(2);
            service.Current.state.Events.Should().HaveCount(3);
        }

        [Test]
        public void DeployStarted_DeployCompleted_DeployCompleted_DeployFinished()
        {
            var service = new AnnotationServiceFake();
            var id = "CorrelationId";
            var annotation = service.LoadOrCreate(id);

            annotation.Apply(CreateDeployStartedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateDeployFinishedEvent(id));
            service.SaveAnnotation(annotation);

            service.Current.Version.Should().Be(4);
            service.Current.state.Deploys.Should().HaveCount(2);
            service.Current.state.Events.Should().HaveCount(4);
        }

        [Test]
        public void DeployStarted_DeployCompleted_DeployCompleted_DeployCancelled()
        {
            var service = new AnnotationServiceFake();
            var id = "CorrelationId";
            var annotation = service.LoadOrCreate(id);

            annotation.Apply(CreateDeployStartedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            annotation.Apply(CreateUnitDeployCompletedEvent(id));
            service.SaveAnnotation(annotation);

            var deployFinishedEvent = (DeployFinishedEvent)CreateDeployFinishedEvent(id);
            deployFinishedEvent.Completed = false;

            annotation.Apply(deployFinishedEvent);
            service.SaveAnnotation(annotation);

            service.Current.Version.Should().Be(4);
            service.Current.state.Deploys.Should().HaveCount(2);
            service.Current.state.Events.Should().HaveCount(4);
            service.Current.state.completed.Should().BeFalse();
        }

        private IEvent CreateDeployFinishedEvent(string id)
        {
            return new DeployFinishedEvent
                   {
                       CorrelationId = id,
                       Finished = new DateTime(2000,1,1),
                       Timestamp = new DateTime(2000,1,1)
                   };
        }


        private UnitDeployCompletedEvent CreateUnitDeployCompletedEvent(string id)
        {
            return new UnitDeployCompletedEvent
                   {
                       AgentName = "",
                       CorrelationId = id,
                       Branch = "",
                       Commits = new List<GitCommit>(),
                       EventName = "",
                       OldVersion = "",
                       Status = "",
                       Timestamp = new DateTime(2000,1,1),
                       UnitName = "",
                       UserId = "",
                       UserName = "",
                       Version = ""
                   };
        }

        private static DeployStartedEvent CreateDeployStartedEvent(string id)
        {
            return new DeployStartedEvent
                   {
                       Body = "Body",
                       CorrelationId = id,
                       Started = new DateTime(2000,1,1),
                       StartedBy = "StartedBy",
                       Timestamp = new DateTime(2000, 1, 1),
                       Title = "Title"
                   };
        }
    }
}
