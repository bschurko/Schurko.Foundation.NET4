
using Schurko.Foundation.Concurrent.WorkerPool.Models;
using System.Threading.Tasks;


#nullable enable
namespace Schurko.Foundation.Concurrent.WorkerPool
{
    public interface IWorker<T> where T : IJob
    {
        string Id { get; }

        void Start();

        void Stop();

        Task Thread();
    }
}
