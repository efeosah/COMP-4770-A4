using System;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// Default equalityComparer for dictionary entries.
    /// Operations only look at keys and uses an externally defined equalityComparer for that.
    /// </summary>
    [Serializable]
    public sealed class KeyValuePairEqualityComparer<K, V> : IEqualityComparer<KeyValuePair<K, V>>
    {
        readonly IEqualityComparer<K> keyequalityComparer;


        /// <summary>
        /// Create an entry equalityComparer using the default equalityComparer for keys
        /// </summary>
        public KeyValuePairEqualityComparer() { keyequalityComparer = EqualityComparer<K>.Default; }


        /// <summary>
        /// Create an entry equalityComparer from a specified item equalityComparer for the keys
        /// </summary>
        /// <param name="keyequalityComparer">The key equalitySCG.Comparer</param>
        public KeyValuePairEqualityComparer(IEqualityComparer<K> keyequalityComparer)
        {
            this.keyequalityComparer = keyequalityComparer ?? throw new NullReferenceException("Key equality comparer cannot be null");
        }


        /// <summary>
        /// Get the hash code of the entry
        /// </summary>
        /// <param name="entry">The entry</param>
        /// <returns>The hash code of the key</returns>
        public int GetHashCode(KeyValuePair<K, V> entry) { return keyequalityComparer.GetHashCode(entry.Key); }


        /// <summary>
        /// Test two entries for equality
        /// </summary>
        /// <param name="entry1">First entry</param>
        /// <param name="entry2">Second entry</param>
        /// <returns>True if keys are equal</returns>
        public bool Equals(KeyValuePair<K, V> entry1, KeyValuePair<K, V> entry2)
        {
            return keyequalityComparer.Equals(entry1.Key, entry2.Key);
        }
    }
}