
using System;



namespace Schurko.Foundation.Concurrent.WorkerPool.Models
{
    public interface IJob
    {
        string Id { get; }
        Exception Exception { get; set; }
        string Input { get; set; }
        int Number { get; set; }

        Action GetJobAction();
        void SetJobAction(Action value);
    }
}
