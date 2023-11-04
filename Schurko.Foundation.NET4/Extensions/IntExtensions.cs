using System;
using System.ComponentModel;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class IntExtensions
    {
        public static bool AsBool(this int value) => value == 0;

        public static int ToInt(this string parseValue, int defaultValue = -1)
        {
            int result;
            return !int.TryParse(parseValue, out result) ? defaultValue : result;
        }

        public static T ToEnum<T>(this int index) => Enum.IsDefined(typeof(T), index) ? (T)Enum.ToObject(typeof(T), index) : throw new InvalidEnumArgumentException(nameof(index), index, typeof(T));

        public static T ToEnum<T>(this int index, T defaultValue)
        {
            try
            {
                return index.ToEnum<T>();
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
