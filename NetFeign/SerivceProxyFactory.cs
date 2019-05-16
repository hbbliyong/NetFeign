using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign
{
    public class SerivceProxyFactory<T>
    {
        public static T Create()
        {
            FeignRealProxy<T> realProxy = new FeignRealProxy<T>();
            return (T)realProxy.GetTransparentProxy();
        }
    }
}
