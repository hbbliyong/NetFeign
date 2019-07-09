using NetFeign;
using NetFeign.Attribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ICalculator calculator = ServiceProxyFactory<ICalculator>.Create();
            var result12 = calculator.Add(1, 2);
            // var data = calculator.AddUsers(new List<User>() { new User() });
            //  Console.WriteLine("x + y = {2} when x = {0} and y = {1}", 1, 2, result);

            IProjectService elementManager = ServiceProxyFactory<IProjectService>.Create();
            dynamic user = new DynamicBase();
            user.userId = "B7ED32BF48754DC79D99E6157A98E136";
           // ResponseData<List<DynamicBase>> result = elementManager.GetProjectsByUserId(user);
            ResponseData<List<DynamicBase>> result1 = elementManager.GetProjectsByUserId(new T1() { UserId = "B7ED32BF48754DC79D99E6157A98E136" });
        }
        public class T1
        {
            public string UserId { get; set; }
        }
        [FeignClient(Name = "测试", BaseUrl = "http://localhost:8080")]
        public interface ICalculator
        {
            [HttpGet("/add")]
            double Add(double a, double b);
            [RequestMapping("/user", RequestMethod = RequestMethod.Get)]
            User GetUser();
            [RequestMapping("/user", RequestMethod = RequestMethod.Post)]
            User AddUsers([RequestBody]List<User> users);
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        [NetFeign.Attribute.FeignClient(BaseUrl = "http://192.168.1.100:8080")]
        public interface IProjectService
        {
            [RequestMapping("project/getprojectsbyuserid", RequestMethod = RequestMethod.Post)]
            ResponseData<List<DynamicBase>> GetProjectsByUserId([RequestBody]DynamicBase userid);

            [HttpPost("project/getprojectsbyuserid")]
            ResponseData<List<DynamicBase>> GetProjectsByUserId([RequestBody]T1 userid);

            [RequestMapping("model/checkmodelid", RequestMethod = RequestMethod.Post)]
            ResponseData<dynamic> CheckModelId([RequestBody]DynamicBase data);
        }
        public class DynamicBase : DynamicObject
        {
            [JsonExtensionData]
            private Dictionary<string, object> _dynamicMembers = new Dictionary<string, object>();

            public DynamicBase()
            {

            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                if (_dynamicMembers.ContainsKey(binder.Name) && _dynamicMembers[binder.Name] is Delegate @delegate)
                {
                    result = @delegate.DynamicInvoke(args);
                    return true;
                }
                return base.TryInvokeMember(binder, args, out result);
            }
            public override IEnumerable<string> GetDynamicMemberNames() => _dynamicMembers.Keys;
            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                if (_dynamicMembers.ContainsKey(binder.Name))
                {
                    _dynamicMembers[binder.Name] = value;
                }
                else
                {
                    _dynamicMembers.Add(binder.Name, value);
                }
                return true;
                // return base.TrySetMember(binder, value);
            }
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                if (_dynamicMembers.ContainsKey(binder.Name))
                {
                    result = _dynamicMembers[binder.Name];
                    return true;
                }

                return base.TryGetMember(binder, out result);
            }
            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();

                foreach (var addItem in _dynamicMembers)
                {
                    Type itemType = addItem.Value.GetType();
                    Type genericType = itemType.IsGenericType ? itemType.GetGenericTypeDefinition() : null;
                    if (genericType != null)
                    {
                        if (genericType != typeof(Func<>) && genericType != typeof(Action<>))
                        {
                            builder.AppendFormat("{0}:{1}{2}", addItem.Key, addItem.Value, Environment.NewLine);
                        }
                    }
                    else
                    {
                        builder.AppendFormat("{0}:{1}{2}", addItem.Key, addItem.Value, Environment.NewLine);
                    }
                }
                return builder.ToString();
            }
            public string ToJson()
            {
                if (this == null) return "";
                return JsonConvert.SerializeObject(this);
            }
        }

    }
    public class ResponseData<T>
    {
        public string State { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public bool IsSuccess { get { return State?.ToLower() == "success"; } }
    }
}
