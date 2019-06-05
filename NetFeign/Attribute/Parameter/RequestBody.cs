using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign.Attribute
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class RequestBody : BaseAttribute
    {
    }
}
