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
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;
using NUnit.Framework;

namespace AsimovDeploy.Annotations.IntegrationTest
{
    [TestFixture]
    public class GitServiceTests
    {
        [Test]
        public void GetCommits()
        {
            var gs = new GitService();
            var version = "7c0360038981802fe35a9273002b5c2bb349e1d5";
            var oldVersion = "b02efbdc2240f558571e92137dceecf231e623dd";

            var list = gs.Test("master", version, oldVersion);
            //Assert.NotEmpty(list);
        }
    }
}
