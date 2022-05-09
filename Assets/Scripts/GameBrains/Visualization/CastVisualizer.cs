using System.ComponentModel;
using GameBrains.EventSystem;
using GameBrains.Extensions;
using GameBrains.Extensions.ScriptableObjects;
using GameBrains.GameManagement;
using UnityEngine;

namespace GameBrains.Visualization
{
    public abstract class CastVisualizer : ExtendedScriptableObject
    {
        static int nextId;
        static int NextId { get { nextId += 1; return nextId; } }

        int id;

        public float hideAfterSeconds;

        public bool destroyInsteadOfHide;
        
        public GameObject visualizerPrefab;
        public GameObject VisualizerObject => visualizerObject;
        protected GameObject visualizerObject;

        public float hideThreshold = 0.1f;

        public override void OnEnable()
        {
            base.OnEnable();
            if (visualizerObject == null) { Create(); }
            Hide(true);
            
            hideAfterSeconds = Parameters.Instance.VisualizerHideAfter;
            
            EventManager.Instance.Subscribe<HideVisualizerEventPayload>(
                Events.HideVisualizerRequest,
                OnHideVisualizer);
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Hide(true);
            
            // If the EventManger got destroyed first, no need to unsubscribe
            if (!EventManager.Instance) return;
            
            EventManager.Instance.Unsubscribe<HideVisualizerEventPayload>(
                Events.HideVisualizerRequest,
                OnHideVisualizer);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            visualizerObject.CheckAndDestroy();
            visualizerObject = null;
        }

        public abstract void Hide(bool shouldHide);

        public abstract void SetColor(Color color);

        public virtual void Draw(Vector3 startPosition, Vector3 endPosition)
        {
            var directionVector = endPosition - startPosition;
            var length = directionVector.magnitude;
            var direction = directionVector.normalized;
            Draw(startPosition, direction, length);
        }

        public virtual void Draw(Vector3 startPosition, Vector3 direction, float length)
        {
            id = NextId;
            EventManager.Instance.Enqueue(
                Events.HideVisualizerRequest,
                hideAfterSeconds,
                new HideVisualizerEventPayload(id, destroyInsteadOfHide));
        }

        protected abstract void Create();
        
        bool OnHideVisualizer(Event<HideVisualizerEventPayload> eventArguments)
        {
            HideVisualizerEventPayload payload = eventArguments.EventData;

            if (payload.id == id)
            {
                if (payload.destroyInsteadOfHide)
                {
                    Destroy(this);
                }
                else
                {
                    Hide(true);
                }
                
                return true;
            }

            return false;
        }
    }
}

// ReSharper disable once CheckNamespace
namespace GameBrains.EventSystem // NOTE: Don't change this namespace
{
    public static partial class Events
    {
        [Description("HideVisualizerRequest")]
        public static readonly EventType HideVisualizerRequest = (EventType)Count++;
    }

    public struct HideVisualizerEventPayload
    {
        public readonly int id;
        public readonly bool destroyInsteadOfHide;

        public HideVisualizerEventPayload(int id, bool destroyInsteadOfHide)
        {
            this.id = id;
            this.destroyInsteadOfHide = destroyInsteadOfHide;
        }
    }
}