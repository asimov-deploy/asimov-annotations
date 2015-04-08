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

namespace AsimovDeploy.Annotations.Agent.Framework.Events
{
    public class UnitDeployCompletedEvent : IEvent
    {
        public string EventName { get; set; }
        public string AgentName { get; set; }
        public string UnitName { get; set; }
        public string Version { get; set; }
        public string OldVersion { get; set; }
        public string Branch { get; set; }
        public string Status { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string CorrelationId { get; set; }
        public IEnumerable<GitCommit> Commits { get; set; }
        public DateTime Timestamp { get; set; }
    }
}