using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsimovDeploy.Annotations.Web.ViewModels
{
    public class AnnotationViewModel
    {
        public string Id { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime started { get; set; }
        public string startedBy { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public bool completed { get; set; }
        public DateTime finished { get; set; }
        public string Message { get; set; }
    }
}
