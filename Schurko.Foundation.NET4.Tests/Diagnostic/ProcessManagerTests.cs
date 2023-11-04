using Schurko.Foundation.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.Diagnostic
{
    [TestClass]
    public class ProcessManagerTests
    {
        [TestMethod]
        public void Process_Manager_Default_Tests()
        {
            var pid = ProcessManager.StartProcess("notepad.exe", string.Empty, @"C:\\Windows\\System32");
            Assert.IsTrue(pid > 0);
            ProcessManager.KillProcess(pid);
        }
    }
}
