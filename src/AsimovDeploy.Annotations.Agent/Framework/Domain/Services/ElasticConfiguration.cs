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
using System.Configuration;
using System.Linq;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Services
{
    public class ElasticConfiguration : IElasticConfiguration
    {
        private readonly string _indexStart;
        private readonly List<string> _markerIndecies = new List<string>();

        public ElasticConfiguration()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["annotationIndex"].ConnectionString;
            _indexStart = ConfigurationManager.AppSettings["annotationIndexStart"];
            _markerIndecies.AddRange(
                ConfigurationManager.ConnectionStrings
                                    .Cast<ConnectionStringSettings>()
                                    .Where(cs => cs.Name.ToLower().StartsWith("marker"))
                                    .Select(x=>x.ConnectionString));
        }

        public ElasticConfiguration(string elasticConnectiopnString, string index)
        {
            ConnectionString = elasticConnectiopnString;
            _indexStart = index;
        }
        public string ConnectionString { get; set; }

        public IEnumerable<ElasticsearchMarkerConfiguration> MarkerIndecies
        {
            get
            {
                var now = DateTime.Now;
                return _markerIndecies.Select(cs =>new ElasticsearchMarkerConfiguration{ 
                    ConnectionString = cs,
                    Index = string.Format("annotation-{0}.{1}", now.Year, now.Month.ToString("D2"))
                });

            }
        }

        public string Index
        {
            get
            {
                var now = DateTime.Now;
                return string.Format("{0}-{1}.{2}", _indexStart, now.Year, now.Month.ToString("D2"));
            }
        }
    }

    public class ElasticsearchMarkerConfiguration
    {
        public string ConnectionString { get; set; }
        public string Index { get; set; }
    }
}