using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Text;
using System.Threading.Tasks;

namespace NetFeign
{
    public class FeignRealProxy<T> : RealProxy
    {
        public FeignRealProxy() : base(typeof(T))
        {

        }

        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;

            try
            {
                var methodInfo = methodCall.MethodBase as MethodInfo;
                //  var result = methodInfo.Invoke(decorated, methodCall.InArgs);
                // if (filter(methodCall.InArgs)) OnAfterExecute(methodCall);
                return new ReturnMessage("", null, 0, null, methodCall);
            }
            catch (Exception ex)
            {
                //if (filter(methodCall.InArgs)) OnErrorExecute(methodCall);
                return new ReturnMessage(ex, methodCall);
            }
        }

        public event EventHandler<IMethodCallMessage> AfterExecute;

        public event EventHandler<IMethodCallMessage> BeforeExecute;

        public event EventHandler<IMethodCallMessage> ErrorExecute;

        private void OnAfterExecute(IMethodCallMessage methodCall)
        {
            if (AfterExecute != null)
            {
                AfterExecute(this, methodCall);
            }
        }

        private void OnBeforeExecute(IMethodCallMessage methodCall)
        {
            if (BeforeExecute != null)
            {
                BeforeExecute(this, methodCall);
            }
        }

        private void OnErrorExecute(IMethodCallMessage methodCall)
        {
            if (ErrorExecute != null)
            {
                ErrorExecute(this, methodCall);
            }
        }
    }
}
