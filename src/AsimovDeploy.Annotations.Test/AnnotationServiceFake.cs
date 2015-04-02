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
using System.Linq;
using AsimovDeploy.Annotations.Agent.Framework.Domain;
using AsimovDeploy.Annotations.Agent.Framework.Domain.Services;

namespace AsimovDeploy.Annotations.Test
{
    public class AnnotationServiceFake : IAnnotationService
    {
        private readonly List<Annotation> _storage = new List<Annotation>();

        private Annotation _annotation;

        public AnnotationServiceFake()
        {
        }

        public AnnotationServiceFake(Annotation annotation)
        {
            _storage.Add(annotation);
        }

        public void SaveAnnotation(Annotation annotation)
        {
            var savedAnnotation = _storage.FirstOrDefault(x => x.Id == annotation.Id);
            if (savedAnnotation != null)
            {
                _storage.Remove(savedAnnotation);
            }

            _storage.Add(annotation);
            _annotation = annotation;
        }

        public Annotation LoadOrCreate(string correlationId)
        {
            var annotation = _storage.FirstOrDefault(x => x.Id == correlationId);
            if (annotation != null)
            {
                _annotation = annotation;
                return annotation;
            }
            annotation = new Annotation(correlationId);
            _storage.Add(annotation);
            _annotation = annotation;
            return annotation;
        }

        public Annotation Current
        {
            get { return _annotation; }
        }
    }
}