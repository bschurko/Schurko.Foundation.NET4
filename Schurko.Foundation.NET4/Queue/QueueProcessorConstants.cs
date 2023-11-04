using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace Schurko.Foundation.Queue
{
    /// <summary>
    /// State of the Queue Processor
    /// </summary>
    public enum QueueProcessState
    {
        /// <summary>
        /// Queue processor is idle.
        /// </summary>
        Idle,


        /// <summary>
        /// Queue processor is busy.
        /// </summary>
        Busy,


        /// <summary>
        /// Queue processor is stopped.
        /// </summary>
        Stopped
    };
}
