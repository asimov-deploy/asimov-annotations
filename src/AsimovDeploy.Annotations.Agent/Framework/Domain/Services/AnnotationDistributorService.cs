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
using Elasticsearch.Net.Connection;
using Nest;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Services
{
    public class AnnotationDistributorService : IAnnotationDistributorService
    {
        private readonly IElasticConfiguration _configuration;

        public AnnotationDistributorService(IElasticConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Publish(Annotation annotation)
        {
            if (annotation.state.completed)
            {
                SaveToMarkerIndicies(new MarkerAnnotation(annotation.state.finished, annotation.state.Message, annotation.state.startedBy));
            }
        }

        private void SaveToMarkerIndicies(MarkerAnnotation markerAnnotation)
        {
            foreach (var markerConf in _configuration.MarkerIndecies)
            {
                var uri = new Uri(markerConf.ConnectionString);
                var settings = new ConnectionSettings(uri, markerConf.Index);
                var client = new ElasticClient(settings, new HttpConnection(settings));
                var conf = markerConf;

                var result = client.Index(markerAnnotation, i => i.Index(conf.Index).Type("markerannotation"));
                if (!result.Created)
                {
                    Console.WriteLine("Error");
                }
            }
        }
    }
}