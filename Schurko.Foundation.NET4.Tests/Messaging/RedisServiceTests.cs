using Newtonsoft.Json;

using Schurko.Foundation.Messaging.Redis;
using Schurko.Foundation.Tests.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Messaging
{
    [TestClass]
    public class RedisServiceTests
    {
        [TestMethod]
        public void Redis_Basic_Test()
        {
            string hostName = "localhost";
            string port = "6379";

            RedisService service = new RedisService(hostName, port);

            ObjectA a = new ObjectA(null);
            service.SetObjectValue<ObjectA>("ObjectA", a);
            var aa = service.GetObjectData<ObjectA>("ObjectA");
            Assert.IsNotNull(aa);

            ObjectB b = new ObjectB();
            service.SetStringValue("ObjectB", JsonConvert.SerializeObject(b));
            var bb = service.GetStringValue("ObjectB");
            var ob = JsonConvert.DeserializeObject<ObjectB>(bb);
            Assert.IsNotNull(ob);
            

        }
    }
}
