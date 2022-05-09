using GameBrains.Entities.EntityData;
using GameBrains.EventSystem;
using GameBrains.Extensions.Vectors;
using UnityEngine;

namespace GameBrains.Entities
{
    public class SteerableAgent : MovingAgent
    {
        #region Motion

        public new SteeringData Data => data as SteeringData;

        #endregion Motion
        
        #region Awake

        public override void Awake()
        {
            base.Awake();

            data = (SteeringData) transform;
        }
        
        #endregion Awake
        
        #region OnEnable, OnDisable, OnDestroy

        // Don't Enable in Editor mode

        public override void OnEnable()
        {
            if (!Application.IsPlaying(this)) { return; }
            
            base.OnEnable();

            SubscribeToSteeringEvents();
        }

        public override void OnDisable()
        {
            if (!Application.IsPlaying(this)) { return; }
            
            base.OnDisable();

            UnsubscribeFromSteeringEvents();
        }
        
        #endregion OnEnable, OnDisable, OnDestroy

        #region Sense, think, and act
        
        protected override void Act(float deltaTime)
        {
            if (!IsPlayerControlled)
            {
                Data.CalculateSteering();
            }

            base.Act(deltaTime);
        }
        
        #endregion Sense, think, and act

        #region Subscribe and Unsubscribe

        void SubscribeToSteeringEvents()
        {
            if (EventManager.Instance != null)
            {
                // LinearStopCompleted
                EventManager.Instance.Subscribe<LinearStopCompletedEventPayload>(
                    Events.LinearStopCompleted,
                    OnLinearStopCompleted);

                // LinearSlowCompleted
                EventManager.Instance.Subscribe<LinearSlowCompletedEventPayload>(
                    Events.LinearSlowCompleted,
                    OnLinearSlowCompleted);

                // SeekCompleted
                EventManager.Instance.Subscribe<SeekCompletedEventPayload>(
                    Events.SeekCompleted,
                    OnSeekCompleted);

                // ArriveCompleted
                EventManager.Instance.Subscribe<ArriveCompletedEventPayload>(
                    Events.ArriveCompleted,
                    OnArriveCompleted);

                // ArriveBrakingStarted
                EventManager.Instance.Subscribe<ArriveBrakingStartedEventPayload>(
                    Events.ArriveBrakingStarted,
                    OnArriveBrakingStarted);

                // FleeCompleted
                EventManager.Instance.Subscribe<FleeCompletedEventPayload>(
                    Events.FleeCompleted,
                    OnFleeCompleted);

                // DepartCompleted
                EventManager.Instance.Subscribe<DepartCompletedEventPayload>(
                    Events.DepartCompleted,
                    OnDepartCompleted);

                // PursueCompleted
                EventManager.Instance.Subscribe<PursueCompletedEventPayload>(
                    Events.PursueCompleted,
                    OnPursueCompleted);

                // EvadeCompleted
                EventManager.Instance.Subscribe<EvadeCompletedEventPayload>(
                    Events.EvadeCompleted,
                    OnEvadeCompleted);

                // AngularStopCompleted
                EventManager.Instance.Subscribe<AngularStopCompletedEventPayload>(
                    Events.AngularStopCompleted,
                    OnAngularStopCompleted);

                // AngularSlowCompleted
                EventManager.Instance.Subscribe<AngularSlowCompletedEventPayload>(
                    Events.AngularSlowCompleted,
                    OnAngularSlowCompleted);

                // AlignCompleted
                EventManager.Instance.Subscribe<AlignCompletedEventPayload>(
                    Events.AlignCompleted,
                    OnAlignCompleted);

                // AngularArriveCompleted
                EventManager.Instance.Subscribe<AngularArriveCompletedEventPayload>(
                    Events.AngularArriveCompleted,
                    OnAngularArriveCompleted);

                // AngularArriveBrakingStarted
                EventManager.Instance.Subscribe<AngularArriveBrakingStartedEventPayload>(
                    Events.AngularArriveBrakingStarted,
                    OnAngularArriveBrakingStarted);

                // WanderCompleted
                EventManager.Instance.Subscribe<WanderCompletedEventPayload>(
                    Events.WanderCompleted,
                    OnWanderCompleted);
            }
            else
            {
                Debug.LogWarning("Event manager missing. Unable to subscribe to steering events.");
            }
        }

