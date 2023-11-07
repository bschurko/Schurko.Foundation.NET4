 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Schurko.Foundation.Data;
using Schurko.Foundation.Logging;
using Schurko.Foundation.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Logger
{
    [TestClass]
    public class DbLoggerTests
    {
        [TestMethod]
        public void TestLogger()
        {
            var connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
     
            IConnectionString conn = new ConnectionString("DefaultConnection", connStr);
            IDbLogger logger = new DbLogger(conn);

            logger.LogInfo("INFO LOG", "Message Template");
            logger.LogWarn("WARN LOG");
            logger.LogDebug("DEBUG LOG", "Message Template");
            logger.LogException("EXCEPTION LOG", new Exception("Exception Error"));
            logger.LogVerbose("VERBOSE LOG", "Message Template", "Properties");
            logger.LogFatal("FATAL LOG", new Exception("Exception Error"), "Message Template FATAL");
            Thread.Sleep(20000);
        }
    }
}
