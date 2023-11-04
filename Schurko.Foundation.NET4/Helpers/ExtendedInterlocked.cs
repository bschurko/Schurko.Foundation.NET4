using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schurko.Foundation.Utilities
{
    /// <summary>
    /// An extension to the interlocked funcionality in .NET for ranges
    /// </summary>
    public static class ExtendedInterlocked
    {

        /// <summary>
        /// Increments a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start of the range (inclusive).</param>
        /// <param name="end">The end of the range (inclusive).</param>
        /// <returns>The incremented result.</returns>
        public static int Increment(ref int value, int start, int end)
        {
            SpinWait spinWait = new SpinWait();
            do
            {
                int v = value;
                if (Interlocked.CompareExchange(ref value, v >= end ? start : v + 1, v) == v)
                    return v;

                spinWait.SpinOnce();
            } while (true);
        }


        /// <summary>
        /// Decrement a value whilst keeping it inside a range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="start">The start of the range (inclusive).</param>
        /// <param name="end">The end of the range (inclusive).</param>
        /// <returns>The decremented result.</returns>
        public static int Decrement(ref int value, int start, int end)
        {
            SpinWait spinWait = new SpinWait();
            do
            {
                int v = value;
                if (Interlocked.CompareExchange(ref value, v - 1 <= start ? end : v - 1, v) == v)
                    return v;

                spinWait.SpinOnce();
            } while (true);
        }
    }
}
