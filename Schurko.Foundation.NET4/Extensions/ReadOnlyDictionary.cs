using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public class ReadOnlyDictionary<TKey, TValue> :
      IDictionary<TKey, TValue>,
      ICollection<KeyValuePair<TKey, TValue>>,
      IEnumerable<KeyValuePair<TKey, TValue>>,
      IEnumerable
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> sourceDictionary) => _dictionary = sourceDictionary;

        public void Add(TKey key, TValue value) => throw new InvalidOperationException("Readonly dictionary");

        public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);

        public ICollection<TKey> Keys => new ReadOnlyCollection<TKey>(new List<TKey>(_dictionary.Keys));

        public bool Remove(TKey key) => throw new InvalidOperationException("Readonly dictionary");

        public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);

        public ICollection<TValue> Values => new ReadOnlyCollection<TValue>(new List<TValue>(_dictionary.Values));

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => throw new InvalidOperationException("Readonly dictionary");
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get => this[key];
            set => throw new InvalidOperationException("Readonly dictionary");
        }

        public void Add(KeyValuePair<TKey, TValue> item) => throw new InvalidOperationException("Readonly dictionary");

        public void Clear() => throw new InvalidOperationException("Readonly dictionary");

        public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _dictionary.CopyTo(array, arrayIndex);

        public int Count => _dictionary.Count();

        public bool IsReadOnly => true;

        public bool Remove(KeyValuePair<TKey, TValue> item) => throw new InvalidOperationException("Readonly dictionary");

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
