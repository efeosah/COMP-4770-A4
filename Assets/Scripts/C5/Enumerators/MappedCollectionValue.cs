using System;
using System.Collections.Generic;

namespace C5
{
    [Serializable]
    internal abstract class MappedCollectionValue<T, V> : CollectionValueBase<V>
    {
        readonly ICollectionValue<T> collectionValue;

        public abstract V Map(T item);

        protected MappedCollectionValue(ICollectionValue<T> collectionValue)
        {
            this.collectionValue = collectionValue;
        }

        public override V Choose() { return Map(collectionValue.Choose()); }

        public override bool IsEmpty => collectionValue.IsEmpty;

        public override int Count => collectionValue.Count;

        public override Speed CountSpeed => collectionValue.CountSpeed;

        public override IEnumerator<V> GetEnumerator()
        {
            foreach (var item in collectionValue)
            {
                yield return Map(item);
            }
        }
    }
}