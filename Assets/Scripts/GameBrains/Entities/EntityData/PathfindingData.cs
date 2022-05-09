using GameBrains.Actuators.Motion.Navigation.PathManagement;
using GameBrains.Actuators.Motion.Navigation.SearchAlgorithms;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.Actuators.Motion.Steering;
using GameBrains.Entities.Types;
using GameBrains.EventSystem;
using GameBrains.Extensions.Vectors;
using GameBrains.Motion.Steering.VelocityBased;
using GameBrains.Visualization;
using UnityEngine;

namespace GameBrains.Entities.EntityData
{
    [System.Serializable]
    public class PathfindingData : SteeringData
    {
        #region Creators

        public static PathfindingData CreatePathfindingDataInstance(Transform t)
        {
            PathfindingData pathfindingData = CreateInstance<PathfindingData>();
            InitializePathfindingData(t, pathfindingData);
            return pathfindingData;
        }

        protected static void InitializePathfindingData(
            Transform t,
            PathfindingData pathfindingData)
        {
            InitializeSteeringData(t, pathfindingData);
        }

        #endregion Creators

        #region Casting

        public new PathfindingAgent Owner => owner as PathfindingAgent;

        public static implicit operator PathfindingData(Transform t)
        {
            return CreatePathfindingDataInstance(t);
        }

        #endregion Casting

        #region Enable/Disable

        public override void OnEnable()
        {
            base.OnEnable();
            
            graph = FindObjectOfType<Graph>();
        }
        
        #endregion Enable/Disable

        #region Pathfinding Data

        public PathManager PathManager
        {
            get
            {
                if (pathManager == null) { pathManager = Owner.GetComponent<PathManager>(); }
                return pathManager;
            }
            set => pathManager = value;
        }
        PathManager pathManager;
        
        public PathPlanner PathPlanner
        {
            get
            {
                if (pathPlanner == null) { pathPlanner = Owner.GetComponent<PathPlanner>(); }
                return pathPlanner;
            }
            set => pathPlanner = value;
        }
        PathPlanner pathPlanner;
        
        public PathFollower PathFollower
        {
            get
            {
                if (pathFollower == null) { pathFollower = Owner.GetComponent<PathFollower>(); }
                return pathFollower;
            }
            set => pathFollower = value;
        }
        PathFollower pathFollower;

        public bool ShowPath
        {
            get => showPath;
            set
            {
                showPath = value;

                if (PathFollower != null)
                {
                    if (PathFollower.PathToFollow != null)
                    {
                        PathFollower.PathToFollow.Show(showPath);
                    }

                    if (PathFollower.EdgeToFollow != null)
                    {
                        PathFollower.EdgeToFollow.Show(showPath);
                    }
                }
            }
        }
        [SerializeField] bool showPath = true;
        
        public bool SmoothPath
        {
            get => smoothPath;
            set => smoothPath = value;
        }
        [SerializeField] bool smoothPath = true;

        CapsuleCastVisualizer closestNodeVisualizer;
        CapsuleCastVisualizer closestToEntityWithTypeVisualizer;
        public bool ShowClosestNodeVisualizer
        {
            get => showClosestNodeVisualizer;
            set => showClosestNodeVisualizer = value;
        }
        [SerializeField] bool showClosestNodeVisualizer = true;
		
        public bool ShowClosestNodeVisualizerOnlyWhenBlocked
        {
            get => showClosestNodeVisualizerOnlyWhenBlocked;
            set => showClosestNodeVisualizerOnlyWhenBlocked = value;
        }
        [SerializeField] bool showClosestNodeVisualizerOnlyWhenBlocked;
		
        public float ShowClosestNodeVisualizerCastRadius
        {
            get => showClosestNodeVisualizerCastRadius;
            set => showClosestNodeVisualizerCastRadius = value;
        }
        [SerializeField] float showClosestNodeVisualizerCastRadius = 1f;
		
        public Color ShowClosestNodeVisualizerClearColor
        {
            get => showClosestNodeVisualizerClearColor;
            set => showClosestNodeVisualizerClearColor = value;
        }
        [SerializeField] Color showClosestNodeVisualizerClearColor = Color.green;
		
        public Color ShowClosestNodeVisualizerBlockedColor
        {
            get => showClosestNodeVisualizerBlockedColor;
            set => showClosestNodeVisualizerBlockedColor = value;
        }
        [SerializeField] Color showClosestNodeVisualizerBlockedColor = Color.red;
        
        public bool ShowClosestToEntityWithTypeVisualizer
        {
            get => showClosestToEntityWithTypeVisualizer;
            set => showClosestToEntityWithTypeVisualizer = value;
        }
        [SerializeField] bool showClosestToEntityWithTypeVisualizer = true;
		
        public bool ShowClosestToEntityWithTypeVisualizerOnlyWhenBlocked
        {
            get => showClosestToEntityWithTypeVisualizerOnlyWhenBlocked;
            set => showClosestToEntityWithTypeVisualizerOnlyWhenBlocked = value;
        }
        [SerializeField] bool showClosestToEntityWithTypeVisualizerOnlyWhenBlocked;
		
        public float ShowClosestToEntityWithTypeVisualizerCastRadius
        {
            get => showClosestToEntityWithTypeVisualizerCastRadius;
            set => showClosestToEntityWithTypeVisualizerCastRadius = value;
        }
        [SerializeField] float showClosestToEntityWithTypeVisualizerCastRadius = 1f;
		
        public Color ShowClosestToEntityWithTypeVisualizerClearColor
        {
            get => showClosestToEntityWithTypeVisualizerClearColor;
            set => showClosestToEntityWithTypeVisualizerClearColor = value;
        }
        [SerializeField] Color showClosestToEntityWithTypeVisualizerClearColor = Color.blue;
		
