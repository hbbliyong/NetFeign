using NetFeign.Attribute;
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
    class Program
    {
        static void Main(string[] args)
        {
            //ICalculator calculator = SerivceProxyFactory<ICalculator>.Create("", new Uri("http://localhost/Artech.WcfFrameworkSimulator/Calculator.aspx"));
            //var result = calculator.Add(1, 2);
            //Console.WriteLine("x + y = {2} when x = {0} and y = {1}", 1, 2, result);
        }

    }
    class MyRealProxy<T> : RealProxy
    {
        public MyRealProxy() : base(typeof(T))
        {

        }
        private readonly T decorated;
        private Func<object[], bool> filter;
        public Func<object[], bool> Filter
        {
            get { return filter; }
            set
            {
                if (value == null)
                    filter = x => true;
                else
                    filter = value;
            }
        }
        public override IMessage Invoke(IMessage msg)
        {
            var methodCall = msg as IMethodCallMessage;
            if (filter(methodCall.InArgs)) OnBeforeExecute(methodCall);
            try
            {
                var methodInfo = methodCall.MethodBase as MethodInfo;
                var result = methodInfo.Invoke(decorated, methodCall.InArgs);
                if (filter(methodCall.InArgs)) OnAfterExecute(methodCall);
                return new ReturnMessage(result, null, 0, null, methodCall);
            }
            catch (Exception ex)
            {
                if (filter(methodCall.InArgs)) OnErrorExecute(methodCall);
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
