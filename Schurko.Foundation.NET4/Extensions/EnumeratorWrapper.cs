using System;
using System.Collections;
using System.Collections.Generic;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public class EnumeratorWrapper<T> : IEnumerator<T>, IEnumerator, IDisposable
    {
        private readonly IEnumerator _enumerator;

        public EnumeratorWrapper(IEnumerator enumerator) => _enumerator = enumerator;

        public T Current => (T)_enumerator.Current;

        public void Dispose()
        {
            if (!(_enumerator is IDisposable))
                return;
            ((IDisposable)_enumerator).Dispose();
        }

        object IEnumerator.Current => _enumerator.Current;

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();
    }
}
