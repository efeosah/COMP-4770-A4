using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBrains.DataStructures
{
    // A priority queue that keeps a mapping of keys to indices. This facilitates changing an
    // items priority which is not a standard priority queue / heap operation.
    public class MappedPriorityQueue<TKey, TValue, TPriority> : IEnumerable
        where TPriority : IComparable<TPriority>
    {
        readonly List<PQItem> pqItems = new List<PQItem>();
        readonly Dictionary<TKey, int> map = new Dictionary<TKey, int>();
        readonly int prioritySign; // used to invert comparisons for Min or Max heaps.

        public enum PriorityOrders
        {
            HighFirst,  // Max heap
            LowFirst    // Min heap
        }

        public MappedPriorityQueue() : this(PriorityOrders.LowFirst) { }

        public MappedPriorityQueue(PriorityOrders priorityOrder)
        {
            PriorityOrder = priorityOrder;

            // multiplier to apply to result of CompareTo
            // 1 for high priority first, -1 for low priority first
            prioritySign = (priorityOrder == PriorityOrders.HighFirst) ? 1 : -1;
        }

        public PriorityOrders PriorityOrder { get; }
        public int Count => pqItems.Count;

        public bool IsEmpty => Count == 0;

        public bool ContainsKey(TKey key) { return map.ContainsKey(key); }

        public void Clear() { map.Clear(); pqItems.Clear(); }

        public PQItem PeekItem() { return Count > 0 ? pqItems[0] : default; }
        public TValue PeekValue() { return Count > 0 ? pqItems[0].Value : default; }
        public TPriority PeekPriority() { return Count > 0 ? pqItems[0].Priority : default; }

        public PQItem GetItem(TKey key) => map.ContainsKey(key) ? pqItems[map[key]] : default;
        public TValue GetValue(TKey key) => map.ContainsKey(key) ? pqItems[map[key]].Value : default;
        public TPriority GetPriority(TKey key)
            => map.ContainsKey(key) ? pqItems[map[key]].Priority : default;

       public PQItem this[TKey key] => GetItem(key);

        public void Enqueue(PQItem pqItem)
        {
            pqItems.Add(pqItem);
            map[pqItem.Key] = Count - 1;
            HeapUp();
            //Debug.Log(VerifyQueue());
        }

        public PQItem Dequeue()
        {
            var pqItem = pqItems[0];
            MoveLastItemToTheTop();
            HeapDown();
            //Debug.Log(VerifyQueue());
            return pqItem;
        }

        public void ChangeValueAndPriority(TKey key, TValue newValue, TPriority newPriority)
        {
            int i = map[key];
            PQItem pqItem = pqItems[i];
            pqItem.Value = newValue;
            TPriority oldPriority = pqItem.Priority;
            pqItem.Priority = newPriority;
            pqItems[i] = pqItem;

            Heapify(oldPriority, newPriority, i);

            //Debug.Log(VerifyQueue());
        }

        public void ChangePriority(TKey key, TPriority newPriority)
        {
            int i = map[key];
            PQItem pqItem = pqItems[i];
            TPriority oldPriority = pqItem.Priority;
            pqItem.Priority = newPriority;
            pqItems[i] = pqItem;

            Heapify(oldPriority, newPriority, i);

            //Debug.Log(VerifyQueue());
        }

        public void Remove(TKey key)
        {
            if (map.ContainsKey(key)) { RemoveAt(map[key]); }
        }

        public void RemoveAt(int index)
        {
            if (!ValidIndex(index)) return;

            PQItem pqItemToRemove = pqItems[index];
            map.Remove(pqItemToRemove.Key);

            var lastIndex = pqItems.Count - 1;
            if (lastIndex != index)
            {
                pqItems[index] = pqItems[lastIndex];
                map[pqItems[index].Key] = index;
            }

            pqItems.RemoveAt(lastIndex);

            if (lastIndex != index)
            {
                Heapify(pqItemToRemove.Priority, pqItems[index].Priority, index);
            }

            //Debug.Log(VerifyQueue());
        }

        public IEnumerator GetEnumerator() { return pqItems.GetEnumerator(); }

        void Heapify(TPriority oldPriority, TPriority newPriority, int index)
        {
            if (PriorityCompare(oldPriority, newPriority) > 0)
            {
                HeapDown(index);
            }
            else if (PriorityCompare(oldPriority, newPriority) < 0)
            {
                HeapUp(index);
            }
        }

        void HeapUp()
        {
            var itemIndex = pqItems.Count - 1;
            HeapUp(itemIndex);
        }

        void HeapUp(int itemIndex)
        {
            while (itemIndex > 0)
            {
                var parentIndex = (itemIndex - 1) / 2;
                if (PriorityCompare(pqItems[itemIndex].Priority, pqItems[parentIndex].Priority) < 0)
                    break;
                Swap(itemIndex, parentIndex);
                itemIndex = parentIndex;
            }
        }

        void MoveLastItemToTheTop()
        {
            var lastIndex = pqItems.Count - 1;
            map.Remove(pqItems[0].Key);
            pqItems[0] = pqItems[lastIndex];
            map[pqItems[0].Key] = 0;
            pqItems.RemoveAt(lastIndex);
        }

        void HeapDown()
        {
            const int itemIndex = 0;
            HeapDown(itemIndex);
        }

        void HeapDown(int itemIndex)
        {
            var lastIndex = pqItems.Count - 1;

            while (true)
            {
                var firstChildIndex = itemIndex * 2 + 1;
                if (firstChildIndex > lastIndex)
                {
                    break;
                }

                var secondChildIndex = firstChildIndex + 1;
                if (secondChildIndex <= lastIndex &&
                    PriorityCompare(
                        pqItems[secondChildIndex].Priority,
                        pqItems[firstChildIndex].Priority) > 0)
                {
                    firstChildIndex = secondChildIndex;
                }

                if (PriorityCompare(pqItems[itemIndex].Priority, pqItems[firstChildIndex].Priority) > 0)
                {
                    break;
                }

                Swap(itemIndex, firstChildIndex);
                itemIndex = firstChildIndex;
            }
        }

        void Swap(int index1, int index2)
        {
            var tmp = pqItems[index1];
            pqItems[index1] = pqItems[index2];
            pqItems[index2] = tmp;
            map[pqItems[index1].Key] = index1;
            map[pqItems[index2].Key] = index2;
        }

        int PriorityCompare(TPriority priority1, TPriority priority2)
        {
            return prioritySign * priority1.CompareTo(priority2);
        }

        int PriorityCompare(int index1, int index2)
        {
            if (!ValidIndex(index1) || !ValidIndex(index2))
            {
                throw new ArgumentException("Invalid index");
            }

            return PriorityCompare(pqItems[index1].Priority, pqItems[index2].Priority);
        }

        bool ValidIndex(int index) { return index >= 0 && index < Count; }

        public struct PQItem
        {
            internal PQItem(TKey key, TValue value, TPriority priority)
            {
                Key = key;
                Value = value;
                Priority = priority;
            }

            public TKey Key { get; }
            public TValue Value { get; set; }
            public TPriority Priority { get; set; }
        }

        // Function to check that the priority queue is coherent.
        public bool VerifyQueue()
        {
            int i = 0;
            while (i < Count / 2)
            {
                int leftChild = (2 * i) + 1;
                int rightChild = leftChild + 1;

                if (PriorityCompare(pqItems[i].Priority, pqItems[leftChild].Priority) < 0)
                {
                    return false;
                }

                if (rightChild < Count &&
                    PriorityCompare(pqItems[i].Priority, pqItems[rightChild].Priority) < 0)
                {
                    return false;
                }

                ++i;
            }

            return true;
        }
    }
}