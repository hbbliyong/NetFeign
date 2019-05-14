using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign.Attribute
{
    public class RequestMapping:BaseAttribute
    {
        public RequestMapping(string value)
        {
            this.Value = value;
        }

        public RequestMethod RequestMethod { get; set; }
    }
}
