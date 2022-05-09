using System;

#pragma warning disable 8632

namespace C5
{
    /// <summary>
    /// Tentative, to conserve memory in GuardedCollectionValueBase
    /// This should really be nested in Guarded collection value, only have a guardereal field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    internal sealed class ProxyEventBlock<T>
    {
        readonly ICollectionValue<T> proxy, real;

        internal ProxyEventBlock(ICollectionValue<T> proxy, ICollectionValue<T> real)
        { this.proxy = proxy; this.real = real; }

        event CollectionChangedHandler<T> CollectionChangedInner;

        CollectionChangedHandler<T>? collectionChangedProxy;
        internal event CollectionChangedHandler<T> CollectionChanged
        {
            add
            {
                if (CollectionChangedInner == null)
                {
                    if (collectionChangedProxy == null)
                    {
                        collectionChangedProxy = delegate { CollectionChangedInner(proxy); };
                    }

                    real.CollectionChanged += collectionChangedProxy;
                }
                CollectionChangedInner += value;
            }
            remove
            {
                CollectionChangedInner -= value;
                if (CollectionChangedInner == null)
                {
                    real.CollectionChanged -= collectionChangedProxy;
                }
            }
        }

        event CollectionClearedHandler<T> CollectionClearedInner;

        CollectionClearedHandler<T>? collectionClearedProxy;
        internal event CollectionClearedHandler<T> CollectionCleared
        {
            add
            {
                if (CollectionClearedInner == null)
                {
                    if (collectionClearedProxy == null)
                    {
                        collectionClearedProxy = delegate (object sender, ClearedEventArgs e) { CollectionClearedInner(proxy, e); };
                    }

                    real.CollectionCleared += collectionClearedProxy;
                }
                CollectionClearedInner += value;
            }
            remove
            {
                CollectionClearedInner -= value;
                if (CollectionClearedInner == null)
                {
                    real.CollectionCleared -= collectionClearedProxy;
                }
            }
        }

        event ItemsAddedHandler<T> ItemsAddedInner;

        ItemsAddedHandler<T>? itemsAddedProxy;
        internal event ItemsAddedHandler<T> ItemsAdded
        {
            add
            {
                if (ItemsAddedInner == null)
                {
                    if (itemsAddedProxy == null)
                    {
                        itemsAddedProxy = delegate (object sender, ItemCountEventArgs<T> e) { ItemsAddedInner(proxy, e); };
                    }

                    real.ItemsAdded += itemsAddedProxy;
                }
                ItemsAddedInner += value;
            }
            remove
            {
                ItemsAddedInner -= value;
                if (ItemsAddedInner == null)
                {
                    real.ItemsAdded -= itemsAddedProxy;
                }
            }
        }

        event ItemInsertedHandler<T> ItemInsertedInner;

        ItemInsertedHandler<T>? itemInsertedProxy;
        internal event ItemInsertedHandler<T> ItemInserted
        {
            add
            {
                if (ItemInsertedInner == null)
                {
                    if (itemInsertedProxy == null)
                    {
                        itemInsertedProxy = delegate (object sender, ItemAtEventArgs<T> e) { ItemInsertedInner(proxy, e); };
                    }

                    real.ItemInserted += itemInsertedProxy;
                }
                ItemInsertedInner += value;
            }
            remove
            {
                ItemInsertedInner -= value;
                if (ItemInsertedInner == null)
                {
                    real.ItemInserted -= itemInsertedProxy;
                }
            }
        }

        event ItemsRemovedHandler<T>? ItemsRemovedInner;

        ItemsRemovedHandler<T>? itemsRemovedProxy;
        internal event ItemsRemovedHandler<T> ItemsRemoved
        {
            add
            {
                if (ItemsRemovedInner == null)
                {
                    if (itemsRemovedProxy == null)
                    {
                        itemsRemovedProxy = delegate (object sender, ItemCountEventArgs<T> e) { ItemsRemovedInner?.Invoke(proxy, e); };
                    }

                    real.ItemsRemoved += itemsRemovedProxy;
                }
                ItemsRemovedInner += value;
            }
            remove
            {
                ItemsRemovedInner -= value;
                if (ItemsRemovedInner == null)
                {
                    real.ItemsRemoved -= itemsRemovedProxy;
                }
            }
        }

        event ItemRemovedAtHandler<T> ItemRemovedAtInner;

        ItemRemovedAtHandler<T>? itemRemovedAtProxy;
        internal event ItemRemovedAtHandler<T> ItemRemovedAt
        {
            add
            {
                if (ItemRemovedAtInner == null)
                {
                    if (itemRemovedAtProxy == null)
                    {
                        itemRemovedAtProxy = delegate (object sender, ItemAtEventArgs<T> e) { ItemRemovedAtInner(proxy, e); };
                    }

                    real.ItemRemovedAt += itemRemovedAtProxy;
                }
                ItemRemovedAtInner += value;
            }
            remove
            {
                ItemRemovedAtInner -= value;
                if (ItemRemovedAtInner == null)
                {
                    real.ItemRemovedAt -= itemRemovedAtProxy;
                }
            }
        }
    }
}