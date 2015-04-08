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
using log4net;
using StructureMap;

namespace AsimovDeploy.Annotations.Agent.Service
{
    public class AsimovAnnotationService : IAsimovAnnotationService
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AsimovAnnotationService));

        public void Start()
        {
            try
            {
                var container = new Container();
                ComponentRegistration.RegisterComponents(container);
                ComponentRegistration.StartStartableComponenters();

                Log.InfoFormat("WinAgent Started, Version={0}", VersionUtil.GetAssemblyVersion());
            }
            catch (Exception e)
            {
                Log.Error("Error while starting AsimovDeployService", e);
                throw;
            }
        }

        public void Stop()
        {
            Log.Info("WinAgent Stopping");
            ComponentRegistration.StopAll();
        }
    }
}