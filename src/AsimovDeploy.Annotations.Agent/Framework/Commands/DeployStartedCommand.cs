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
using AsimovDeploy.Annotations.Agent.Web.Commands;

namespace AsimovDeploy.Annotations.Agent.Framework.Commands
{
    public class DeployStartedCommand : AsimovCommand
    {
        public DateTime timestamp { get; set; }
        public string startedBy { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}