using System.Collections.Generic;
using System.ComponentModel;
using GameBrains.Entities;
using GameBrains.Entities.EntityData;
using GameBrains.Entities.Types;
using GameBrains.EventSystem;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameBrains.Entities
{
    [ExecuteAlways] // Needed so Entity Data appears in the Inspector
    public class Entity : ExtendedMonoBehaviour
    {
        public bool defaultCameraTarget;
        
        public int ID { get; protected set; }
        
        IEventHandlingComponent[] eventHandlers;
        
        #region Short Name and Color

        [SerializeField] string shortName;

        public string ShortName
        {
            get => shortName;
            set
            {
                shortName = value;
                SetNameAndColor();
            }
        }

        [SerializeField] Color color;
        
        public Color Color
        {
            get => color;
            set
            {
                color = value;
                SetNameAndColor();
            }
        }
        
        // TODO: Maybe create name change and color change events to decouple from specific objects.
        void SetNameAndColor()
        {
            var backgroundTransform = transform.Find("WeebleCanvas/NameBackground");
            if (backgroundTransform)
            {
                var backgroundImage = backgroundTransform.GetComponent<Image>();
                if (backgroundImage)
                {
                    backgroundImage.color = Invert(color);
                }
            }
            
            var nameTransform = transform.Find("WeebleCanvas/Name");
            if (nameTransform)
            {
                var nameTextMesh = nameTransform.GetComponent<TextMeshProUGUI>();
                if (nameTextMesh)
                {
                    nameTextMesh.text = shortName;
                    nameTextMesh.color = color;
                }
            }
            
        }
        
        Color Invert(Color rgbColor) 
        {
            float h, s, v;
            Color.RGBToHSV(rgbColor, out h, out s, out v);
            return Color.HSVToRGB((h + 0.5f) % 1, s, v);
        }

        #endregion Short Name and Color
        
        #region Static Data

        [SerializeField] protected StaticData data;

        public StaticData Data
        {
            get => data;
            set => data = value;
        }

        #endregion Static Data
        
        #region Physics

        public CapsuleCollider CapsuleCollider { get; set; }
		
        public CharacterController CharacterController { get; set; }
		
        public Rigidbody Rigidbody { get; set; }

        #endregion Physics
        
        #region Statuses

        public enum Statuses
        {
            Active,
            Inactive,
            Spawning
        }

        public Statuses Status { get; set; }

        public bool IsInactive => Status == Statuses.Inactive;

        public bool IsActive => Status == Statuses.Active;

        public bool IsSpawning => Status == Statuses.Spawning;

        public void SetSpawning()
        {
            Status = Statuses.Spawning;
        }

        public void SetInactive()
        {
            Status = Statuses.Inactive;
        }

        public void SetActive()
        {
            Status = Statuses.Active;
        }

        #endregion Statuses
        
        #region Entity Type

        public List<EntityType> entityTypes = new List<EntityType>();

        #endregion Entity Type
        
        #region Health

        [SerializeField] int maxHealth = 100;
        int currentHealth;

        public event System.Action<float> OnHealthPercentChanged = delegate { };
		
        [Range(0,100)]
        [SerializeField] int health;
		
        public int Health
        {
            get => currentHealth;
            set => CurrentHealth = value;
        }

        public int CurrentHealth
        {
            get => currentHealth;
            set 
            { 
                currentHealth = value; 
                var currentHealthPercent = currentHealth / (float)maxHealth;
                OnHealthPercentChanged(currentHealthPercent); 
				
                // TODO: Die when 0
            }
        }
		
        public void ModifyHealth(int amount)
        {
            CurrentHealth += amount;
        }
		
        #endregion Health

        #region Awake
        
        public override void Awake()
        {
            base.Awake();

            ID = EntityManager.NextID;
            EntityManager.Add(this);
            
            eventHandlers = GetComponents<IEventHandlingComponent>();
			
            Data = transform;
            
            CapsuleCollider = GetComponent<CapsuleCollider>();
            CharacterController = GetComponent<CharacterController>();
            Rigidbody = GetComponent<Rigidbody>();
            
            Status = Statuses.Active;
            
            SetNameAndColor();
        }
        
        #endregion Awake

        #region Controller Collision
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //     if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) { return; }
            //
            //     Debug.Log("Steerable Agent collided with: " + hit.collider.name);

            EventManager.Instance.Fire(
                Events.ControllerCollision,
                new ControllerCollisionEventPayload(this, hit));
        }

        #endregion Controller Collision
        
        #region Event Handling

        // Typical usages:
        // EventManager.Instance.Enqueue(Events.MyEntityEvent, receiver.ID, data);
        // EventManager.Instance.Fire(Events.MyEntityEvent, receiver.ID, data);
        // EventManager.Instance.Fire(Events.Message, lookedUpReceiver.ID, message);
        // EventManager.Instance.Enqueue(Events.Message, delay, receiver.ID, message);
        // EventManager.Instance.Subscribe<type>(Events.MyDelegateEvent, HandleEvent);

        public virtual bool HandleEvent<T>(Event<T> eventArguments)
        {
            bool handled = false;

            if (eventArguments.ReceiverId != ID
                || eventArguments.ReceiverId != EventManager.ReceiverIDIrrelevant)
            {
                return false;
            }
			
            foreach (IEventHandlingComponent eventHandler in eventHandlers)
            {
                handled |= eventHandler.HandleEvent(eventArguments);
            }

            if (handled && VerbosityDebug)
            {
                Debug.Log($"Entity {name} handled {eventArguments}");
            }

            return handled;
        }

        #endregion Event Handling
        
        #region Spawn

        // Relocate and reactivate entity
        public virtual void Spawn(VectorXYZ spawnPoint)
        {
            Status = Statuses.Active;
            Data.Position = spawnPoint;
            Data.Rotation = Quaternion.identity;
            Data.ResetStaticData();
            Physics.SyncTransforms(); // Apply Transform changes to the physics engine.
        }

        #endregion
    }
}

#region Events

// ReSharper disable once CheckNamespace
namespace GameBrains.EventSystem // NOTE: Don't change this namespace
{
    public static partial class Events
    {
        [Description("Controller Collision")]
        public static readonly EventType ControllerCollision = (EventType)Count++;
    }

    public struct ControllerCollisionEventPayload
    {
        public Entity entity;
        public ControllerColliderHit hit;

        public ControllerCollisionEventPayload(
            Entity entity,
            ControllerColliderHit hit)
        {
            this.entity = entity;
            this.hit = hit;
        }
    }
}

#endregion Events