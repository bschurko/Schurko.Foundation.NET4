

using Schurko.Foundation.Concurrent.WorkerPool.Models;
using Schurko.Foundation.Scheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Tests.DemoWorkerPool
{
    [TestClass]
    public class AdminPoolTests
    {
        [TestMethod]
        public void AdministratorDemo_Test()
        {
            var admin = new DemoAdministrator();

            while (true)
            {
                IJob job = new JobEntry()
                {
                    Input = "Input Data",
                    Number = new Random().Next(1, 10),
                    Exception = null
                };

                admin.SubmitJob(job);
            }
             
        }
    }
}
