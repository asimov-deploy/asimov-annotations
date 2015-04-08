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
using System.Diagnostics;
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework.Events;
using Elasticsearch.Net.ConnectionPool;
using Nest;
using Newtonsoft.Json;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Services
{
    public interface IAnnotationService
    {
        void SaveAnnotation(Annotation annotation);
        Annotation LoadOrCreate(string correlationId);
    }

    public class AnnotationService : IAnnotationService
    {
        private readonly IElasticConfiguration _configuration;
        private readonly ElasticClient _client;

        public AnnotationService(IElasticConfiguration configuration)
        {
            _configuration = configuration;
            var connectionSettings = new ConnectionSettings(new SingleNodeConnectionPool(new Uri(configuration.ConnectionString)));
            connectionSettings.SetJsonSerializerSettingsModifier(s => s.TypeNameHandling = TypeNameHandling.Auto);
            _client = new ElasticClient(connectionSettings);
        }

        public void SaveAnnotation(Annotation annotation)
        {
            var index = _configuration.Index;
            var response = _client.Index(
                annotation.state,
                x=>x.Index(index)
                    .Id(annotation.state.Id)
                    .Version(annotation.Version - 1)
                    .Type("annotationstate"));

            Debug.Assert(response.IsValid);
        }

        public Annotation LoadOrCreate(string correlationId)
        {
            var annotationState = _client.Get<AnnotationState>(correlationId, _configuration.Index);
            if (annotationState.Found)
            {
                return Annotation.Load(annotationState.Id, annotationState.Source.Events.Cast<IEvent>().ToList());                
            }
            return new Annotation(correlationId);
        }
    }
}