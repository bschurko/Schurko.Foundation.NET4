using Schurko.Foundation.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, T item) => collection.Concat((IEnumerable<T>)new T[1]
        {
      item
        });

        public static IEnumerable<Type> GetTypes(this IEnumerable<object> objIEnumerable) => objIEnumerable.Select(item => item.GetType());

        public static bool ContainsType<T>(this IEnumerable<object> objIEnumerable) => objIEnumerable.OfType<T>().Any();

        public static IEnumerable<TX> GetByType<T, TX>(this T[] objArray) where TX : class => objArray.OfType<TX>().Select(item => item);

        public static IEnumerable<TX> GetByType<TX>(this IEnumerable objArray) where TX : class => objArray.OfType<TX>().Select(item => item);

        public static T SelectItem<T>(this IEnumerable<T> items, int? position) => items.ToList().SelectItem(position);

        public static T SelectItem<T>(this IList<T> items, int? position)
        {
            position = new int?(position.GetValueOrDefault());
            int count = items.Count;
            int? nullable1 = position;
            int? nullable2 = nullable1.HasValue ? new int?(nullable1.GetValueOrDefault() + 1) : new int?();
            int valueOrDefault = nullable2.GetValueOrDefault();
            return !(count >= valueOrDefault & nullable2.HasValue) ? default : items[position.Value];
        }

        public static T SelectFirstItem<T>(this IEnumerable<T> items) => items != null ? items.SelectItem(new int?()) : default;

        public static T SelectFirstItem<T>(this IList<T> items) => items != null ? items.SelectItem(new int?()) : default;

        public static string ToHexString(this byte[] byteArray) => BitConverter.ToString(byteArray);

        public static IList ReadOnly(this IList sourceList) => new ReadOnlyCollection<object>(sourceList.Cast<object>().ToList());

        public static IList<T> ReadOnly<T>(this IList<T> sourceList) => new ReadOnlyCollection<T>(sourceList);
    }
}
