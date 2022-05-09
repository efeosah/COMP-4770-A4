using System.ComponentModel;
using GameBrains.Actuators.Motion.Navigation.PathManagement;
using GameBrains.Entities;
using GameBrains.Entities.Types;
using GameBrains.Extensions.Vectors;

// ReSharper disable once CheckNamespace
namespace GameBrains.EventSystem // NOTE: Don't change this namespace
{
	#region Events
	
	public static partial class Events
    {
        [Description("PathToLocationRequest")]
        public static readonly EventType PathToLocationRequest = (EventType)Count++;

		[Description("PathToLocationReady")]
        public static readonly EventType PathToLocationReady = (EventType)Count++;

		[Description("NoPathToLocationAvailable")]
        public static readonly EventType NoPathToLocationAvailable = (EventType)Count++;

		[Description("PathToEntityWithTypeRequest")]
        public static readonly EventType PathToEntityWithTypeRequest = (EventType)Count++;

		[Description("PathToEntityWithTypeReady")]
        public static readonly EventType PathToEntityWithTypeReady = (EventType)Count++;

		[Description("NoPathToEntityWithTypeAvailable")]
        public static readonly EventType NoPathToEntityWithTypeAvailable = (EventType)Count++;

		[Description("FollowCompleted")]
        public static readonly EventType FollowCompleted = (EventType)Count++;

		[Description("FollowFailed")]
        public static readonly EventType FollowFailed = (EventType)Count++;

		[Description("TraversalCompleted")]
        public static readonly EventType TraversalCompleted = (EventType)Count++;

		[Description("TraversalFailed")]
        public static readonly EventType TraversalFailed = (EventType)Count++;
    }
	
	#endregion Events

	#region Event Payloads
	
	public struct PathToLocationRequestEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public VectorXZ destination;
		
		public PathToLocationRequestEventPayload(
			PathfindingAgent pathfindingAgent,
	        VectorXZ destination)
	    {
	        this.pathfindingAgent = pathfindingAgent;
	        this.destination = destination;
	    }
	}
	
	public struct PathToLocationReadyEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public Path path;
		
		public PathToLocationReadyEventPayload(
			PathfindingAgent pathfindingAgent,
	        Path path)
	    {
	        this.pathfindingAgent = pathfindingAgent;
	        this.path = path;
	    }
	}
	
	public struct NoPathToLocationAvailableEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		
		public NoPathToLocationAvailableEventPayload(PathfindingAgent pathfindingAgent)
	    {
	        this.pathfindingAgent = pathfindingAgent;
	    }
	}
	
	public struct PathToEntityWithTypeRequestEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public EntityType entityType;

		public PathToEntityWithTypeRequestEventPayload(
			PathfindingAgent pathfindingAgent,
			EntityType entityType)
	    {
	        this.pathfindingAgent = pathfindingAgent;
			this.entityType = entityType;
	    }
	}
	
	public struct PathToEntityWithTypeReadyEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public Path path;
		public Entity entity;
		
		public PathToEntityWithTypeReadyEventPayload(
			PathfindingAgent pathfindingAgent,
	        Path path,
		    Entity entity)
	    {
	        this.pathfindingAgent = pathfindingAgent;
	        this.path = path;
			this.entity = entity;
	    }
	}
	
	public struct NoPathToEntityWithTypeAvailableEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		
		public NoPathToEntityWithTypeAvailableEventPayload(PathfindingAgent pathfindingAgent)
	    {
	        this.pathfindingAgent = pathfindingAgent;
	    }
	}
	
	public struct FollowCompletedEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public Path path;

		public FollowCompletedEventPayload(
			PathfindingAgent pathfindingAgent,
			Path path)
		{
			this.pathfindingAgent = pathfindingAgent;
			this.path = path;
		}
	}

	public struct FollowFailedEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public Path path;

		public FollowFailedEventPayload(
			PathfindingAgent pathfindingAgent,
			Path path)
		{
			this.pathfindingAgent = pathfindingAgent;
			this.path = path;
		}
	}
	
	public struct TraversalCompletedEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public QuasiEdge quasiEdge;

		public TraversalCompletedEventPayload(
			PathfindingAgent pathfindingAgent,
			QuasiEdge quasiEdge)
		{
			this.pathfindingAgent = pathfindingAgent;
			this.quasiEdge = quasiEdge;
		}
	}

	public struct TraversalFailedEventPayload
	{
		public PathfindingAgent pathfindingAgent;
		public QuasiEdge quasiEdge;

		public TraversalFailedEventPayload(
			PathfindingAgent pathfindingAgent,
			QuasiEdge quasiEdge)
		{
			this.pathfindingAgent = pathfindingAgent;
			this.quasiEdge = quasiEdge;
		}
	}

	#endregion Event Payloads
}