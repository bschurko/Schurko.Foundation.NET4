using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schurko.Foundation.IoC.MEF;
using Schurko.Foundation.Tests.Interfaces;

namespace Schurko.Foundation.Tests.MEF
{
    /// <summary>
    /// The automatic resolver tests.
    /// </summary>
    [TestClass]
    public class AutomaticResolverTests
    {
        /// <summary>
        /// Example of how you can use Import and ImportingConstructor to hookup your dependencies
        /// </summary>
        [TestMethod]
        public void ImportingConstructorTest()
        {
            var myObject = DependencyInjector.Resolve<IObjectA>();

            Assert.AreEqual("Parent", myObject.Name);
            Assert.AreEqual("Child", myObject.SubObject.Name);
            Assert.AreEqual("ObjectC", myObject.SubObject.SubItem1.Name);
            Assert.AreEqual("ObjectD", myObject.SubObject.SubItem2.Name);
        }
    }
}
