using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schurko.Foundation.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.NET4.Tests.Logger
{
    [TestClass]
    public class LoggerTests
    {
        [TestMethod]
        public void Basic_Logger_Test()
        {
            ILogger logger = new Logging.Logger();
             
            Assert.IsTrue(logger != null);

            logger.LogWarn("Warning");
            logger.Log("Regular log");
            logger.LogWarn("Info Log");
        }
    }
}
