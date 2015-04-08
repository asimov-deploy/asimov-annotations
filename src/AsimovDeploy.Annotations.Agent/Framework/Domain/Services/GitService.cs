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
using System.Collections.Generic;
using LibGit2Sharp;

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Services
{
    public class GitService : IGitService
    {
        //private readonly string _repo;

        public GitService()
        {
        }

        public IEnumerable<Commit> GetCommits(string branch, string version, string oldVersion)
        {
            foreach (var commit in Test(branch, version, oldVersion)) yield return commit;
        }

        public IEnumerable<Commit> Test(string branch, string version, string oldVersion)
        {
            //Console.WriteLine("WAT");
            //using (var repo = new Repository(_repo))
            //{
            //    repo.Checkout(branch, new CheckoutOptions());
            //    repo.Lookup<Commit>(oldVersion);
            //    var filter = new CommitFilter {Since = repo.Lookup<Commit>(oldVersion), Until = repo.Lookup<Commit>(version)};

            //    foreach (var c in repo.Commits.QueryBy(filter))
            //    {
            //        Console.WriteLine(c.Id); // Of course the output can be prettified ;-)
            //        yield return c;
            //    }
            //}
            return new List<Commit>();
        }
    }
}