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
using AsimovDeploy.Annotations.Agent.Service;
using log4net;
using Topshelf;

namespace AsimovDeploy.Annotations.Agent
{
    class Program
    {
        public static ILog _log = LogManager.GetLogger(typeof (Program));

        private const string ServiceName = "AsimovDeploy.Annotations";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            
            var host = HostFactory.New(x =>
            {
                
                x.Service<IAsimovAnnotationService>(s =>
                {
                    s.BeforeStartingService(c => _log.DebugFormat("Starting {0}...", ServiceName));
                    s.AfterStoppingService(c => _log.DebugFormat("Stopping {0}...", ServiceName));
                    
                    s.ConstructUsing(name => new AsimovAnnotationService());

                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                
                x.RunAsLocalSystem();
                x.SetDisplayName(ServiceName);
                x.SetDescription(ServiceName);
                x.SetServiceName(ServiceName);
            });

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
            
            host.Run();
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            _log.Error("Unhandled exception", (Exception)e.ExceptionObject);
        }
    }
}
