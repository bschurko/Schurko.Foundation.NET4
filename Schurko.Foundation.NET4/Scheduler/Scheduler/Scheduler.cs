using Microsoft.Extensions.Hosting;
using Schurko.Foundation.Concurrent.WorkerPool;
using Schurko.Foundation.Concurrent.WorkerPool.Models;
using Schurko.Foundation.Scheduler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Schurko.Foundation.Scheduler.Scheduler
{
    public class Scheduler : Administrator<IJob>
    {
        private readonly IScheduleSettings _scheduleSettings;

        public Scheduler(IScheduleSettings scheduleSettings)
        {
            _scheduleSettings = scheduleSettings;

            for (var i = 0; i < Environment.ProcessorCount; i++)
                AttachWorker();
        }

        public override void AckJobComplete(IJob job)
        {
            if (job.Exception != null)
            {
                Console.WriteLine(job.Exception.Message);
                return;
            }
        }

        public override Task JobProcessorAsync(IJob job)
        {
            return Task.Run(() =>
            {
        
                if (job.JobAction == null)
                {
                    job.Exception = new Exception("JobAction delegate is null and cannot be executed.");
                    return;
                }
                else
                {
                    job.JobAction();
                }
            });
        }
    }
}
