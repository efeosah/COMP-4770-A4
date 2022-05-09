using System.Collections.Generic;
using UnityEngine;

namespace GameBrains.Extensions.DictionaryExtensions
{
    public abstract class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        public List<TKey> keysList = new List<TKey>();
        public List<TValue> valuesList = new List<TValue>();

        public void OnBeforeSerialize()
        {
            keysList.Clear();
            valuesList.Clear();
            foreach (KeyValuePair<TKey,TValue> keyValuePair in this)
            {
                keysList.Add(keyValuePair.Key);
                valuesList.Add(keyValuePair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < keysList.Count; i++)
            {
                this[keysList[i]] = valuesList[i];
            }
        }
    }
}