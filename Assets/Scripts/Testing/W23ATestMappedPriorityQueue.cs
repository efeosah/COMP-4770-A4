using GameBrains.DataStructures;
using GameBrains.Extensions.MonoBehaviours;
using UnityEngine;

namespace Testing
{
    using PQItem = MappedPriorityQueue<string,string,int>.PQItem;
    public sealed class W23ATestMappedPriorityQueue : ExtendedMonoBehaviour
    {
        #region Members and Properties
        
        [SerializeField] bool testEnqueue;
        
        #endregion Members and Properties

        #region Update

        public override void Update()
        {
            base.Update();
            
            if (testEnqueue)
            {
                testEnqueue = false;

                MappedPriorityQueue<string, string, int> mpq =
                    new MappedPriorityQueue<string, string, int>();

                mpq.Enqueue(new PQItem("a", "w", 0));
                mpq.Enqueue(new PQItem("b", "x", 5));
                mpq.Enqueue(new PQItem("c", "y", 2));
                mpq.Enqueue(new PQItem("d", "7", 7));

                var pqItem = mpq.Dequeue();
                Debug.Log($"Dequeue: <{pqItem.Key}, {pqItem.Value}, {pqItem.Priority}> which should be <a,w,0>.");

                mpq.ChangeValueAndPriority("b", "v", 1);

                pqItem = mpq.Dequeue();
                
                Debug.Log($"Dequeue: <{pqItem.Key}, {pqItem.Value}, {pqItem.Priority}> which should be <b,v,1>.");

                mpq.RemoveAt(0);

                pqItem = mpq.PeekItem();
                Debug.Log($"Peek: <{pqItem.Key}, {pqItem.Value}, {pqItem.Priority}> which should be <d,7,7>.");
                
                pqItem = mpq.Dequeue();
                Debug.Log($"Dequeue: <{pqItem.Key}, {pqItem.Value}, {pqItem.Priority}> which should be <d,7,7>.");
            }
        }

        #endregion Update
    }
}