using System;
using System.Collections;
using System.Collections.Generic;

namespace C5
{
    /// <summary>
    /// A read-only wrapper class for a generic enumerable
    ///
    /// <i>This is mainly interesting as a base of other guard classes</i>
    /// </summary>
    [Serializable]
    public class GuardedEnumerable<T> : IEnumerable<T>
    {
        #region Fields

        readonly IEnumerable<T> enumerable;

        #endregion

        #region Constructor

        /// <summary>
        /// Wrap an enumerable in a read-only wrapper
        /// </summary>
        /// <param name="enumerable">The enumerable to wrap</param>
        public GuardedEnumerable(IEnumerable<T> enumerable)
        { this.enumerable = enumerable; }

        #endregion

        #region System.Collections.Generic.IEnumerable<T> Members

        /// <summary>
        /// Get an enumerator from the wrapped enumerable
        /// </summary>
        /// <returns>The enumerator (itself wrapped)</returns>
        public IEnumerator<T> GetEnumerator()
        { return new GuardedEnumerator<T>(enumerable.GetEnumerator()); }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}