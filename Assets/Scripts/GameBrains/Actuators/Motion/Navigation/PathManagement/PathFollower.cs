using GameBrains.Entities;
using GameBrains.EventSystem;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.PathManagement
{
	public sealed class PathFollower : ExtendedMonoBehaviour
	{
		#region Members and Properties

		PathfindingAgent pathfindingAgent;
		EdgeTraverser edgeTraverser;

		public Path PathToFollow
		{
			get => pathToFollow;
			private set => pathToFollow = value;
		}
		Path pathToFollow;

		public QuasiEdge EdgeToFollow
		{
			get => edgeToFollow;
			private set => edgeToFollow = value;
		}
		QuasiEdge edgeToFollow;

		public bool IsFollowing { get; private set; }
		
		bool BrakeOnFinalApproach
		{
			get => brakeOnFinalApproach;
			set => brakeOnFinalApproach = value;
		}
		[SerializeField] bool brakeOnFinalApproach;
		
		bool StopOnFinalArrival
		{
			get => stopOnFinalArrival;
			set => stopOnFinalArrival = value;
		}
		[SerializeField] bool stopOnFinalArrival;
		
		bool BrakeOnEachApproach
		{
			get => brakeOnEachApproach;
			set => brakeOnEachApproach = value;
		}
		[SerializeField] bool brakeOnEachApproach;
		
		bool StopOnEachArrival
		{
			get => stopOnEachArrival;
			set => stopOnEachArrival = value;
		}
		[SerializeField] bool stopOnEachArrival;

		#endregion Members and Property

		#region Awake
		public override void Awake()
		{
			base.Awake();

			pathfindingAgent = GetComponent<PathfindingAgent>();
			edgeTraverser = GetComponent<EdgeTraverser>();
		}

		#endregion Awake

		#region Enable/Disable

		public override void OnEnable()
		{
			base.OnEnable();

			if (EventManager.Instance != null)
			{
				EventManager.Instance.Subscribe<PathToLocationReadyEventPayload>(
					Events.PathToLocationReady,
					OnPathToLocationReady);
				
				EventManager.Instance.Subscribe<PathToEntityWithTypeReadyEventPayload>(
					Events.PathToEntityWithTypeReady,
					OnPathToEntityWithTypeReady);
				
				EventManager.Instance.Subscribe<TraversalCompletedEventPayload>(
					Events.TraversalCompleted,
					OnTraversalCompleted);

				EventManager.Instance.Subscribe<TraversalFailedEventPayload>(
					Events.TraversalFailed,
					OnTraversalFailed);
				
				EventManager.Instance.Subscribe<FollowCompletedEventPayload>(
					Events.FollowCompleted,
					OnFollowCompleted);

				EventManager.Instance.Subscribe<FollowFailedEventPayload>(
					Events.FollowFailed,
					OnFollowFailed);
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
				
				EventManager.Instance.Unsubscribe<PathToEntityWithTypeReadyEventPayload>(
					Events.PathToEntityWithTypeReady,
					OnPathToEntityWithTypeReady);
				
				EventManager.Instance.Unsubscribe<TraversalCompletedEventPayload>(
					Events.TraversalCompleted,
					OnTraversalCompleted);

				EventManager.Instance.Unsubscribe<TraversalFailedEventPayload>(
					Events.TraversalFailed,
					OnTraversalFailed);
				
				EventManager.Instance.Unsubscribe<FollowCompletedEventPayload>(
					Events.FollowCompleted,
					OnFollowCompleted);

				EventManager.Instance.Unsubscribe<FollowFailedEventPayload>(
					Events.FollowFailed,
					OnFollowFailed);
			}
		}

		#endregion Enable/Disable

		#region Follow Path

		public bool Follow(Path nextPathToFollow)
		{
			if (pathfindingAgent == null)
			{
				pathfindingAgent = GetComponent<PathfindingAgent>();
			}

			if (pathfindingAgent != null)
			{
				if (edgeTraverser == null)
				{
					edgeTraverser = GetComponent<EdgeTraverser>();
				}

				if (edgeTraverser != null)
				{
					StopIfFollowingPath();
					PathToFollow = nextPathToFollow;

					if (PathToFollow != null)
					{
						PathToFollow.Show(pathfindingAgent.Data.ShowPath);
						IsFollowing = true;
						TraverseNextEdge();
						return true;
					}

					return false;
				}

				return false;
			}

			return false;
		}

		void StopIfFollowingPath()
		{
			if (IsFollowing)
			{
				IsFollowing = false;
				CancelPath(ref pathToFollow, ref edgeToFollow);
				// TODO: Perhaps this should be a FollowCancelled event
				EventManager.Instance.Enqueue(
					Events.FollowCompleted,
					new FollowCompletedEventPayload(pathfindingAgent, PathToFollow));
			}
		}

		void TraverseNextEdge()
		{
			CancelEdge(ref edgeToFollow);
			EdgeToFollow = PathToFollow.Dequeue();

			if (PathToFollow.IsEmpty) // last edge
			{
				edgeTraverser.Traverse(
					EdgeToFollow,
					BrakeOnFinalApproach,
					StopOnFinalArrival);
			}
			else
			{
				edgeTraverser.Traverse(
					EdgeToFollow,
					BrakeOnEachApproach,
					StopOnEachArrival);
			}
		}

		#endregion Follow Path

		#region Smooth Path

		void SmoothPath(Path path)
		{
			if (pathfindingAgent.Data.CanMoveTo((VectorXYZ)path.Destination))
			{
				CancelPath(pathToFollow, edgeToFollow);
				edgeToFollow = null;
				path.CreateQuasiEdge(false, pathfindingAgent.Data.Location, path.Destination);
				path.QuasiEdges[0].Show(pathfindingAgent.Data.ShowPath);
				return;
			}

			for (int index = path.QuasiEdges.Count - 1; index >= 0; index--)
			{
				var toLocation = path.QuasiEdges[index].toLocation;
				if (pathfindingAgent.Data.CanMoveTo((VectorXYZ)toLocation))
				{
					for (int i = 0; i <= index; i++)
					{
						path.QuasiEdges[i] = CancelEdge(path.QuasiEdges[i]);
					}

					path.QuasiEdges.RemoveRange(1, index);
					path.ReplaceQuasiEdge(0,false, pathfindingAgent.Data.Location, toLocation);
					path.QuasiEdges[0].Show(pathfindingAgent.Data.ShowPath);
					break;
				}
			}
		}

		#endregion Smooth Path

		#region Cancel Paths and Edges

		public void CancelPath(ref Path currentPath, ref QuasiEdge currentQuasiEdge)
		{
			if (currentPath != null)
			{
				currentPath.Show(false);
				currentPath.CleanUp();
				currentPath = null;
			}

			CancelEdge(ref currentQuasiEdge);
		}

		// Use when you can't pass by ref
		public (Path path, QuasiEdge quasiEdge) CancelPath(Path currentPath, QuasiEdge currentQuasiEdge)
		{
			CancelPath(ref currentPath, ref currentQuasiEdge);
			return (currentPath, currentQuasiEdge);
		}

		public void CancelEdge(ref QuasiEdge currentQuasiEdge)
		{
			if (currentQuasiEdge != null)
			{
				currentQuasiEdge.Show(false);
				currentQuasiEdge.CleanUp();
				currentQuasiEdge = null;
			}
		}

		// Use when you can't pass by ref
		public QuasiEdge CancelEdge(QuasiEdge currentQuasiEdge)
		{
			CancelEdge(ref currentQuasiEdge);
			return currentQuasiEdge;
		}

		#endregion Cancel Paths and Edges

		#region Pathfinding Events
		
		bool OnPathToLocationReady(Event<PathToLocationReadyEventPayload> eventArguments)
		{	
			PathToLocationReadyEventPayload payload = eventArguments.EventData;
			
			if (payload.pathfindingAgent != pathfindingAgent) // event not for us
			{
				return false;
			}

			if (IsFollowing)
			{
				IsFollowing = false;
				CancelPath(ref pathToFollow, ref edgeToFollow);
			}

			if (pathfindingAgent.Data.SmoothPath) { SmoothPath(payload.path); }
			Follow(payload.path);
			return true;
		}
		
		bool OnPathToEntityWithTypeReady(Event<PathToEntityWithTypeReadyEventPayload> eventArguments)
		{	
			PathToEntityWithTypeReadyEventPayload payload = eventArguments.EventData;
			
			if (payload.pathfindingAgent != pathfindingAgent) // event not for us
			{
				return false;
			}
			
			if (IsFollowing)
			{
				IsFollowing = false;
				CancelPath(ref pathToFollow, ref edgeToFollow);
			}

			if (pathfindingAgent.Data.SmoothPath) { SmoothPath(payload.path); }
			Follow(payload.path);
			return true;
		}

		bool OnTraversalCompleted(Event<TraversalCompletedEventPayload> eventArguments)
		{
			TraversalCompletedEventPayload payload = eventArguments.EventData;
		
			if (payload.pathfindingAgent != pathfindingAgent) // event not for us
			{
				return false;
			}
			
			CancelEdge(ref edgeToFollow);

			if (PathToFollow.IsEmpty)
			{
				IsFollowing = false;
				EventManager.Instance.Enqueue(
					Events.FollowCompleted,
					new FollowCompletedEventPayload(pathfindingAgent, PathToFollow));
				return true;
			}
			
			if (pathfindingAgent.Data.SmoothPath) { SmoothPath(PathToFollow); }
			TraverseNextEdge();
			return true;
		}

		bool OnTraversalFailed(Event<TraversalFailedEventPayload> eventArguments)
		{
			TraversalFailedEventPayload payload = eventArguments.EventData;
		
			if (payload.pathfindingAgent != pathfindingAgent) // event not for us
			{
				return false;
			}

			CancelEdge(ref edgeToFollow);
			IsFollowing = false;
			EventManager.Instance.Enqueue(
				Events.FollowFailed,
				new FollowFailedEventPayload(pathfindingAgent, PathToFollow));

			return true;
		}

		bool OnFollowCompleted(Event<FollowCompletedEventPayload> eventArguments)
		{
			FollowCompletedEventPayload payload = eventArguments.EventData;

			if (payload.pathfindingAgent != pathfindingAgent) // event not for us
			{
				return false;
			}
			
			IsFollowing = false;
			CancelPath(ref pathToFollow, ref edgeToFollow);

			return true;
		}
		
		bool OnFollowFailed(Event<FollowFailedEventPayload> eventArguments)
		{
			FollowFailedEventPayload payload = eventArguments.EventData;

			if (payload.pathfindingAgent != pathfindingAgent) // event not for us
			{
				return false;
			}

			IsFollowing = false;
			CancelPath(ref pathToFollow, ref edgeToFollow);
			
			return true;
		}

		#endregion Pathfinding Events
	}
}