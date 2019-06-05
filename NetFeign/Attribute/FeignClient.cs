using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign.Attribute
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class FeignClient : BaseAttribute
    {
        public string Name { get; set; }
        public string BaseUrl { get; set; }
    }
}
