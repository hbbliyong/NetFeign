using NetFeign.Attribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            var methodInfo = methodCall.MethodBase as MethodInfo;

            var methodAttribute = methodInfo.Attributes;
            var requestMapping = methodInfo.GetCustomAttribute<RequestMapping>();

            var type = methodInfo.DeclaringType;
            var feignClient = type.GetCustomAttribute<FeignClient>();
            try
            {
                var baseUrl = feignClient.BaseUrl;
                var requestParam = "";
                //获取参数
                for (int i = 0; i < methodCall.ArgCount; i++)
                {
                    requestParam += $"{methodCall.GetArgName(i)}={methodCall.GetArg(i)}";
                    if (i != methodCall.ArgCount - 1)
                    {
                        requestParam += "&";
                    }
                }
                var url = $"{baseUrl}{requestMapping.Value}";
                if (requestParam != "") url += "?" + requestParam;
                var result = Get(url);
                var returntype = methodInfo.ReturnType;
                //判断类型是否为基础类型

                var method = this.GetType().GetMethod("ConvertType");
                var m1 = method.MakeGenericMethod(returntype);
                var r1 = m1.Invoke(this, new Object[] { result });
                return new ReturnMessage(r1, null, 0, null, methodCall);

            }
            catch (Exception ex)
            {
                //if (filter(methodCall.InArgs)) OnErrorExecute(methodCall);
                return new ReturnMessage(ex, methodCall);
            }
        }
        public T1 ConvertType<T1>(object val)
        {
            if (val == null) return default(T1);//返回类型的默认值
            Type tp = typeof(T1);
            //泛型Nullable判断，取其中的类型
            if (tp.IsGenericType)
            {
                tp = tp.GetGenericArguments()[0];
            }
            //string直接返回转换
            if (tp.Name.ToLower() == "string")
            {
                return (T1)(val);
            }
           
            if (tp.IsPrimitive)//是否为基本类型
            {
                //反射获取TryParse方法
                var TryParse = tp.GetMethod("TryParse", BindingFlags.Public | BindingFlags.Static, Type.DefaultBinder,
                                            new Type[] { typeof(string), tp.MakeByRefType() },
                                            new ParameterModifier[] { new ParameterModifier(2) });
                var parameters = new object[] { val, Activator.CreateInstance(tp) };
                bool success = (bool)TryParse.Invoke(null, parameters);
                //成功返回转换后的值，否则返回类型的默认值
                if (success)
                {
                    return (T1)parameters[1];
                }
            }
            else//为class对象需要反序列化
            {
              return  JsonConvert.DeserializeObject<T1>(val.ToString());
            }
            return default(T1);
        }
        public string Get(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "Get";
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                string reader = new StreamReader(webResponse.GetResponseStream(), Encoding.GetEncoding("gb2312")).ReadToEnd();
                return reader;
            }
        }
        public string Post(string url, string paramData)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;
            webRequest.ContentType = "application/soap+xml; charset=utf-8";

            string responseContent = string.Empty;
            try
            {
                byte[] byteArray =System.Text.ASCIIEncoding.ASCII .GetBytes(paramData); //转化

                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;
                using (Stream reqStream = webRequest.GetRequestStream())
                {
                    reqStream.Write(byteArray, 0, byteArray.Length);//写入参数
                }
                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        responseContent = sr.ReadToEnd().ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return responseContent;
        }

     
    }
}
