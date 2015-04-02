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
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using NUnit.Framework;

namespace AsimovDeploy.Annotations.IntegrationTest
{
    [TestFixture]
    public class AnnotationServiceTest
    {
        private const string _elastic = "http://localhost:9200";
        private const string _index = "deploy-annotation";

        [Test]
        public void Should_create_a_new_annotation_if_none_already_exists()
        {
            var service = new AnnotationService(new ElasticConfiguration(_elastic, _index));
            var id = Guid.NewGuid().ToString();
            var annotation = service.LoadOrCreate(id);
            annotation.Apply(new DeployStartedEvent { Body = "Body", CorrelationId = id, Started = DateTime.Now, StartedBy = "fhelje", Timestamp = DateTime.Now, Title = "Title" });
            service.SaveAnnotation(annotation);
        }

        [Test]
        public void Should_add_unit_deploy_to_existing_item()
        {
            var service = new AnnotationService(new ElasticConfiguration(_elastic, _index));
            var id = Guid.NewGuid().ToString();
            HandleEvent(service, id, new DeployStartedEvent { Body = "Body", CorrelationId = id, Started = DateTime.Now, StartedBy = "fhelje", Timestamp = DateTime.Now, Title = "Title" });
            HandleEvent(service, id, new UnitDeployCompletedEvent
                                     {
                                         AgentName = "AgentName",
                                         Branch = "Branch",
                                         Commits = new List<GitCommit>(),
                                         CorrelationId = id,
                                         EventName = "EventName",
                                         OldVersion = "OldVersion",
                                         Status = "Status",
                                         Timestamp = DateTime.Now,
                                         UnitName = "UnitName",
                                         UserId = "UserId",
                                         UserName = "UserName",
                                         Version = "Version"
                                     });
        }

        private static void HandleEvent(AnnotationService service, string id, IEvent eventToHandle)
        {
            var annotation = service.LoadOrCreate(id);
            annotation.Apply(eventToHandle);
            service.SaveAnnotation(annotation);
        }

        [Test]
        public void Should_add_two_unit_deploy_to_existing_item()
        {
            var service = new AnnotationService(new ElasticConfiguration(_elastic, _index));
            var id = Guid.NewGuid().ToString();
            HandleEvent(service, id, new DeployStartedEvent { Body = "Body", CorrelationId = id, Started = DateTime.Now, StartedBy = "fhelje", Timestamp = DateTime.Now, Title = "Title" });
            
            HandleEvent(service, id, new UnitDeployCompletedEvent
            {
                AgentName = "AgentName1",
                Branch = "Branch1",
                Commits = new List<GitCommit>(),
                CorrelationId = id,
                EventName = "EventName1",
                OldVersion = "OldVersion1",
                Status = "Status1",
                Timestamp = DateTime.Now,
                UnitName = "UnitName1",
                UserId = "UserId1",
                UserName = "UserName1",
                Version = "Version1"
            });
            
            HandleEvent(service, id, new UnitDeployCompletedEvent
            {
                AgentName = "AgentName2",
                Branch = "Branch2",
                Commits = new List<GitCommit>(),
                CorrelationId = id,
                EventName = "EventName2",
                OldVersion = "OldVersion2",
                Status = "Status2",
                Timestamp = DateTime.Now,
                UnitName = "UnitName2",
                UserId = "UserId2",
                UserName = "UserName2",
                Version = "Version2"
            });

            HandleEvent(service, id, new DeployFinishedEvent
                                     {
                                         CorrelationId = id,
                                         Finished = DateTime.Now,
                                         Timestamp = DateTime.Now,
                                         Completed = true
                                     });
        }
    }
}
