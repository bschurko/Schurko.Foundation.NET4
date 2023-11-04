using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace Schurko.Foundation.Queue
{
    /// <summary>
    /// Action context when performing various actions on the processing queue.
    /// </summary>
    public class QueueProcessActionContext
    {
        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public int BatchSize { get; set; }
    }
}
