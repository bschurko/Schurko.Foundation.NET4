using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace Schurko.Foundation.Queue
{
    /// <summary>
    /// Interface for a persistance queue repository.
    /// </summary>
    /// <typeparam name="T">Type of items to store in repository.</typeparam>
    public interface IQueueRepository<T>
    {
        /// <summary>
        /// Saves the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        void Save(IList<T> items);


        /// <summary>
        /// Loads all.
        /// </summary>
        /// <returns>List of all items.</returns>
        IList<T> LoadAll();


        /// <summary>
        /// Loads the batch.
        /// </summary>
        /// <returns>List of all items.</returns>
        IList<T> LoadBatch();
    }
}
