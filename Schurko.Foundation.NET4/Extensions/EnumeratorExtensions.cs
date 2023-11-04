
using System.Collections;
using System.Collections.Generic;



namespace Schurko.Foundation.Extensions
{
    public static class EnumeratorExtensions
    {
        public static IEnumerator<T> ConvertTo<T>(this IEnumerator enumerator) => new EnumeratorWrapper<T>(enumerator);
    }
}
