using System;
using System.Threading;
using System.Threading.Tasks;
using Schurko.Foundation.Concurrent.WorkerPool;
using Schurko.Foundation.Concurrent.WorkerPool.Models;

namespace Schurko.Foundation.Tests.DemoWorkerPool
{
    public class DemoAdministrator : Administrator<IJob>
    {
        private int _sum;

        public DemoAdministrator()
        {
            _sum = 0;
            for (var i = 0; i < Environment.ProcessorCount; i++)
                AttachWorker();
        }

        public void Clear()
        {
            _sum = 0;
        }

        public override Task JobProcessorAsync(IJob job)
        {
            return Task.Factory.StartNew(() =>
            {
                int number;
                if (!int.TryParse(job.Input, out number))
                {
                    job.Exception = new Exception("invalid key. only number are excepted.");
                }
                else
                {
                    job.Number = number;
                }
            });
        }

        public override void AckJobComplete(IJob job)
        {
            if (job.Exception != null)
            {
                Console.WriteLine(job.Exception.Message);
                return;
            }

            Interlocked.Add(ref _sum, job.Number);
            Console.WriteLine("JobID: '{0}' completed!\r\n Job adds: {1}.  Sum: {2}", job.Id, job.Number, _sum);
        }
    }
}