        public Color ShowClosestToEntityWithTypeVisualizerBlockedColor
        {
            get => showClosestToEntityWithTypeVisualizerBlockedColor;
            set => showClosestToEntityWithTypeVisualizerBlockedColor = value;
        }
        [SerializeField] Color showClosestToEntityWithTypeVisualizerBlockedColor = Color.black;

        public int OverlapSphereMaximumColliders
        {
            get => overlapSphereMaximumColliders;
            set => overlapSphereMaximumColliders = value;
        }
        [SerializeField] int overlapSphereMaximumColliders = 10;

        public float OverlapSphereRadius
        {
            get => overlapSphereRadius;
            set => overlapSphereRadius = value;
        }
        [SerializeField] float overlapSphereRadius = 6f;

        Graph graph;
        SteeringBehaviour steering;

        #endregion Pathfinding Data

        #region Pathfinding

        public void FindPathTo(VectorXZ location)
        {
            EventManager.Instance.Enqueue(
                Events.PathToLocationRequest,
                new PathToLocationRequestEventPayload(Owner, location));
        }

        public int SeekToLocation(VectorXZ location)
        {
            if (steering != null) { RemoveSteeringBehaviour(steering.ID); }
            steering = Seek.CreateInstance(this, location);
            ((Seek)steering).LinearDrag = 1.05f;
            ((Seek)steering).CloseEnoughDistance = 1.5f;
            return AddSteeringBehaviour(steering);
        }

        public int ArriveAtLocation(VectorXZ location)
        {
            if (steering != null) { RemoveSteeringBehaviour(steering.ID); }
            steering = Arrive.CreateInstance(this, location);
            ((Arrive)steering).CloseEnoughDistance = 1.5f;
            return AddSteeringBehaviour(steering);
        }
        
        public void FindPathTo(EntityType entityType)
        {
            EventManager.Instance.Enqueue(
                Events.PathToEntityWithTypeRequest,
                new PathToEntityWithTypeRequestEventPayload(Owner, entityType));
        }
        
        public Node ClosestNodeToLocation(VectorXZ position)
        {
            float closestSoFar = float.MaxValue;
            Node closestNode = null;

            foreach (Node node in graph.NodeCollection.Nodes)
            {
                float distance = Vector3.Distance(node.Position, (VectorXYZ)position);

                if (distance >= closestSoFar) { continue; }

                closestNodeVisualizer = ShowClosestNodeVisualizer ? CreateInstance<CapsuleCastVisualizer>() : null;
                if (closestNodeVisualizer) { closestNodeVisualizer.destroyInsteadOfHide = true; }

                if (CanMoveBetween(
                    (VectorXYZ)position, 
                    node.Position,
                    closestNodeVisualizer,
                    ShowClosestNodeVisualizer,
                    ShowClosestNodeVisualizerOnlyWhenBlocked,
                    ShowClosestNodeVisualizerCastRadius,
                    ShowClosestNodeVisualizerClearColor,
                    ShowClosestNodeVisualizerBlockedColor))
                {
                    closestNode = node;
                    closestSoFar = distance;
                }
            }

            return closestNode;
        }
        
        public float CostToClosestEntityWithType(EntityType entityType)
        {
            float closestSoFar = float.MaxValue;

            Node closestNodeToAgent = ClosestNodeToLocation(Location);

            if (closestNodeToAgent == null) { return closestSoFar; }

            foreach (Node node in graph.NodeCollection.Nodes)
            {
                if (!NodeIsCloseToEntityOfType(node, entityType, out Entity _)) { continue; }

                float cost = BestPathTable.Cost(closestNodeToAgent, node);

                if (cost < closestSoFar) { closestSoFar = cost; }
            }

            return closestSoFar;
        }
        
        public bool NodeIsCloseToEntityOfType(Node node, EntityType entityType, out Entity foundEntity)
        {
            Collider[] hitColliders = new Collider[OverlapSphereMaximumColliders];
            int numColliders
                = Physics.OverlapSphereNonAlloc(
                    node.Position,
                    OverlapSphereRadius,
                    hitColliders,
                    Physics.AllLayers, // TODO: Should this be Default and Obstacle layers only
                    QueryTriggerInteraction.Collide); // consider triggers (needed for pickups)

            for (int i = 0; i < numColliders; i++)
            {
                foundEntity = hitColliders[i].GetComponent<Entity>();
                if (foundEntity != null && foundEntity.entityTypes.Contains(entityType))
                {
                    closestToEntityWithTypeVisualizer = 
                        ShowClosestToEntityWithTypeVisualizer ? CreateInstance<CapsuleCastVisualizer>() : null;
                    if (closestToEntityWithTypeVisualizer)
                    {
                        closestToEntityWithTypeVisualizer.destroyInsteadOfHide = true;
                    }
                    
                    if (CanMoveBetween(
                        (VectorXYZ)((VectorXZ)node.Position),
                        (VectorXYZ)foundEntity.Data.Location,
                        closestToEntityWithTypeVisualizer,
                        ShowClosestToEntityWithTypeVisualizer,
                        ShowClosestToEntityWithTypeVisualizerOnlyWhenBlocked,
                        ShowClosestToEntityWithTypeVisualizerCastRadius,
                        ShowClosestToEntityWithTypeVisualizerClearColor,
                        ShowClosestToEntityWithTypeVisualizerBlockedColor))
                    {
                        return true;
                    }
                }
            }

            foundEntity = null;
            return false;
        }

        #endregion Pathfinding
    }
}