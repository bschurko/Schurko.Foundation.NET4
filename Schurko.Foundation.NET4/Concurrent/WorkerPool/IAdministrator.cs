
using Schurko.Foundation.Concurrent.WorkerPool.Models;
using System;


#nullable enable
namespace Schurko.Foundation.Concurrent.WorkerPool
{
    public interface IAdministrator<T> : IDisposable where T : IJob
    {
        int NoOfWorkers { get; }

        void SubmitJob(T job);

        void AttachWorker();

        void DetachWorker();
    }
}
