using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign
{
    class SerivceProxyFactory<T>
    {
        public static T Create(string str, Uri remoteAddress)
        {
            FeignRealProxy<T> realProxy = new FeignRealProxy<T>();
            return (T)realProxy.GetTransparentProxy();
        }
    }
}
