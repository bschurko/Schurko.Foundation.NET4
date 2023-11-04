using Microsoft.EntityFrameworkCore;
using Schurko.Foundation.Concurrent.WorkerPool.Models;
using Schurko.Foundation.Patterns;
using Schurko.Foundation.Tests.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Schurko.Foundation.Scheduler.Scheduler;
using Schurko.Foundation.Scheduler.Interfaces;
using System.Threading.Tasks.Dataflow;
using Schurko.Foundation.Messaging.Redis;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Schurko.Foundation.Tests.Schedule
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public void Schedule_Tests()
        {

            int c = 0;
            Scheduler.Scheduler.Scheduler scheduler = new Scheduler.Scheduler.Scheduler(new ScheduleSettings());
            while (true)
            {
                IJob job = new JobEntry("Input", (int)new Random().NextInt64(0, 100));
                object syncLock = new object();
                string hostName = "localhost";
                string port = "6379";
                RedisService service = new RedisService(hostName, port);
                 
                job.JobAction = async () => {

                    Monitor.Enter(syncLock);
                   
                    var countString = service.GetStringValue("count");
                    if (countString != null && int.TryParse(countString, out c))
                    {
                        c++;
                        service.SetStringValue("count", c.ToString());
                        
                    }
                    else
                    {
                        c = 1;
                        service.SetStringValue("count", c.ToString());
                    }
                  
                    Monitor.Exit(syncLock);

                    await Task.Delay(5000);
                };

                scheduler.SubmitJob(job);
            }
             
        }
    }

   

    public class ScheduleSettings : IScheduleSettings
    {

    }
}
