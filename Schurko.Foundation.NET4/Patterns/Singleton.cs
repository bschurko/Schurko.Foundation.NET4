using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schurko.Foundation.Patterns
{
    /// <summary>
    /// Singleton implementation using generics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T>
    {
        private static T _item;
        private static object _syncRoot = new object();
        private static Func<T> _creator;


        /// <summary>
        /// Prevent instantiation
        /// </summary>
        private Singleton() { }


        /// <summary>
        /// Initalize with the creator.
        /// </summary>
        /// <param name="creator"></param>
        public static void Init(Func<T> creator)
        {
            _creator = creator;
        }


        /// <summary>
        /// Initialize using instance.
        /// </summary>
        /// <param name="item"></param>
        public static void Init(T item)
        {
            _item = item;
            _creator = new Func<T>(() => item);
        }


        /// <summary>
        /// Get the instance of the singleton item T.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_creator == null)
                    return default;

                if (_item == null)
                {
                    lock (_syncRoot)
                    {
                        _item = _creator();
                    }
                }

                return _item;
            }
        }
    }
}
