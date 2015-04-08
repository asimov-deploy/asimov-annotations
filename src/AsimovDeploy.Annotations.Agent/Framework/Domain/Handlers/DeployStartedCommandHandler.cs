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
using AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing;
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Handlers
{
    public class DeployStartedCommandHandler : ICommandExecutor
    {
        public bool Handles(AsimovCommand command)
        {
            return command is DeployStartedCommand;
        }

        public IEvent Execute(AsimovCommand command)
        {
            if (command == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(command.correlationId))
            {
                // TODO: Log error and save command to some error queue
                return null;
            }

            var deployStartedCommand = (DeployStartedCommand)command;

            var @event = new DeployStartedEvent
                         {
                             CorrelationId = deployStartedCommand.correlationId,
                             Started = deployStartedCommand.timestamp,
                             StartedBy = deployStartedCommand.startedBy,
                             Title = deployStartedCommand.title,
                             Body = deployStartedCommand.body,
                             Timestamp = DateTime.UtcNow
                         };
            return @event;
        }
    }
}