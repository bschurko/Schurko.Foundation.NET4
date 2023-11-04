using System;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] ConcatArray<T>(this T[] arr1, T[] arr2)
        {
            T[] dst = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, dst, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, dst, arr1.Length, arr2.Length);
            return dst;
        }

        public static T[] SubArray<T>(this T[] arr, int start, int length = 0)
        {
            if (length == 0)
                length = arr.Length - start;
            T[] dst = new T[length];
            Buffer.BlockCopy(arr, start, dst, 0, length);
            return dst;
        }
    }
}
