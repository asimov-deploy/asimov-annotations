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
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using Ionic.Zip;
using log4net;

namespace AsimovDeploy.Annotations.Updater
{
    public class Updater : IAsimovAnnotationUpdaterService
    {
        private Timer _timer;

        private static readonly ILog _log = LogManager.GetLogger(typeof(Updater));
        private string _watchFolder;
        private int _port;
        private string _installDir;
        private const int Interval = 4000;
        
        public void Start()
        {
            _watchFolder = ConfigurationManager.AppSettings["Asimov.Annotations.WatchFolder"];
            _installDir = ConfigurationManager.AppSettings["Asimov.Annotations.InstallFolder"];
            _port = Int32.Parse(ConfigurationManager.AppSettings["Asimov.Annotations.WebPort"]);

            _timer = new Timer(TimerTick, null, 0, Interval);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void TimerTick(object state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);

            _log.Info("Looking for new version");

            try
            {
                var updateInfo = new UpdateInfoCollector(_watchFolder, _port).Collect();

                _log.InfoFormat(updateInfo.ToString());

                using (var service = new ServiceController("AsimovDeploy.Annotations"))
                {
                    if (!updateInfo.NeedsAnyUpdate())
                        return;

                    StopService(service);
                    
                    if (updateInfo.NewBuildFound())
                    {
                        UpdateAnnotationsWithNewBuild(updateInfo.LastBuild);
                    }

                    StartService(service);
                }

            }
            catch(Exception ex)
            {
                _log.Error("Failed to check for upgrade", ex);
            }
            finally
            {
                _timer.Change(Interval, Interval);
            }
        }

        private void UpdateAnnotationsWithNewBuild(AsimovVersion lastBuild)
        {
            _log.InfoFormat("Installing new build {0}", lastBuild.Version);
            
            CleanFolder(_installDir);

            CopyNewBuildToInstallDir(_installDir, lastBuild.FilePath);
        }

        private static void StopService(ServiceController serviceController)
        {
            if (serviceController.Status == ServiceControllerStatus.Running)
            {
                _log.Info("Stopping AsimovDeploy.Annotations...");
                serviceController.Stop();
                serviceController.WaitForStatus(ServiceControllerStatus.Stopped);
                _log.Info("AsimovDeploy..Annotations stopped");
            }
            else
            {
                _log.Info("AsimovDeploy Service was not running, trying to update and start it");
            }
        }

        private void StartService(ServiceController serviceController)
        {
            _log.Info("Starting service...");
            serviceController.Start();
            serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(1));
            
            _log.Info("Service started");
        }

        private void CopyNewBuildToInstallDir(string installDir, string filePath)
        {
            using (var zipFile = ZipFile.Read(filePath))
            {
               zipFile.ExtractAll(installDir);
            }
        }

        private void CleanFolder(string destinationFolder)
        {
            if (destinationFolder.Contains("AsimovAnnotations") == false)
            {
                throw new Exception("Asimov.Annotations install dir does not contain asimov, will abort upgrade");
            }

            var dir = new DirectoryInfo(destinationFolder);
            foreach (var file in dir.GetFiles())
            {
                if (!file.Extension.Contains("log"))
                    file.Delete();
            }

            foreach (var subDirectory in dir.GetDirectories()) subDirectory.Delete(true);
        }

       
    }
}