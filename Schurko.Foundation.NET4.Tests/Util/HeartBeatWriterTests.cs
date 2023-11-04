
using Schurko.Foundation.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Util
{
    [TestClass]
    public class HeartBeatWriterTests
    {
        [TestMethod]
        public void Default_HeartBeatWriter_Tests()
        {
            bool hasStarted = false;
            while (true)
            {
                if (!hasStarted)
                {
                    hasStarted = true;
                    var hbw = new HeartbeatWriter(HeartbeatMode.FileWriter);
                    hbw.Start();
                }

                Task.Delay(3000);
            }
        }
    }
}
