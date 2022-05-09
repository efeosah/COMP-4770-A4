using UnityEngine;

namespace GameBrains.Extensions.ScriptableObjects
{
    public abstract class EnumScriptableObject : ScriptableObject
    {
        public string description;

        void OnEnable() { description ??= name; }
    }
}