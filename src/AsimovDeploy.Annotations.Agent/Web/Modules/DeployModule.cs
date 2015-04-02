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

using AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing;
using AsimovDeploy.Annotations.Agent.Framework.Commands;
using log4net;
using Nancy;
using Nancy.ModelBinding;

namespace AsimovDeploy.Annotations.Agent.Web.Modules
{
    public class DeployModule : NancyModule
    {
        public static ILog _log = LogManager.GetLogger(typeof(DeployModule));

        public DeployModule(ICommandQueue queue)
        {

            Post["/deploy/start"] = _ =>
            {
                var command = this.Bind<DeployStartedCommand>();
                
                _log.Debug(string.Format("Received command from /deploy/start: {0}", command.title));

                queue.EnqueueCommand(command);
                
                return Response.AsJson(new { OK = true });
            };

            Post["/deploy/unit_completed"] = _ =>
            {
                var command = this.Bind<DeployCompletedCommand>();

                _log.Debug(string.Format("Received command from /deploy/unit_completed: {0}", command.eventName));

                queue.EnqueueCommand(command);
                
                return Response.AsJson(new { OK = true });
            };

            Post["/deploy/finished"] = _ =>
            {
                var command = this.Bind<DeployFinishedCommand>();

                _log.Debug(string.Format("Received command from /deploy/finished: {0}", command.timestamp));

                queue.EnqueueCommand(command);
                
                return Response.AsJson(new { OK = true });
            };

            Post["/deploy/cancelled"] = _ =>
            {
                var command = this.Bind<DeployCancelledCommand>();

                _log.Debug(string.Format("Received command from /deploy/cancelled: {0}", command.timestamp));

                queue.EnqueueCommand(command);

                return Response.AsJson(new { OK = true });
            };
        }
    }
}