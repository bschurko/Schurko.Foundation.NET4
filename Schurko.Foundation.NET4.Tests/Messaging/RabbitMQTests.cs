using Schurko.Foundation.Messaging.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Messaging
{
    [TestClass]
    public class RabbitMQTests
    {
        [TestMethod]
        public void RabbitMQ_Publish_And_Consume_Data_Test()
        {
            string hostName = "localhost";
            string userName = "guest";
            string password = "guest";
            RabbitMQService service = new RabbitMQService(hostName, userName, password);
            service.Publish("SchurkoQueue", "Hello World!");
            var data = service.Consume("SchurkoQueue");
            Assert.IsTrue(data == "Hello World!");

        }
    }
}
