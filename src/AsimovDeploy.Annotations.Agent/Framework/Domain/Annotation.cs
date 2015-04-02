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
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework.Events;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain
{
    public class Annotation
    {
        private readonly List<KeyValuePair<string, Action<IEvent>>> _handlers = new List<KeyValuePair<string, Action<IEvent>>>();

        public string Id { get; private set; }
        public int Version { get; set; }
        public AnnotationState state { get; set; }

        public static Annotation Load(string correlationId, List<IEvent> events)
        {
            var annotation = new Annotation(correlationId, events);
            return annotation;
        }
        
        public Annotation(string correlationId)
        {
            SetUpAppliers();
            Id = correlationId;
            Version = 0;
            state = new AnnotationState(correlationId);
        }

        private Annotation(string id, List<IEvent> events)
        {
            state = new AnnotationState(id, events);
            SetUpAppliers();
            Id = id;
            foreach (var @event in events)
            {
                ApplyInternal(@event, false);
            }
        }

        private void SetUpAppliers()
        {
            _handlers.Add(new KeyValuePair<string, Action<IEvent>>( "DeployStartedEvent", e => ApplyInternal((DeployStartedEvent)e)));
            _handlers.Add(new KeyValuePair<string, Action<IEvent>>( "UnitDeployCompletedEvent", e => ApplyInternal((UnitDeployCompletedEvent)e)));
            _handlers.Add(new KeyValuePair<string, Action<IEvent>>("DeployFinishedEvent", e => ApplyInternal((DeployFinishedEvent)e)));
        }


        public void Apply(IEvent @event)
        {
            // Save events 
            ApplyInternal(@event, true);
        }

        private void ApplyInternal(IEvent @event, bool isNew)
        {
            // Save events 
            if (isNew)
            {
                state.Events.Add(@event);
            }
            ExecuteHandler(@event);
            Version++;
        }

        private void ExecuteHandler(IEvent @event)
        {
            foreach (var pair in _handlers)
            {
                if (pair.Key != @event.GetType().Name) continue;
                pair.Value(@event);
                return;
            }
        }

        private void ApplyInternal(DeployStartedEvent deployStartedEvent)
        {
            state.timestamp = deployStartedEvent.Timestamp;
            state.startedBy = deployStartedEvent.StartedBy;
            state.started = deployStartedEvent.Started;
            state.title = deployStartedEvent.Title;
            state.body = deployStartedEvent.Body;
        }
        private void ApplyInternal(DeployFinishedEvent deployStartedEvent)
        {
            state.timestamp = deployStartedEvent.Timestamp;
            state.finished = deployStartedEvent.Finished;
            state.completed = deployStartedEvent.Completed;

            var deploy = state.Deploys.OrderByDescending(x=>x.Finished).FirstOrDefault();
            state.timestamp = deploy != null ? deploy.Finished : state.finished; 
            // Render message
            state.Message = string.Format("{0}. By: {1} finished {2}", state.title, state.startedBy, state.finished);
        }
        private void ApplyInternal(UnitDeployCompletedEvent deployStartedEvent)
        {

            state.Deploys.Add(new UnitDeploy
            {
                 
                Finished = deployStartedEvent.Timestamp,
                AgentName = deployStartedEvent.AgentName,
                Branch = deployStartedEvent.Branch,
                Commits = deployStartedEvent.Commits.ToList(),
                EventName = deployStartedEvent.EventName,
                OldVersion = deployStartedEvent.OldVersion,
                Status = deployStartedEvent.Status,
                UnitName = deployStartedEvent.UnitName,
                UserId = deployStartedEvent.UserId,
                UserName = deployStartedEvent.UserName,
                Version = deployStartedEvent.Version,
            });
            state.unitnames.Add(deployStartedEvent.UnitName);
            state.agents.Add(deployStartedEvent.AgentName);
        }
    }
}