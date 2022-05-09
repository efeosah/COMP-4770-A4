using GameBrains.Actuators.Motion.Navigation.PathManagement;
using GameBrains.Entities;
using GameBrains.Entities.Types;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.Visualization;
using UnityEngine;

namespace Testing
{
    public class W25TestPathVisualization : ExtendedMonoBehaviour
    {
        #region Members and Properties
        
        [Header("Options")]
        [SerializeField] bool showPath = true;
        [SerializeField] bool smoothPath;
        [SerializeField] bool showClosestNodeVisualizer = true;
        [SerializeField] bool showClosestToEntityWithTypeVisualizer = true;
        [SerializeField] bool showDirectPathVisualizer = true;
        
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

        [Header("Test Point Marker")]
        [SerializeField] bool testPointMarker;
        [SerializeField] VectorXZ pointMarkerLocation;

        [Header("Test Point Marker Hide")]
        [SerializeField] bool testPointMarkerHide;
        [SerializeField] bool testPointMarkerDestroy;
        
        [Header("Test Point Beacon")]
        [SerializeField] bool testPointBeacon;
        [SerializeField] VectorXZ pointBeaconLocation;

        [Header("Test Point Beacon Hide")]
        [SerializeField] bool testPointBeaconHide;
        [SerializeField] bool testPointBeaconDestroy;
        
        [Header("Test Edge Beacon")]
        [SerializeField] bool testEdgeBeacon;
        [SerializeField] VectorXZ edgeBeaconStartLocation;
        [SerializeField] VectorXZ edgeBeaconEndLocation;

        [Header("Test Edge Beacon Hide")]
        [SerializeField] bool testEdgeBeaconHide;
        
        [Header("Test Edge Beacon Destroy")]
        [SerializeField] bool testEdgeBeaconDestroy;
        
        [Header("References")]
        public PathfindingAgent pathfindingAgent;
        public PathPlanner pathPlanner;

        PointMarker pointMarker;
        PointBeacon pointBeacon;
        EdgeBeacon edgeBeacon;
        bool shouldHidePointMarker;
        bool shouldHidePointBeacon;
        bool shouldHideEdgeBeacon;
        
        #endregion Members and Properties

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

            if (testPointMarker)
            {
                testPointMarker = false;

                if (pointMarker != null)
                {
                    Destroy(pointMarker);
                }

                pointMarker = ScriptableObject.CreateInstance<PointMarker>();
                pointMarker.Draw(pointMarkerLocation);
            }

            if (testPointMarkerHide)
            {
                testPointMarkerHide = false;

                if (pointMarker != null)
                {
                    shouldHidePointMarker = !shouldHidePointMarker;
                    pointMarker.Hide(shouldHidePointMarker);
                }
            }

            if (testPointMarkerDestroy)
            {
                testPointMarkerDestroy = false;

                if (pointMarker != null)
                {
                    Destroy(pointMarker);
                    pointMarker = null;
                }
            }

            if (testPointBeacon)
            {
                testPointBeacon = false;

                if (pointBeacon != null)
                {
                    Destroy(pointBeacon);
                }

                pointBeacon = ScriptableObject.CreateInstance<PointBeacon>();
                pointBeacon.Draw(pointBeaconLocation);
            }

            if (testPointBeaconHide)
            {
                testPointBeaconHide = false;

                if (pointBeacon != null)
                {
                    shouldHidePointBeacon = !shouldHidePointBeacon;
                    pointBeacon.Hide(shouldHidePointBeacon);
                }
            }

            if (testPointBeaconDestroy)
            {
                testPointBeaconDestroy = false;

                if (pointBeacon != null)
                {
                    Destroy(pointBeacon);
                    pointBeacon = null;
                }
            }

            if (testEdgeBeacon)
            {
                testEdgeBeacon = false;

                if (edgeBeacon != null)
                {
                    Destroy(edgeBeacon);
                }

                edgeBeacon = ScriptableObject.CreateInstance<EdgeBeacon>();
                edgeBeacon.Draw(edgeBeaconStartLocation, edgeBeaconEndLocation);
            }

            if (testEdgeBeaconHide)
            {
                testEdgeBeaconHide = false;

                if (edgeBeacon != null)
                {
                    shouldHideEdgeBeacon = !shouldHideEdgeBeacon;
                    edgeBeacon.Hide(shouldHideEdgeBeacon);
                }
            }

            if (testEdgeBeaconDestroy)
            {
                testEdgeBeaconDestroy = false;

                if (edgeBeacon != null)
                {
                    Destroy(edgeBeacon);
                    edgeBeacon = null;
                }
            }
        }

        #endregion Update
    }
}