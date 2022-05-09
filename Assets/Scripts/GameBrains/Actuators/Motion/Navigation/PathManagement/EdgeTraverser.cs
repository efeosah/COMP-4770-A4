using GameBrains.Actuators.Motion.Steering;
using GameBrains.Entities;
using GameBrains.EventSystem;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Motion.Steering.VelocityBased;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.PathManagement
{
	public sealed class EdgeTraverser : ExtendedMonoBehaviour
	{
		#region Members and Properties

		PathfindingAgent pathfindingAgent;
		SteeringBehaviour steering;
		QuasiEdge EdgeToFollow { get; set; }

		#endregion Members and Properties

		#region Awake

		public override void Awake()
		{
			base.Awake();
			pathfindingAgent = GetComponent<PathfindingAgent>();
		}

		#endregion Awake

		#region Enable/Disable

		public override void OnEnable()
		{
			base.OnEnable();

			if (EventManager.Instance != null)
			{
				EventManager.Instance.Subscribe<LinearStopCompletedEventPayload>(
					Events.LinearStopCompleted,
					OnLinearStopCompleted);

				EventManager.Instance.Subscribe<SeekCompletedEventPayload>(
					Events.SeekCompleted,
					OnSeekCompleted);
			}
			else
			{
				Debug.LogWarning("Event manager missing. Unable to subscribe to steering events.");
			}
		}

		public override void OnDisable()
		{
			base.OnDisable();

			if (EventManager.Instance != null)
			{
				EventManager.Instance.Unsubscribe<LinearStopCompletedEventPayload>(
					Events.LinearStopCompleted,
					OnLinearStopCompleted);

				EventManager.Instance.Unsubscribe<SeekCompletedEventPayload>(
					Events.SeekCompleted,
					OnSeekCompleted);
			}
		}

		#endregion Enable/Disable

		#region Traverse Edge
	
		public bool Traverse(QuasiEdge edgeToFollow, bool brakeOnApproach, bool stopOnArrival)
		{	
			if (pathfindingAgent == null) { return false; }

			EdgeToFollow = edgeToFollow;

			if (steering != null)
			{
				((SteerableAgent)pathfindingAgent).Data.RemoveSteeringBehaviour(steering.ID);
				Destroy(steering);
			}

			steering = brakeOnApproach 
				? Arrive.CreateInstance(((SteerableAgent)pathfindingAgent).Data)
				: Seek.CreateInstance(((SteerableAgent)pathfindingAgent).Data);
			steering.NoStop = !stopOnArrival;
			steering.NeverCompletes = false;
			
			steering.TargetLocation = EdgeToFollow.toLocation;
			((SteerableAgent)pathfindingAgent).Data.AddSteeringBehaviour(steering);

			return true;
		}

		#endregion Traverse Edge

		#region Steering Events

		bool OnLinearStopCompleted(Event<LinearStopCompletedEventPayload> eventArguments)
		{
			LinearStopCompletedEventPayload payload = eventArguments.EventData;

			if (steering == null ||
			    payload.steerableAgent != pathfindingAgent ||
			    payload.linearStop.ID != steering.ID) // event not for us
			{
				return false;
			}
			
			if (steering != null)
			{
				((SteerableAgent)pathfindingAgent).Data.RemoveSteeringBehaviour(steering.ID);
				Destroy(steering);
			}

			return true;
		}

		bool OnSeekCompleted(Event<SeekCompletedEventPayload> eventArguments)
		{
			SeekCompletedEventPayload payload = eventArguments.EventData;
	
			if (steering == null ||
			    payload.steerableAgent != pathfindingAgent ||
			    payload.seek.ID != steering.ID) // event not for us
			{
				return false;
			}

			if (payload.seek.TargetLocation == EdgeToFollow.toLocation)
			{
				if (steering != null && steering.NoStop)
				{
					((SteerableAgent)pathfindingAgent).Data.RemoveSteeringBehaviour(steering.ID);
					Destroy(steering);
				}
				
				EventManager.Instance.Enqueue(
					Events.TraversalCompleted,
					new TraversalCompletedEventPayload(pathfindingAgent, EdgeToFollow));
			}

			return true;
		}

		#endregion Steering Events
	}
}