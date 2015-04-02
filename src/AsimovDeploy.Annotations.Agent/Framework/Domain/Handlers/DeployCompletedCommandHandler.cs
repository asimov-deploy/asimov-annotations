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
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing;
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Handlers
{
    public class DeployCompletedCommandHandler : ICommandExecutor
    {
        private readonly IGitService gitService;

        public DeployCompletedCommandHandler(IGitService gitService)
        {
            this.gitService = gitService;
        }

        public bool Handles(AsimovCommand command)
        {
            return command is DeployCompletedCommand;
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

            var completedCommand = (DeployCompletedCommand)command;
            // Off load to background task? Might be expensive!
            var commits = gitService.GetCommits(completedCommand.branch, completedCommand.version, completedCommand.oldVersion);
            var @event = new UnitDeployCompletedEvent
                         {
                             Timestamp = DateTime.UtcNow,
                             AgentName = completedCommand.agentName,
                             Branch = completedCommand.branch,
                             CorrelationId = completedCommand.correlationId,
                             EventName = completedCommand.eventName,
                             Status = completedCommand.status,
                             UnitName = completedCommand.unitName,
                             UserId = completedCommand.userId,
                             UserName = completedCommand.userName,
                             Version = completedCommand.version,
                             OldVersion = completedCommand.oldVersion,
                             Commits = commits.Select(x => new GitCommit
                                                           {
                                                               Committer = x.Author.Name,
                                                               Email = x.Author.Email,
                                                               When = x.Author.When,
                                                               Message = x.Message,
                                                               Sha = x.Sha
                                                           }).ToArray()
                         };
            return @event;
        }
    }
}