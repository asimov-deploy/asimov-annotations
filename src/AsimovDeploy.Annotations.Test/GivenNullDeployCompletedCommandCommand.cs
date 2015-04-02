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
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Handlers;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using FluentAssertions;
using NUnit.Framework;

namespace AsimovDeploy.Annotations.Test
{
    [TestFixture]
    public class GivenNullDeployCompletedCommandCommand
    {
        private readonly UnitDeployCompletedEvent _event;
        private readonly DeployCompletedCommand _command;

        public GivenNullDeployCompletedCommandCommand()
        {
            IGitService gitService = new GitServiceFake();
            var handler = new DeployCompletedCommandHandler(gitService);

            _command = null;
            _event = handler.Execute(_command) as UnitDeployCompletedEvent;

        }

        [Test]
        public void returned_event_should_be_null()
        {
            _event.Should().BeNull();
        }
    }
}