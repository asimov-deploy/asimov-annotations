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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AsimovDeploy.Annotations.Updater
{
    public class UpdateInfoCollector
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof (UpdateInfoCollector));

        private readonly string _watchFolder;
        private readonly int _port;

        public UpdateInfoCollector(string watchFolder, int port)
        {
            _watchFolder = watchFolder;
            _port = port;
        }

        public UpdateInfo Collect()
        {
            return new UpdateInfo()
                {
                    LastBuild = GetLatestVersion(),
                    Current = GetCurrentBuild()
                };
        }

        private AsimovVersion GetLatestVersion()
        {
            if (!Directory.Exists(_watchFolder))
            {
                _log.Error("Watchfolder does not exist: " + _watchFolder);
                return null;
            }

            var pattern = @"v(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)";
            var regex = new Regex(pattern);
            var list = new List<AsimovVersion>();

            foreach (var file in Directory.EnumerateFiles(_watchFolder))
            {
                var match = regex.Match(file);
                if (match.Success)
                {
                    list.Add(new AsimovVersion()
                    {
                        FilePath = file,
                        Version = new Version(int.Parse(match.Groups["major"].Value), int.Parse(match.Groups["minor"].Value), int.Parse(match.Groups["build"].Value))
                    });
                }
            }

            return list.OrderByDescending(x => x.Version).FirstOrDefault();
        }

        public static string GetFullHostName()
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            return ipProperties.DomainName == string.Empty
                ? ipProperties.HostName
                : string.Format("{0}.{1}", ipProperties.HostName, ipProperties.DomainName);
        }

        private AgentVersionInfo GetCurrentBuild()
        {
            var url = String.Format("http://{0}:{1}/version", GetFullHostName(), _port);
            try
            {
                using (var response = WebRequest.Create(url).GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        using (var jsonReader = new JsonTextReader(reader))
                        {
                            var jObject = JObject.Load(jsonReader);
                            var version = (string)jObject.Property("version").Value;
                            var parts = version.Split('.');
                            return new AgentVersionInfo()
                                {
                                    Version = new Version(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]))
                                };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Failed fetch version from: {0}", url);
                _log.Error(ex);
                return new AgentVersionInfo() {Version = new Version(0, 0, 0)};
            }
        }
    }
}