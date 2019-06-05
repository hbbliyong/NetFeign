using NetFeign;
using NetFeign.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ICalculator calculator = SerivceProxyFactory<ICalculator>.Create();
          //  var result = calculator.Add(1, 2);
            var data = calculator.AddUsers(new List<User>() { new User() });
          //  Console.WriteLine("x + y = {2} when x = {0} and y = {1}", 1, 2, result);
        }

        [FeignClient(Name = "测试", BaseUrl = "http://localhost:8080")]
        public interface ICalculator
        {
            [RequestMapping("/add", RequestMethod = RequestMethod.Get)]
            double Add(double a, double b);
            [RequestMapping("/user", RequestMethod=RequestMethod.Get)]
            User GetUser();
            [RequestMapping("/user", RequestMethod = RequestMethod.Post)]
            User AddUsers([RequestBody]List<User> users);
        }

        public class User
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

    }
}
