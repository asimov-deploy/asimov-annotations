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
using AsimovDeploy.Annotations.Agent.Framework.Events;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain
{
    public class AnnotationState
    {

        public AnnotationState(string id, List<IEvent> events = null)
        {
            Id = id;
            Events = events ?? new List<IEvent>();
            Deploys = new List<UnitDeploy>();
            unitnames = new List<string>();
            agents = new List<string>();
        }

        public string Id { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime started { get; set; }
        public string startedBy { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public bool completed { get; set; }
        public DateTime finished { get; set; }
        public List<UnitDeploy> Deploys { get; set; }

        public List<IEvent> Events { get; set; }
        public string Message { get; set; }
        public List<string> unitnames { get; set; }
        public List<string> agents { get; set; }
    }
}