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
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Handlers;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using FluentAssertions;
using Xunit;

namespace AsimovDeploy.Annotations.Test
{
    public class GivenValidDeployCompletedCommandCommand
    {
        private readonly UnitDeployCompletedEvent _event;
        private readonly DeployCompletedCommand _command;

        public GivenValidDeployCompletedCommandCommand()
        {
            const string correlationId = "1";
            IGitService gitService = new GitServiceFake();
            var handler = new DeployCompletedCommandHandler(gitService);

            _command = new DeployCompletedCommand
                       {
                           agentName = "agentName",
                           branch = "branch",
                           correlationId = correlationId,
                           eventName = "eventName",
                           status = "status",
                           unitName = "unitName",
                           userId = "userId",
                           userName = "userName",
                           version = "verison",
                           oldVersion = "oldVersion"
                       };
            _event = handler.Execute(_command) as UnitDeployCompletedEvent;
            
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_agentName()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.AgentName.Should().Be(_command.agentName);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_branch()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.Branch.Should().Be(_command.branch);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_correlationId()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.CorrelationId.Should().Be(_command.correlationId);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_eventName()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.EventName.Should().Be(_command.eventName);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_oldVersion()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.OldVersion.Should().Be(_command.oldVersion);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_status()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.Status.Should().Be(_command.status);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_timestamp()
        {
            _event.Status.Should().NotBeNullOrEmpty();
            _event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 50);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_UnitName()
        {
            _event.UnitName.Should()
                .NotBeNullOrEmpty()
                .And.Be(_command.unitName);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_userId()
        {
            _event.UserId.Should()
                .NotBeNullOrEmpty()
                .And.Be(_command.userId);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_userName()
        {
            _event.UserName.Should()
                .NotBeNullOrEmpty()
                .And.Be(_command.userName);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_version()
        {
            _event.Version.Should()
                .NotBeNullOrEmpty()
                .And.Be(_command.version);
        }

        [Fact]
        public void returned_event_should_have_same_value_for_property_commits()
        {
            _event.Commits.Should().BeEmpty();
        }
    }
}