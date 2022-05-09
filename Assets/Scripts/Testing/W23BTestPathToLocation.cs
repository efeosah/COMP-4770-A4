using GameBrains.Entities;
using GameBrains.Entities.Types;
using GameBrains.EventSystem;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEngine;

namespace Testing
{
    public sealed class W23BTestPathToLocation : ExtendedMonoBehaviour
    {
        #region Members and Properties
        
        [SerializeField] bool testPathToLocation;
        [SerializeField] PathfindingAgent pathfindingAgent;
        [SerializeField] VectorXZ destination;

        [SerializeField] bool testPathToEntityWithType;
        [SerializeField] EntityType entityType;
        
        #endregion Members and Properties

        #region Enable/Disable

        public override void OnEnable()
        {
            base.OnEnable();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Subscribe<PathToLocationReadyEventPayload>(
                    Events.PathToLocationReady,
                    OnPathToLocationReady);

                EventManager.Instance.Subscribe<NoPathToLocationAvailableEventPayload>(
                    Events.NoPathToLocationAvailable,
                    OnNoPathToLocationAvailable);

                EventManager.Instance.Subscribe<PathToEntityWithTypeReadyEventPayload>(
                    Events.PathToEntityWithTypeReady,
                    OnPathToEntityWithTypeReady);

                EventManager.Instance.Subscribe<NoPathToEntityWithTypeAvailableEventPayload>(
                    Events.NoPathToEntityWithTypeAvailable,
                    OnNoPathToEntityWithTypeAvailable);
            }
            else
            {
                Debug.LogWarning("Event manager missing. Unable to subscribe to pathfinding events.");
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();

            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<PathToLocationReadyEventPayload>(
                    Events.PathToLocationReady,
                    OnPathToLocationReady);

                EventManager.Instance.Unsubscribe<NoPathToLocationAvailableEventPayload>(
                    Events.NoPathToLocationAvailable,
                    OnNoPathToLocationAvailable);

                EventManager.Instance.Unsubscribe<PathToEntityWithTypeReadyEventPayload>(
                    Events.PathToEntityWithTypeReady,
                    OnPathToEntityWithTypeReady);

                EventManager.Instance.Unsubscribe<NoPathToEntityWithTypeAvailableEventPayload>(
                    Events.NoPathToEntityWithTypeAvailable,
                    OnNoPathToEntityWithTypeAvailable);
            }
        }
        
        #endregion Enable/Disable

        #region Awake

        public override void Awake()
        {
            base.Awake();
            
            if (entityType == null)
            {
                entityType = Resources.Load<EntityType>($"{Parameters.Instance.EntityTypeFrameworkPath}TestType");
            }
        }
        
        #endregion Awake

        #region Update

        public override void Update()
        {
            if (testPathToLocation)
            {
                testPathToLocation = false;

                EventManager.Instance.Enqueue(
                    Events.PathToLocationRequest,
                    new PathToLocationRequestEventPayload(pathfindingAgent, destination));
            }

            if (testPathToEntityWithType)
            {
                testPathToEntityWithType = false;

                EventManager.Instance.Enqueue(
                    Events.PathToEntityWithTypeRequest,
                    new PathToEntityWithTypeRequestEventPayload(pathfindingAgent, entityType));
            }
        }

        #endregion Update

        #region Events

        bool OnPathToLocationReady(Event<PathToLocationReadyEventPayload> eventArguments)
        {
            PathToLocationReadyEventPayload payload = eventArguments.EventData;

            if (payload.pathfindingAgent != pathfindingAgent) // event not for us
            {
                Debug.Log($"Path ready for someone else: {payload.pathfindingAgent}");
                return false;
            }

            Debug.Log($"Path ready for us: {payload.path}");

            return true;
        }

        bool OnNoPathToLocationAvailable(Event<NoPathToLocationAvailableEventPayload> eventArguments)
        {
            NoPathToLocationAvailableEventPayload payload = eventArguments.EventData;

            if (payload.pathfindingAgent != pathfindingAgent) // event not for us
            {
                Debug.Log($"Path not available for someone else: {payload.pathfindingAgent}");
                return false;
            }

            Debug.Log("Path not available for us");
            return true;
        }

        bool OnPathToEntityWithTypeReady(Event<PathToEntityWithTypeReadyEventPayload> eventArguments)
        {
            PathToEntityWithTypeReadyEventPayload payload = eventArguments.EventData;

            if (payload.pathfindingAgent != pathfindingAgent) // event not for us
            {
                Debug.Log($"Path ready for someone else: {payload.pathfindingAgent}");
                return false;
            }

            Debug.Log($"Path ready for us: {payload.path}");
            return true;
        }

        bool OnNoPathToEntityWithTypeAvailable(Event<NoPathToEntityWithTypeAvailableEventPayload> eventArguments)
        {
            NoPathToEntityWithTypeAvailableEventPayload payload = eventArguments.EventData;

            if (payload.pathfindingAgent != pathfindingAgent) // event not for us
            {
                Debug.Log($"Path not available for someone else: {payload.pathfindingAgent}");
                return false;
            }

            Debug.Log("Path not available for us");
            return true;
        }

        #endregion Events
    }
}