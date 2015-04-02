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
using NUnit.Framework;

namespace AsimovDeploy.Annotations.Test
{
    [TestFixture]
    public class GivenValidDeployFinishedCommand
    {
        private readonly DeployFinishedEvent _event;
        private readonly DeployFinishedCommand _deployStartedCommand;

        public GivenValidDeployFinishedCommand()
        {
            const string correlationId = "1";
            var handler = new DeployFinishedCommandHandler();

            _deployStartedCommand = new DeployFinishedCommand
                                    {
                                        correlationId = correlationId,
                                        timestamp = new DateTime(2000, 1, 1),                                        
                                    };
            _event = handler.Execute(_deployStartedCommand) as DeployFinishedEvent;
        }

        [Test]
        public void Event_property_body_should_equal_command_property_correlationId()
        {

            _event.CorrelationId.Should().Be(_deployStartedCommand.correlationId);
        }

        [Test]
        public void Event_property_body_should_equal_command_property_timestamp()
        {

            _event.Finished.Should().Be(_deployStartedCommand.timestamp);
        }

        [Test]
        public void Event_property_completed_should_be_true()
        {

            _event.Completed.Should().Be(true);
        }

        [Test]
        public void Event_property_timestamp_should_be_equal_to_utcnow()
        {

            _event.Timestamp.Should().BeCloseTo(DateTime.UtcNow, 50);
        }
    }
}