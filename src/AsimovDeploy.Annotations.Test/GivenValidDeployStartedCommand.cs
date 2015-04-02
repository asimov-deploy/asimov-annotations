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
using AsimovDeploy.Annotations.Agent.Framework.Events;
using FluentAssertions;
using Xunit;

namespace AsimovDeploy.Annotations.Test
{
    public class GivenValidDeployStartedCommand
    {
        private readonly DeployStartedEvent _event;
        private readonly DeployStartedCommand _deployStartedCommand;

        public GivenValidDeployStartedCommand()
        {
            const string correlationId = "1";
            var handler = new DeployStartedCommandHandler();

            _deployStartedCommand = new DeployStartedCommand
                                    {
                                        body = "body",
                                        correlationId = correlationId,
                                        startedBy = "startedBy",
                                        timestamp = new DateTime(2000, 1, 1),
                                        title = "title"
                                    };
            _event = handler.Execute(_deployStartedCommand) as DeployStartedEvent;            
        }

        [Fact]
        public void Event_property_body_should_equal_command_property_body()
        {

            _event.Body.Should().Be(_deployStartedCommand.body);
        }

        [Fact]
        public void Event_property_StartedBy_should_equal_command_property_StartedBy()
        {

            _event.StartedBy.Should().Be(_deployStartedCommand.startedBy);
        }

        [Fact]
        public void Event_property_title_should_equal_command_property_title()
        {

            _event.Title.Should().Be(_deployStartedCommand.title);
        }

        [Fact]
        public void Event_property_Started_should_equal_command_property_timestamp()
        {

            _event.Started.Should().Be(_deployStartedCommand.timestamp);
        }

        [Fact]
        public void Event_property_correlationId_should_equal_command_property_correlationId()
        {

            _event.CorrelationId.Should().Be(_deployStartedCommand.correlationId);
        }

        [Fact]
        public void Event_property_Timestamp_should_close_to_utc_now()
        {
            _event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, precision: 500);
        }
    }
}