using GameBrains.Entities.EntityData;
using GameBrains.EventSystem;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Motors
{
    public sealed class CharacterControllerMotor : Motor
    {
        #region OnEnable, OnDisable, OnDestroy

        public override void OnEnable()
        {
            base.OnEnable();

            SubscribeToControllerEvents();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            UnsubscribeFromControllerEvents();
        }

        #endregion OnEnable, OnDisable, OnDestroy

        #region Subscribe and Unsubscribe

        void SubscribeToControllerEvents()
        {
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Subscribe<ControllerCollisionEventPayload>(
                    Events.ControllerCollision,
                    OnControllerCollision);
            }
            else
            {
                Debug.LogWarning("Event manager missing. Unable to subscribe to controller events.");
            }
        }

        void UnsubscribeFromControllerEvents()
        {
            // If the EventManger got destroyed first, no need to unsubscribe
            if (EventManager.Instance != null)
            {
                EventManager.Instance.Unsubscribe<ControllerCollisionEventPayload>(
                    Events.ControllerCollision,
                    OnControllerCollision);
            }
        }

        #endregion Subscribe and Unsubscribe

        #region Start

        public override void Start()
        {
            base.Start();
            SetupCharacterController();
        }

        #endregion Start

        #region Calculate Physics

        public override void CalculatePhysics(KinematicData kinematicData, float deltaTime)
        {
            kinematicData.DoUpdate(deltaTime, false);
            if (Agent.CharacterController.enabled)
            {
                Agent.CharacterController.SimpleMove((Vector3)kinematicData.Velocity);
            }
        }

        #endregion Calculate Physics

        #region Events

        public bool OnControllerCollision(Event<ControllerCollisionEventPayload> eventArguments)
        {
            ControllerCollisionEventPayload payload = eventArguments.EventData;

            if (payload.entity != Agent) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        #endregion Events
    }
}