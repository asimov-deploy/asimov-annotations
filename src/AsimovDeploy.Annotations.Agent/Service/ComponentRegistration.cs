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
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework;
using AsimovDeploy.Annotations.Agent.Framework.BackgroundQueing;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace AsimovDeploy.Annotations.Agent.Service
{
    public static class ComponentRegistration
    {
        private static Container _container;
        public static void RegisterComponents(Container container)
        {
            var commandQueue = new ConcurrentCommandQueue();
            _container = container;
            _container.Configure(registry =>
            {
                registry.For<IElasticConfiguration>().Singleton().Use(new ElasticConfiguration());
                registry.For<ICommandQueue>().Singleton().Use(commandQueue);
                registry.Scan(assembly =>
                {
                    assembly.TheCallingAssembly();
                    assembly.WithDefaultConventions();
                    assembly.With(new SingletonConvention<IStartable>());
                    assembly.AddAllTypesOf<ICommandExecutor>();
                });
            });
        }

        public static void StartStartableComponenters()
        {

            foreach (var startable in _container.GetAllInstances<IStartable>())
            {
                startable.Start();
            }
        }

        public static void StopAll()
        {

            foreach (var startable in _container.GetAllInstances<IStartable>())
            {
                startable.Stop();
            }
        }
    }

    internal class SingletonConvention<TPluginFamily> : IRegistrationConvention
    {
        public void Process(Type type, Registry registry)
        {
            if (!type.IsConcrete() || !type.CanBeCreated() || !type.AllInterfaces().Contains(typeof(TPluginFamily))) return;

            registry.For(typeof(TPluginFamily)).Singleton().Use(type).Named(type.Name);
        }
    }
}