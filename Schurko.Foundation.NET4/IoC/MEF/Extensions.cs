
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.IoC.MEF
{
    public static class Extensions
    {
        #region -- Extensions --

        /// <summary>
        /// The append the provided instances to resolve all.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static IEnumerable<TServiceType> AppendToResolveAll<TServiceType>(this IEnumerable<TServiceType> target, params Func<TServiceType>[] args)
        {
            if (args.Length <= 0) return target;

            return target == null ? args.Select(a => a()) : target.Concat(args.Select(a => a()));
        }

        /// <summary>
        /// This will prefix the provide instances to the resolved list.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public static IEnumerable<TServiceType> PrefixToResolveAll<TServiceType>(this IEnumerable<TServiceType> target, params Func<TServiceType>[] args)
        {
            if (args.Length <= 0) return target;

            return target == null ? args.Select(a => a()) : args.Select(a => a()).Concat(target);
        }

        /// <summary>
        /// The filter on metadata.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <example>
        /// Get fast vehicles
        /// var fastVehicles = DependencyInjector.ResolveAllWithMetaData{IVehicle}().FilterOnMetadata(v => (string)v.Metadata["Speed"] == "Fast");
        /// // NOTE the cast in the filter to string. Generic Metadata is stored in IDictionary{string,object} so you must cast before you compare.
        /// </example>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        /// </returns>
        public static IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> Filter<TServiceType>(this IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> source, Func<Lazy<TServiceType, IDictionary<string, object>>, bool> filter)
        {
            return source.Where(filter);
        }

        /// <summary>
        /// Extension applies a filter of an AllWithMetaData, simple Key, Value filter.
        /// NOTE: Now supports isMultiple=true
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <typeparam name="TValueType">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        /// </returns>
        public static IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> Filter<TServiceType, TValueType>(this IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> source, string key, TValueType value) where TValueType : IComparable
        {
            if (value == null)
                throw new ArgumentNullException(
                    "value",
                    "The 'value' for Filter was passed in as null.You must pass a valid filter value, example: value = value ?? \"unknown\"");
            return source.Where(instance => instance.Metadata.Any(meta => string.Equals(meta.Key, key, StringComparison.CurrentCultureIgnoreCase) && (meta.Value is IEnumerable<TValueType> ? ((IEnumerable<TValueType>)meta.Value).Any(t => value.CompareTo(t) == 0) : value.CompareTo(meta.Value) == 0)));
        }

        /// <summary>
        /// Casts an AllWithMetaData IEnumerable to IEnumerable{IServiceType}
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="defaultsIfNotResolved">
        /// The defaults if not resolved.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        /// </returns>
        public static IEnumerable<TServiceType> ResolveAll<TServiceType>(this IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> source, params Func<TServiceType>[] defaultsIfNotResolved)
        {
            var sourceReturn = source.Select(a => a.Value).ToArray();
            return sourceReturn.Any() ? sourceReturn : defaultsIfNotResolved.Select(a => a());
        }

        /// <summary>
        /// Resolve FirstOrDefault TServiceType from an AllWithMetaData
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="defaultIfNotResolved">
        /// The default if not resolved.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TServiceType"/>.
        /// </returns>
        public static TServiceType Resolve<TServiceType>(this IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> source, Func<TServiceType> defaultIfNotResolved = null) where TServiceType : class
        {
            return source.Select(a => a.Value).FirstOrDefault() ?? (defaultIfNotResolved ?? (() => default))();
        }

        /// <summary>
        /// Resolve FirstOrDefault TServiceType from an AllWithMetaData
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <typeparam name="TDefaultType">
        /// </typeparam>
        /// <returns>
        /// The <see cref="TServiceType"/>.
        /// </returns>
        public static TServiceType Resolve<TServiceType, TDefaultType>(
            this IEnumerable<Lazy<TServiceType, IDictionary<string, object>>> source)
            where TServiceType : class
            where TDefaultType : TServiceType, new()
        {
            return source.Select(a => a.Value).FirstOrDefault() ?? new TDefaultType();
        }

        /// <summary>
        /// Filter on custom metadata attributes.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <typeparam name="TServiceType">
        /// </typeparam>
        /// <typeparam name="TMetaDataType">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        /// </returns>
        public static IEnumerable<TServiceType> Filter<TServiceType, TMetaDataType>(this IEnumerable<Lazy<TServiceType, TMetaDataType>> source, Func<Lazy<TServiceType, TMetaDataType>, bool> filter)
        {
            return source.Where(filter).Select(t => t.Value);
        }

        #endregion 
    }
}
