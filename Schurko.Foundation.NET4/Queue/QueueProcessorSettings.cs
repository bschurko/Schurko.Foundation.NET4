using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace Schurko.Foundation.Queue
{
    /// <summary>
    /// Settings for the processing queue.
    /// </summary>
    public class QueueProcessorSettings
    {
        /// <summary>
        /// Gets or sets the number to process per dequeue.
        /// </summary>
        /// <value>The number to process per dequeue.</value>
        public int NumberToProcessPerDequeue { get; set; }
    }
}
