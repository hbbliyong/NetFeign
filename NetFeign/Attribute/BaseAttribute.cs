using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign.Attribute
{
   public class BaseAttribute: System.Attribute
    {
        public string Value { get; set; }
    }
}
