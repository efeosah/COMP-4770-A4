using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameBrains.DataStructures
{
    /// <summary>
    /// A priority queue item whose value is of type
    /// <typeparamref name="TValue"/> and whose priority is of type
    /// <typeparamref name="TPriority"/>.
    /// This might be derived from a tutorial by Jim Mischel.
    /// </summary>
    [ComVisible(false)]
    public struct PriorityQueueItem<TValue, TPriority>
    {
        readonly TPriority _priority;

        readonly TValue _value;

        internal PriorityQueueItem(TValue val, TPriority pri)
        {
            _value = val;
            _priority = pri;
        }

        /// <summary>
        /// Accessor for <see cref="_priority"/>
        /// </summary>
        public TPriority Priority => _priority;

        /// <summary>
        /// Accessor for <see cref="_value"/>
        /// </summary>
        public TValue Value => _value;
    }

    /// <summary>
    /// A Priority Queue class
    /// </summary>
    [ComVisible(false)]
    public class PriorityQueue<TValue, TPriority> : ICollection, IEnumerable<PriorityQueueItem<TValue, TPriority>>
    {
        const int DEFAULT_CAPACITY = 16;
        int _capacity;
        Comparison<TPriority> _compareFunc;
        PriorityQueueItem<TValue, TPriority>[] _items;
        int _numItems;
        int _prioritySign;

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the default comparer and
        /// is ordered high first.
        /// <see cref="IComparer"/>.
        /// </summary>
        public PriorityQueue()
            : this(DEFAULT_CAPACITY, Comparer<TPriority>.Default, PriorityOrder.HighFirst)
        {
        }
		
		/// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the default comparer and
        /// is ordered as specified.
        /// <see cref="IComparer"/>.
        /// </summary>
        /// <param name="priorityOrder">
        /// The priority order.
        /// </param>
        public PriorityQueue(PriorityOrder priorityOrder)
            : this(DEFAULT_CAPACITY, Comparer<TPriority>.Default, priorityOrder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the given initial capacity, and uses the default comparer and
        /// is ordered high first.
        /// </summary>
        /// <param name="initialCapacity">
        /// </param>
        public PriorityQueue(int initialCapacity)
            : this(initialCapacity, Comparer<TPriority>.Default, PriorityOrder.HighFirst)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the given comparer and
        /// is ordered high first.
        /// </summary>
        /// <param name="comparer">
        /// </param>
        public PriorityQueue(IComparer<TPriority> comparer)
            : this(DEFAULT_CAPACITY, comparer, PriorityOrder.HighFirst)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the given initial capacity, and uses the given comparer and
        /// is ordered high first.
        /// </summary>
        /// <param name="initialCapacity">
        /// </param>
        /// <param name="comparer">
        /// </param>
        public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer)
        {
            Init(initialCapacity, comparer.Compare, PriorityOrder.HighFirst);
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the given comparison and
        /// is ordered high first.
        /// </summary>
        /// <param name="comparison">
        /// </param>
        public PriorityQueue(Comparison<TPriority> comparison)
            : this(DEFAULT_CAPACITY, comparison, PriorityOrder.HighFirst)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the given initial capacity, and uses the given comparison and
        /// is ordered high first.
        /// </summary>
        /// <param name="initialCapacity">
        /// </param>
        /// <param name="comparison">
        /// </param>
        public PriorityQueue(int initialCapacity, Comparison<TPriority> comparison)
        {
            Init(initialCapacity, comparison, PriorityOrder.HighFirst);
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the given initial capacity, and uses the default comparer and
        /// has the given priority order.
        /// </summary>
        public PriorityQueue(int initialCapacity, PriorityOrder priorityOrder)
            : this(initialCapacity, Comparer<TPriority>.Default, priorityOrder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the given comparer and
        /// has the given priority order.
        /// </summary>
        public PriorityQueue(IComparer<TPriority> comparer, PriorityOrder priorityOrder)
            : this(DEFAULT_CAPACITY, comparer, priorityOrder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the given initial capacity, and uses the given comparer and
        /// has the given priority order.
        /// </summary>
        public PriorityQueue(int initialCapacity, IComparer<TPriority> comparer, PriorityOrder priorityOrder)
        {
            Init(initialCapacity, comparer.Compare, priorityOrder);
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the default initial capacity, and uses the given comparison and
        /// has the given priority order.
        /// </summary>
        public PriorityQueue(Comparison<TPriority> comparison, PriorityOrder priorityOrder)
            : this(DEFAULT_CAPACITY, comparison, priorityOrder)
        {
        }

        /// <summary>
        /// Initializes a new instance of the PriorityQueue class that is empty,
        /// has the given initial capacity, and uses the given comparison and
        /// has the given priority order.
        /// </summary>
        public PriorityQueue(int initialCapacity, Comparison<TPriority> comparison, PriorityOrder priorityOrder)
        {
            Init(initialCapacity, comparison, priorityOrder);
        }

        /// <summary>
        /// Priority order (highest first or lowest first)
        /// </summary>
        public enum PriorityOrder
        {
            HighFirst,
            LowFirst
        }

        /// <summary>
        /// Capacity of the priority queue
        /// </summary>
        public int Capacity
        {
            get => _items.Length;

            set => SetCapacity(value);
        }

        /// <summary>
        /// Gets number of items in the priority queue
        /// </summary>
        public int Count => _numItems;

        /// <summary>
        /// Gets a value indicating whether access to this priority queue is
        /// synchronized (thread safe).
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronize access to this
        /// priority queue.
        /// </summary>
        public object SyncRoot => _items.SyncRoot;

        /// <summary>
        /// Clear priority queue
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _numItems; ++i)
            {
                _items[i] = default(PriorityQueueItem<TValue, TPriority>);
            }

            _numItems = 0;
            TrimExcess();
        }

        /// <summary>
        /// Tests if priority queue contains the given value
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// </returns>
        public bool Contains(TValue value)
        {
            foreach (PriorityQueueItem<TValue, TPriority> x in _items)
            {
                if (x.Value.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Copy to array starting at given array index.
        /// </summary>
        public void CopyTo(PriorityQueueItem<TValue, TPriority>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "arrayIndex is less than 0.");
            }

            if (array.Rank > 1)
            {
                throw new ArgumentException("array is multidimensional.");
            }

            if (_numItems == 0)
            {
                return;
            }

            if (arrayIndex >= array.Length)
            {
                throw new ArgumentException("arrayIndex is equal to or greater than the length" + " of the array.");
            }

            if (_numItems > (array.Length - arrayIndex))
            {
                throw new ArgumentException(
                    "The number of elements in the source ICollection is" +
                    " greater than the available space from arrayIndex to" + " the end of the destination array.");
            }

            for (int i = 0; i < _numItems; i++)
            {
                array[arrayIndex + i] = _items[i];
            }
        }

        /// <summary>
        /// Dequeue
        /// </summary>
        /// <returns>
        /// item (value and priority)
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// empty queue
        /// </exception>
        public PriorityQueueItem<TValue, TPriority> Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The queue is empty");
            }

            return RemoveAt(0);
        }

        /// <summary>
        /// Enqueue <paramref name="newItem"/>
        /// </summary>
        public void Enqueue(PriorityQueueItem<TValue, TPriority> newItem)
        {
            if (_numItems == _capacity)
            {
                // need to increase capacity
                // grow by 50 percent
                SetCapacity((3 * Capacity) / 2);
            }

            int i = _numItems;
            ++_numItems;
            while ((i > 0) && (_prioritySign * _compareFunc(_items[(i - 1) / 2].Priority, newItem.Priority) < 0))
            {
                _items[i] = _items[(i - 1) / 2];
                i = (i - 1) / 2;
            }

            _items[i] = newItem;

            // if (!VerifyQueue())
            // {
            //      Debug.Log("ERROR: Queue out of order!");
            // }
        }

        /// <summary>
        /// Enqueue <paramref name="value"/> with <paramref name="priority"/>
        /// </summary>
        public void Enqueue(TValue value, TPriority priority)
        {
            Enqueue(new PriorityQueueItem<TValue, TPriority>(value, priority));
        }

        /// <summary>
        /// Get (peek but not dequeue) first item
        /// </summary>
        /// <returns>
        /// first item in the priority queue
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// empty queue
        /// </exception>
        public PriorityQueueItem<TValue, TPriority> Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The queue is empty.");
            }

            return _items[0];
        }

        /// <summary>
        /// Removes the item with the specified value from the queue.
        /// The passed equality comparison is used.
        /// </summary>
        /// <param name="item">
        /// The item to be removed.
        /// </param>
        /// <param name="comparer">
        /// An object that implements the <see cref="IEqualityComparer"/>
        /// interface for the type of item in the collection.
        /// </param>
        /// <exception cref="ApplicationException">The specified item is not in the queue.</exception>
        public void Remove(TValue item, IEqualityComparer comparer)
        {
            // need to find the PriorityQueueItem that has the Data value of item
            for (int index = 0; index < _numItems; ++index)
            {
                if (!comparer.Equals(item, _items[index].Value))
                {
                    continue;
                }

                RemoveAt(index);
                return;
            }

            throw new Exception("The specified item is not in the queue.");
        }

        /// <summary>
        /// Removes the item with the specified value from the queue.
        /// The default type comparison function is used.
        /// </summary>
        /// <param name="item">
        /// The item to be removed.
        /// </param>
        public void Remove(TValue item)
        {
            Remove(item, EqualityComparer<TValue>.Default);
        }
		
		public bool Remove(Predicate<TValue> match)
        {
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }

            bool matchedAtLeastOne = false;

            for (int index = 0; index < Count; ++index)
            {
                if (match(_items[index].Value))
                {
                    RemoveAt(index);
                    index--;
                    matchedAtLeastOne = true;
                }
            }

            return matchedAtLeastOne;
        }

        /// <summary>
        /// Set the capacity to the actual number of items, if the current
        /// number of items is less than 90 percent of the current capacity.
        /// </summary>
        public void TrimExcess()
        {
            if (_numItems < (float)0.9 * _capacity)
            {
                SetCapacity(_numItems);
            }
        }

        /// <summary>
        /// Function to check that the queue is coherent.
        /// </summary>
        public bool VerifyQueue()
        {
            int i = 0;
            while (i < _numItems / 2)
            {
                int leftChild = (2 * i) + 1;
                int rightChild = leftChild + 1;
                if (_prioritySign * _compareFunc(_items[i].Priority, _items[leftChild].Priority) < 0)
                {
                    return false;
                }

                if (rightChild < _numItems &&
                    _prioritySign * _compareFunc(_items[i].Priority, _items[rightChild].Priority) < 0)
                {
                    return false;
                }

                ++i;
            }

            return true;
        }

        /// <summary>
        /// Copy to array starting at given index.
        /// </summary>
        public void CopyTo(Array array, int index)
        {
            CopyTo((PriorityQueueItem<TValue, TPriority>[])array, index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through this priority queue.
        /// </summary>
        /// <returns>
        /// an enumerator that iterates through this priority queue.
        /// </returns>
        public IEnumerator<PriorityQueueItem<TValue, TPriority>> GetEnumerator()
        {
            for (int i = 0; i < _numItems; i++)
            {
                yield return _items[i];
            }
        }

        void Init(int initialCapacity, Comparison<TPriority> comparison, PriorityOrder priorityOrder)
        {
            _numItems = 0;
            _compareFunc = comparison;
            SetCapacity(initialCapacity);

            // multiplier to apply to result of compareFunc
            // 1 for high priority first, -1 for low priority first
            _prioritySign = (priorityOrder == PriorityOrder.HighFirst) ? 1 : -1;
        }

        PriorityQueueItem<TValue, TPriority> RemoveAt(int index)
        {
            PriorityQueueItem<TValue, TPriority> o = _items[index];
            --_numItems;

            // move the last item to fill the hole
            PriorityQueueItem<TValue, TPriority> tmp = _items[_numItems];

            // If you forget to clear this, you have a potential memory leak.
            _items[_numItems] = default(PriorityQueueItem<TValue, TPriority>);
            if (_numItems > 0 && index != _numItems)
            {
                // If the new item is greater than its parent, bubble up.
                int i = index;
                int parent = (i - 1) / 2;
                while (_prioritySign * _compareFunc(tmp.Priority, _items[parent].Priority) > 0)
                {
                    _items[i] = _items[parent];
                    i = parent;
                    parent = (i - 1) / 2;
                }

                // if i == index, then we didn't move the item up
                if (i == index)
                {
                    // bubble down ...
                    while (i < _numItems / 2)
                    {
                        int j = (2 * i) + 1;
                        if ((j < _numItems - 1) &&
                            (_prioritySign * _compareFunc(_items[j].Priority, _items[j + 1].Priority) < 0))
                        {
                            ++j;
                        }

                        if (_prioritySign * _compareFunc(_items[j].Priority, tmp.Priority) <= 0)
                        {
                            break;
                        }

                        _items[i] = _items[j];
                        i = j;
                    }
                }

                // Be sure to store the item in its place.
                _items[i] = tmp;
            }

            // if (!VerifyQueue())
            // {
            //     Debug.Log("ERROR: Queue out of order!");
            // }
            return o;
        }

        void SetCapacity(int newCapacity)
        {
            int newCap = newCapacity;
            if (newCap < DEFAULT_CAPACITY)
            {
                newCap = DEFAULT_CAPACITY;
            }

            // throw exception if newCapacity < NumItems
            if (newCap < _numItems)
            {
                throw new ArgumentOutOfRangeException("newCapacity", "New capacity is less than Count");
            }

            _capacity = newCap;
            if (_items == null)
            {
                _items = new PriorityQueueItem<TValue, TPriority>[newCap];
                return;
            }

            // Resize the array.
            Array.Resize(ref _items, newCap);
        }
    }
}