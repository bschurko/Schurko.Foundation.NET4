
using System;


#nullable enable
namespace Schurko.Foundation.Concurrent.WorkerPool.Models
{
    public interface IJob
    {
        string Id { get; }
        Exception Exception { get; set; }
        string Input { get; set; }
        int Number { get; set; }
        public Action JobAction { get; set; }
    }
}
