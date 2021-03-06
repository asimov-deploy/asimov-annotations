﻿/*******************************************************************************
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
using System.Configuration;

namespace AsimovDeploy.Annotations.Agent.Web.Setup
{
    public class AnnotationsConfig : IAnnotationsConfig
    {
        public AnnotationsConfig()
        {
            var host = ConfigurationManager.AppSettings["WebControlUrl"];
            var port = int.Parse(ConfigurationManager.AppSettings["WebPort"]);
            ApiKey = "";

            WebControlUrl = new Uri(string.Format("{0}:{1}", host, port));

        }
        public Uri WebControlUrl { get; set; }
        public string ApiKey { get; set; }
    }
}