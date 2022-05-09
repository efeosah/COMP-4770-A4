using GameBrains.Actuators.Motion.Navigation.PathManagement;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;

using GameBrains.Entities;
using GameBrains.Entities.Types;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using UnityEngine;

namespace Testing
{
    public class A4Testing : ExtendedMonoBehaviour
    {
        // TODO for A4: Add appropriate testing for LevelInfo and CornerGraphSearchSpace.
        
        #region Members and Properties
        
        [Header("Options")]
        [SerializeField] bool showPath = true;
        [SerializeField] bool smoothPath = true;
        [SerializeField] bool showClosestNodeVisualizer;
        [SerializeField] bool showClosestToEntityWithTypeVisualizer;
        [SerializeField] bool showDirectPathVisualizer;
        
        [Header("Respawn")]
        public bool respawn;
        public VectorXYZ spawnPoint;
        
        [Header("Test Path to Location")]
        public bool testPathToLocation;
        public VectorXZ destination;
        
        [Header("Test Path to Entity with Type")]
        public bool testPathToEntityWithType;
        public EntityType entityType;
        
        [Header("Test Path to Random Location")]
        public bool testPathToRandomLocation;

        [Header("References")]
        public PathfindingAgent pathfindingAgent;
        public PathPlanner pathPlanner;

        // TODO for A4: Add your fields and properties here.

        public Graph graph;
        [Header("Test Path to Random Node")]
        public bool testPathToRandomNode;


        [Header("Test Path to Random Node Continous ")]
        public bool testPathToRandomNodeContinous;
        bool isFirstTrip = true;
        bool isPathComplete;
        Node targetNode;



        #endregion Members and Properties

        // TODO for A4: Add Awake and/or Start, etc. here if need.
        //public override void Start()
        //{
        //    base.Start();

            
        //}

        #region Update

        public override void Update()
        {
            base.Update();

            if (pathfindingAgent != null)
            {
                pathfindingAgent.Data.ShowPath = showPath;
                pathfindingAgent.Data.SmoothPath = smoothPath;
                pathfindingAgent.Data.ShowClosestNodeVisualizer = showClosestNodeVisualizer;
                pathfindingAgent.Data.ShowClosestToEntityWithTypeVisualizer =
                    showClosestToEntityWithTypeVisualizer;
            }

            if (pathPlanner != null)
            {
                pathPlanner.ShowDirectPathVisualizer = showDirectPathVisualizer;
            }
            
            if (respawn)
            {
                respawn = false;
                pathfindingAgent.Spawn(spawnPoint);
            }

            if (testPathToLocation)
            {
                testPathToLocation = false;

                pathfindingAgent.Data.FindPathTo(destination);
            }
            
            if (testPathToRandomLocation)
            {
                testPathToRandomLocation = false;

                var location = new VectorXZ(Random.Range(-49, 49), Random.Range(-36, 36));

                pathfindingAgent.Data.FindPathTo(location);
            }

            if (testPathToEntityWithType)
            {
                testPathToEntityWithType = false;

                pathfindingAgent.Data.FindPathTo(entityType);
            }


            // TODO for A4: Add your tests here.

            if (testPathToRandomNode)
            {
                testPathToRandomNode = false;

                int randPoint = Random.Range(0, graph.NodeCollection.Nodes.Length);

                var node = graph.NodeCollection.Nodes[randPoint];

                //pathfindingAgent.Data.FindPathTo(node.Location);

                if (targetNode)
                {
                    pathfindingAgent.Data.FindPathTo(targetNode.Location);
                }
                else
                {
                    pathfindingAgent.Data.FindPathTo(node.Location);
                }


            }

            if (testPathToRandomNodeContinous)
            {
                if (isFirstTrip)
                {
                    targetNode = FindRandomNode();
                    testPathToRandomNode = true;
                    isFirstTrip = false;
                }

                isPathComplete = VectorXZ.Distance(targetNode.Location,pathfindingAgent.Data.Location) < 0.4f;

                if (isPathComplete)
                {
                    targetNode = FindRandomNode();
                    testPathToRandomNode = true;
                }


               
            }

        }

        #endregion Update


        private Node FindRandomNode()
        {

            int randPoint = Random.Range(0, graph.NodeCollection.Nodes.Length);
            var node = graph.NodeCollection.Nodes[randPoint];

            return node;
        }
    }
}