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

namespace AsimovDeploy.Annotations.Agent.Framework.Domain.Services
{
    public class MarkerAnnotation
    {
        private readonly DateTime _finished;
        private readonly string _message;
        private readonly string _startedBy;

        public MarkerAnnotation(DateTime finished, string message, string startedBy)
        {
            _finished = finished;
            _message = message;
            _startedBy = startedBy;
        }

        public DateTime Finished
        {
            get { return _finished; }
        }

        public string Message
        {
            get { return _message; }
        }

        public string StartedBy
        {
            get { return _startedBy; }
        }
    }
}