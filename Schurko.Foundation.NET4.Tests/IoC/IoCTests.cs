using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schurko.Foundation.Helpers;
using Schurko.Foundation.Tests.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.IoC
{
    [TestClass]
    public class IoCTests
    {
        [TestMethod]
        public void Init_Dependency_Injection_Test()
        {
            IHost host = Foundation.IoC.DI.IoC.InitDependency((serviceCollection)  => 
            {
                serviceCollection.AddTransient<IHeartbeatWriter, HeartbeatWriter>();
                serviceCollection.AddTransient<IProduct, Product>();
            });

            IHeartbeatWriter writer = host.Services.GetService<IHeartbeatWriter>();
            IProduct oa = host.Services.GetService<IProduct>();

            Assert.IsNotNull(oa);
            Assert.IsNotNull(writer);
        }
    }
}
