
using Schurko.Foundation.Messaging.EventAggregator;
using Schurko.Foundation.Tests.Implementations;
using Schurko.Foundation.Tests.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Messaging
{
    [TestClass]
    public class EventAggregatorTests
    {
        [TestMethod]
        public void Subscribe_and_Publish_Test()
        {
            IEventAggregator eventAggregator = new EventAggregator(typeof(EventAggregatorTests));

            Action<Type, IObjectA> act = delegate (Type t, IObjectA i)
            {
                Console.WriteLine(i.Name);
            };

            eventAggregator.Subscribe(act);

            eventAggregator.Publish<IObjectA>();
        }

        [TestMethod]
        public void Predicate_Filter_Test()
        {
            List<ObjectA> custList = new List<ObjectA>();
            custList.Add(new ObjectA(new ObjectB()));
            Predicate<ObjectA> hydCustomers = x => x.Name == "Parent";
            ObjectA customer = custList.Find(hydCustomers);
            Console.WriteLine(customer.Name);
        }
    }
}