        void UnsubscribeFromSteeringEvents()
        {
            // If the EventManger got destroyed first, no need to unsubscribe
            if (EventManager.Instance != null)
            {
                // StopCompleted
                EventManager.Instance.Unsubscribe<LinearStopCompletedEventPayload>(
                    Events.LinearStopCompleted,
                    OnLinearStopCompleted);

                // SlowCompleted
                EventManager.Instance.Unsubscribe<LinearSlowCompletedEventPayload>(
                    Events.LinearSlowCompleted,
                    OnLinearSlowCompleted);

                // SeekCompleted
                EventManager.Instance.Unsubscribe<SeekCompletedEventPayload>(
                    Events.SeekCompleted,
                    OnSeekCompleted);

                // ArriveCompleted
                EventManager.Instance.Unsubscribe<ArriveCompletedEventPayload>(
                    Events.ArriveCompleted,
                    OnArriveCompleted);

                // ArriveBrakingStarted
                EventManager.Instance.Unsubscribe<ArriveBrakingStartedEventPayload>(
                    Events.ArriveBrakingStarted,
                    OnArriveBrakingStarted);

                // FleeCompleted
                EventManager.Instance.Unsubscribe<FleeCompletedEventPayload>(
                    Events.FleeCompleted,
                    OnFleeCompleted);

                // DepartCompleted
                EventManager.Instance.Unsubscribe<DepartCompletedEventPayload>(
                    Events.DepartCompleted,
                    OnDepartCompleted);

                // PursueCompleted
                EventManager.Instance.Unsubscribe<PursueCompletedEventPayload>(
                    Events.PursueCompleted,
                    OnPursueCompleted);

                // EvadeCompleted
                EventManager.Instance.Unsubscribe<EvadeCompletedEventPayload>(
                    Events.EvadeCompleted,
                    OnEvadeCompleted);

                // AngularStopCompleted
                EventManager.Instance.Unsubscribe<AngularStopCompletedEventPayload>(
                    Events.AngularStopCompleted,
                    OnAngularStopCompleted);

                // AngularSlowCompleted
                EventManager.Instance.Unsubscribe<AngularSlowCompletedEventPayload>(
                    Events.AngularSlowCompleted,
                    OnAngularSlowCompleted);

                // AlignCompleted
                EventManager.Instance.Unsubscribe<AlignCompletedEventPayload>(
                    Events.AlignCompleted,
                    OnAlignCompleted);

                // ArriveOrientationCompleted
                EventManager.Instance.Unsubscribe<AngularArriveCompletedEventPayload>(
                    Events.AngularArriveCompleted,
                    OnAngularArriveCompleted);

                // AngularArriveBrakingStarted
                EventManager.Instance.Unsubscribe<AngularArriveBrakingStartedEventPayload>(
                    Events.AngularArriveBrakingStarted,
                    OnAngularArriveBrakingStarted);

                // WanderCompleted
                EventManager.Instance.Unsubscribe<WanderCompletedEventPayload>(
                    Events.WanderCompleted,
                    OnWanderCompleted);
            }
        }

        #endregion Subscribe and Unsubscribe

        #region Spawn

        // Relocate and reactive moving entity. Reset Kinematic Data.
        public override void Spawn(VectorXYZ spawnPoint)
        {
            base.Spawn(spawnPoint);

            Data.ResetSteeringData();
        }

        #endregion Spawn

        #region Events

        #region Steering Events

        bool OnLinearStopCompleted(Event<LinearStopCompletedEventPayload> eventArguments)
        {
            LinearStopCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnLinearSlowCompleted(Event<LinearSlowCompletedEventPayload> eventArguments)
        {
            LinearSlowCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnSeekCompleted(Event<SeekCompletedEventPayload> eventArguments)
        {
            SeekCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnArriveCompleted(Event<ArriveCompletedEventPayload> eventArguments)
        {
            ArriveCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnArriveBrakingStarted(Event<ArriveBrakingStartedEventPayload> eventArguments)
        {
            ArriveBrakingStartedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnFleeCompleted(Event<FleeCompletedEventPayload> eventArguments)
        {
            FleeCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnDepartCompleted(Event<DepartCompletedEventPayload> eventArguments)
        {
            DepartCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnPursueCompleted(Event<PursueCompletedEventPayload> eventArguments)
        {
            PursueCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnEvadeCompleted(Event<EvadeCompletedEventPayload> eventArguments)
        {
            EvadeCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnAngularStopCompleted(Event<AngularStopCompletedEventPayload> eventArguments)
        {
            AngularStopCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnAngularSlowCompleted(Event<AngularSlowCompletedEventPayload> eventArguments)
        {
            AngularSlowCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnAlignCompleted(Event<AlignCompletedEventPayload> eventArguments)
        {
            AlignCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnAngularArriveCompleted(Event<AngularArriveCompletedEventPayload> eventArguments)
        {
            AngularArriveCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnAngularArriveBrakingStarted(Event<AngularArriveBrakingStartedEventPayload> eventArguments)
        {
            AngularArriveBrakingStartedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        bool OnWanderCompleted(Event<WanderCompletedEventPayload> eventArguments)
        {
            WanderCompletedEventPayload payload = eventArguments.EventData;

            if (payload.steerableAgent != this) // event not for us
            {
                return false;
            }

            // TODO: Do stuff

            return true;
        }

        #endregion Steering Events

        #endregion Events
    }
}