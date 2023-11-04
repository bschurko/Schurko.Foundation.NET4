
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class DictionaryExtensions
    {

        /// <summary>
        /// Convert to delimited text.
        /// </summary>
        /// <param name="delimitedText"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ToDictionary(this string delimitedText, char delimeter)
        {
            IDictionary<string, string> items = new Dictionary<string, string>();
            string[] tokens = delimitedText.Split(delimeter);

            // Check
            if (tokens == null) return new Dictionary<string, string>(items);

            foreach (string token in tokens)
            {
                items[token] = token;
            }
            return new Dictionary<string, string>(items);
        }


        /// <summary>
        /// Parses a delimited list of items into a string[].
        /// </summary>
        /// <param name="delimitedText"></param>
        /// <param name="delimeter"></param>
        /// <returns></returns>
        public static string[] ToStringList(this string delimitedText, char delimeter)
        {
            if (string.IsNullOrEmpty(delimitedText))
                return null;

            string[] tokens = delimitedText.Split(delimeter);
            return tokens;
        }

        public static IDictionary ReadOnly(this IDictionary sourceDictionary)
        {
            IDictionary<object, object> sourceDictionary1 = new Dictionary<object, object>();
            foreach (DictionaryEntry source in sourceDictionary)
                sourceDictionary1.Add(new KeyValuePair<object, object>(source.Key, source.Value));
            return new ReadOnlyDictionary<object, object>(sourceDictionary1).ToDictionary(k => k.Key, v => v.Value);
        }

        //public static TValue GetValue<TKey, TValue>(
        //  this IDictionary<TKey, TValue> sourceDictionary,
        //  TKey key,
        //  TValue defaultValue = null)
        //{
        //  TValue obj;
        //  return !sourceDictionary.TryGetValue(key, out obj) ? defaultValue : obj;
        //}

        public static IDictionary<TKey, TValue> ReadOnly<TKey, TValue>(
          this IDictionary<TKey, TValue> sourceDictionary)
        {
            return new ReadOnlyDictionary<TKey, TValue>(sourceDictionary);
        }
    }
}
