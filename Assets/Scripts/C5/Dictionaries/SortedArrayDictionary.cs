using System;
using System.Collections.Generic;

namespace C5
{
    [Serializable]
    internal class SortedArrayDictionary<K, V> : SortedDictionaryBase<K, V>
    {
        #region Constructors

        public SortedArrayDictionary() : this(Comparer<K>.Default, EqualityComparer<K>.Default) { }

        /// <summary>
        /// Create a red-black tree dictionary using an external comparer for keys.
        /// </summary>
        /// <param name="comparer">The external comparer</param>
        public SortedArrayDictionary(IComparer<K> comparer) : this(comparer, new ComparerZeroHashCodeEqualityComparer<K>(comparer)) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="equalityComparer"></param>
        public SortedArrayDictionary(IComparer<K> comparer, IEqualityComparer<K> equalityComparer)
            : base(comparer, equalityComparer)
        {
            pairs = sortedpairs = new SortedArray<KeyValuePair<K, V>>(new KeyValuePairComparer<K, V>(comparer));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="equalityComparer"></param>
        /// <param name="capacity"></param>
        public SortedArrayDictionary(int capacity, IComparer<K> comparer, IEqualityComparer<K> equalityComparer)
            : base(comparer, equalityComparer)
        {
            pairs = sortedpairs = new SortedArray<KeyValuePair<K, V>>(capacity, new KeyValuePairComparer<K, V>(comparer));
        }
        #endregion
    }
}