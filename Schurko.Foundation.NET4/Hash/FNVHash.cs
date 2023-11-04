
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


#nullable enable
namespace Schurko.Foundation.Hash
{
    public sealed class FNVHash
    {
        private const uint FnvPrime32 = 16777619;
        private const ulong FnvPrime64 = 1099511628211;
        private const uint FnvOffset32 = 2166136261;
        private const ulong FnvOffset64 = 14695981039346656037;

        public static uint Hash32(IEnumerable<byte> data)
        {
            uint num1 = 2166136261;
            foreach (byte num2 in data)
            {
                num1 ^= num2;
                num1 *= 16777619U;
            }
            return num1;
        }

        public static ulong Hash64(IEnumerable<byte> data)
        {
            ulong num1 = 14695981039346656037;
            foreach (byte num2 in data)
            {
                num1 ^= num2;
                num1 *= 1099511628211UL;
            }
            return num1;
        }

        public static ulong HashString(params object[] data) => 
            Hash64(Encoding.UTF8.GetBytes(data.Select(e => e.ToString()).OrderBy(s => s).Aggregate((a, b) 
                => string.Format("{0}_{1}", a, b))));
    }
}
