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
namespace AsimovDeploy.Annotations.Updater
{
    public class UpdateInfo
    {
        public AsimovVersion LastBuild { get; set; }
        public AgentVersionInfo Current { get; set; }

        public bool HasLastBuild { get { return LastBuild != null; }}

        public bool NeedsAnyUpdate()
        {
            return NewBuildFound();
        }

        public bool NewBuildFound()
        {
            return HasLastBuild && Current.Version < LastBuild.Version;
        }
        
        public override string ToString()
        {
            return string.Format("Current Build: {0}, LatestBuild: {1}, ", Current.Version,
                HasLastBuild ? LastBuild.Version.ToString() : "NA");
        }


    }
}