

using Microsoft.Extensions.Logging;

using Schurko.Foundation.Concurrent.WorkerPool.Models;
using Schurko.Foundation.Logging;
using Schurko.Foundation.Scheduler.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


#nullable enable
namespace Schurko.Foundation.Concurrent.WorkerPool
{
    /// <summary>
    /// Administrator abstract class.  Administrator recieves the job from client.
    /// </summary>
    /// <typeparam name="T">Job</typeparam>
    public abstract class Administrator<T> : IAdministrator<T> where T : IJob 
    {
        private int _noOfWorker;
        private int _noOfWorkerToHalt;

        private readonly ConcurrentDictionary<string, IWorker<T>> _workers;
        private readonly ConcurrentQueue<T> _jobs;

        private readonly CancellationTokenSource _cancellationTokenSource;

        private readonly SemaphoreSlim _jobSemaphore;
        private readonly SemaphoreSlim _workerToHaltSemaphore;

        private readonly ManualResetEventSlim _submitJobLock;

        private ILoggerFactory loggerFactory = new LoggerFactory();
        private ILogger? _logger;
        private string _connectionString;

        private ILogger Logger => _logger ?? (_logger = loggerFactory.CreateLogger("AdministratorPool"));

        protected Administrator()
        {

            _workers = new ConcurrentDictionary<string, IWorker<T>>();
            _jobs = new ConcurrentQueue<T>();
            _jobSemaphore = new SemaphoreSlim(0);
            _workerToHaltSemaphore = new SemaphoreSlim(0);

            _submitJobLock = new ManualResetEventSlim(true);

            _noOfWorker = 0;
            _noOfWorkerToHalt = 0;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Get number of workers
        /// </summary>
        public int NoOfWorkers { get { return _noOfWorker; } }

        /// <summary>
        /// Client calls this method to submit job
        /// </summary>
        /// <param name="job"></param>
        public void SubmitJob(T job)
        {
            _submitJobLock.Wait();
            Logger.LogInformation(string.Format("Submitting job({0})....", job.Id));
            _jobs.Enqueue(job);
            _jobSemaphore.Release();
            Logger.LogInformation(string.Format("Submitting job({0}) has been submitted...", job.Id));
        }

        /// <summary>
        /// Hire worker to work
        /// </summary>
        public void AttachWorker()
        {
            Logger.LogInformation(string.Format("Attaching worker..."));
            var worker = new Worker<T>(this);
            Logger.LogInformation(string.Format("Worker '{0}' created.", worker.Id));
            if (!_workers.TryAdd(worker.Id, worker))
            {
                if (!_workers.TryAdd(worker.Id, worker))
                    return;
            }

            worker.Start();
            Logger.LogInformation(string.Format("Worker '{0}' started.", worker.Id));
            Interlocked.Increment(ref _noOfWorker);
            Logger.LogInformation(string.Format("{0} of workers reports to administrator", _noOfWorker));
        }

        /// <summary>
        /// Fire worker
        /// </summary>
        public void DetachWorker()
        {
            if (_noOfWorker == 0) return;

            _submitJobLock.Reset();
            _workerToHaltSemaphore.Release(1);
            _jobSemaphore.Release(1);
            _submitJobLock.Set();

            Interlocked.Increment(ref _noOfWorkerToHalt);
        }

        /// <summary>
        /// Job process logic
        /// </summary>
        /// <param name="job">Client requested job</param>
        /// <returns></returns>
        public abstract Task JobProcessorAsync(T job);

        /// <summary>
        /// Worker calls this method to get next job.  Call will get blocked if there are no job.
        /// </summary>
        /// <param name="worker"></param>
        /// <returns></returns>
        public T GetNextJob(IWorker<T> worker)
        {
            while (true)
            {
                _jobSemaphore.Wait(_cancellationTokenSource.Token);

                if (_workerToHaltSemaphore.CurrentCount > 0)
                {
                    _workerToHaltSemaphore.Wait();
                    worker.Stop();
                    Interlocked.Decrement(ref _noOfWorker);
                    return default;
                }

                T job;
                if (_jobs.TryDequeue(out job)) return job;
            }
        }

        /// <summary>
        /// Worker will call when job is completed
        /// </summary>
        /// <param name="job"></param>
        public abstract void AckJobComplete(T job);

        /// <summary>
        /// Dispose object.
        /// Wait for all worker to complete the job and halt all attached workers
        /// </summary>
        public virtual async void Dispose()
        {
            Logger.LogInformation("administrator is disposing....");
            Logger.LogInformation("Stopping Job administrator....");

            _cancellationTokenSource.Cancel();
            _jobSemaphore.Release(_noOfWorker);

            var workerWaitForCompleteTasks = _workers.Values.Select(worker => worker.Thread());

            await Task.WhenAll(workerWaitForCompleteTasks).ContinueWith(task =>
            {
                Logger.LogInformation("Waiting workers to finish job....");
                Logger.LogInformation("Job administrator Stopped....");
            });

            _jobSemaphore.Dispose();
            _workerToHaltSemaphore.Dispose();
            _cancellationTokenSource.Dispose();
        }
    }
}
