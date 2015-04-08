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
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using AsimovDeploy.Annotations.Agent.Service;
using Elasticsearch.Net.Connection;
using Nancy;
using Nancy.ModelBinding;
using Nest;

namespace AsimovDeploy.Annotations.Agent.Web.Modules
{
    public class MainModule : NancyModule
    {
        private readonly IDeploySearchClient _deploySearchClient;

        public MainModule(IDeploySearchClient deploySearchClient)
        {
            _deploySearchClient = deploySearchClient;

            Get["/"] = parameters => View["Index"];

            Get["/version"] = _ =>
                {
                    var resp = new
                        {
                            version = VersionUtil.GetAssemblyVersion()
                        };
                    return Response.AsJson(resp);
                };

            Post["/deploys"] = _ =>
            {
                var command = this.Bind<DeploysQuery>();
                var result = _deploySearchClient.Query(command);
                return Response.AsJson(result);
            };
        }
    }

    public interface IDeploySearchClient
    {
        IEnumerable<AnnotationViewModel> Query(DeploysQuery query);
    }

    public class DeploySearchClient : IDeploySearchClient
    {
        private readonly IElasticConfiguration _configuration;
        private string _elasticDateFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        public DeploySearchClient(IElasticConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<AnnotationViewModel> Query(DeploysQuery query)
        {
            var uri = new Uri(_configuration.ConnectionString);
            var settings = new ConnectionSettings(uri, _configuration.Index);
            var client = new ElasticClient(settings, new HttpConnection(settings));
            var filters = GetFilters(query);

            var annotations = client.Search<AnnotationViewModel>(s =>
                s.Index("deploy-annotations")
                 .Type("annotationstate")
                 .Query(q =>
                    q.Filtered(fqd => 
                        fqd.Query(fq => fq.Match(m => m.OnField(mqd => mqd.title).Query(query.Query)))
                           .Filter(fd=>fd.Bool(bfd=>bfd.Must(filters))))));

            return annotations.Hits.Select(x => x.Source);
        }

        private FilterContainer[] GetFilters(DeploysQuery query)
        {
            var filters = new List<FilterContainer>
                          {
                              new FilterContainer(new RangeFilter
                                                  {
                                                      Field = "finished",
                                                      GreaterThan = query.From.ToString(_elasticDateFormat),
                                                      LowerThanOrEqualTo = query.To.ToString(_elasticDateFormat)
                                                  }),
                              new FilterContainer(new TermFilter
                                                  {
                                                      Field = "completed",
                                                      Value = "true"
                                                  })
                          };
            return filters.ToArray();
        }
    }

    public class AnnotationViewModel
    {
        public string Id { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime started { get; set; }
        public string startedBy { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public bool completed { get; set; }
        public DateTime finished { get; set; }
        public string Message { get; set; }
        public List<string> unitnames { get; set; }
        public List<string> agents { get; set; }
    }

    public class DeploysQuery
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Query { get; set; }
    }
}