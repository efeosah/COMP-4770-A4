using System.ComponentModel;
using GameBrains.Extensions.Attributes;
using GameBrains.Extensions.MonoBehaviours;
using UnityEngine;

namespace GameBrains.GameManagement
{
	public sealed class Parameters : ExtendedMonoBehaviour
	{
		#region Parameters Singleton

		static Parameters instance;

		public static Parameters Instance
		{
			get
			{
				if (!instance)
				{
					instance = FindObjectOfType<Parameters>();
				}

				return instance;
			}
		}

		#endregion Parameters Singleton
		
		#region Resource Path Settings

		[Foldout("Resource Path Settings")] [SerializeField]
		string entityTypePath = "ScriptableObjects/EntityTypes/";
		
		// Gets or sets the path to EntityType scriptable objects.
		[Category("Resource Path Settings")]
		[Description("The path to EntityType scriptable objects.")]
		public string EntityTypePath
		{
			get => entityTypePath;
			set => entityTypePath = value;
		}
		
		[Foldout("Resource Path Settings")] [SerializeField]
		string entityTypeFrameworkPath = "ScriptableObjects/EntityTypes/Framework/";
		
		// Gets or sets the path to EntityType Framework scriptable objects.
		[Category("Resource Path Settings")]
		[Description("The path to EntityType Framework scriptable objects.")]
		public string EntityTypeFrameworkPath
		{
			get => entityTypeFrameworkPath;
			set => entityTypeFrameworkPath = value;
		}

		[Foldout("Resource Path Settings")] [SerializeField]
		string defaultEntityTypePath = "ScriptableObjects/EntityTypes/Framework/Default";
		
		// Gets or sets the path to the default EntityType scriptable objects.
		[Category("Resource Path Settings")]
		[Description("The path to the default EntityType scriptable objects.")]
		public string DefaultEntityTypePath
		{
			get => defaultEntityTypePath;
			set => defaultEntityTypePath = value;
		}

		#endregion Resource Path Settings

		#region Visualizer Settings

		[Foldout("Visualizer Settings")] [SerializeField]
		float visualizeThreshold = 0.1f;
		
		// Gets or sets the smallest ray or capsule to visualize.
		[Category("Visualizer Settings")]
		[Description("Hide ray or capsule visualizer smaller than this size.")]
		public float VisualizeThreshold
		{
			get => visualizeThreshold;
			set => visualizeThreshold = value;
		}
		
		[Foldout("Visualizer Settings")] [SerializeField]
		float visualizerHideAfter = 1f;
		
		// Gets or sets the time in seconds after which to hide the visualizer.
		[Category("Visualizer Settings")]
		[Description("Hide the visualizer after these seconds.")]
		public float VisualizerHideAfter
		{
			get => visualizerHideAfter;
			set => visualizerHideAfter = value;
		}

		#region Point Marker Settings

		[Foldout("Visualizer Settings")] [SerializeField]
		float pointMarkerDropFromHeightOffset = 10f; // should be higher than the highest point.
		
		// Gets or sets the height offset to drop a point marker from.
		[Category("Visualizer Settings")]
		[Description("The height offset to drop a point marker from.")]
		public float PointMarkerDropFromHeightOffset
		{
			get => pointMarkerDropFromHeightOffset;
			set => pointMarkerDropFromHeightOffset = value;
		}

		[Foldout("Visualizer Settings")] [SerializeField]
		float pointMarkerRadius = 0.5f;
		
		// Gets or sets the point marker radius.
		[Category("Visualizer Settings")]
		[Description("The point marker radius.")]
		public float PointMarkerRadius
		{
			get => pointMarkerRadius;
			set => pointMarkerRadius = value;
		}

		[Foldout("Visualizer Settings")] [SerializeField]
		float pointMarkerSurfaceOffset = 0f;
		
		// Gets or sets the offset from the surface to drop the point marker.
		[Category("Visualizer Settings")]
		[Description("The offset from the surface to drop the point marker.")]
		public float PointMarkerSurfaceOffset
		{
			get => pointMarkerSurfaceOffset;
			set => pointMarkerSurfaceOffset = value;
		}
		
		#endregion Point Marker Settings
		
		#region Point Beacon Settings

		[Foldout("Visualizer Settings")] [SerializeField]
		float pointBeaconDropFromHeightOffset = 10f; // should be higher than the highest point.
		
		// Gets or sets the height offset to drop a point beacon from.
		[Category("Visualizer Settings")]
		[Description("The height offset to drop a point beacon from.")]
		public float PointBeaconDropFromHeightOffset
		{
			get => pointBeaconDropFromHeightOffset;
			set => pointBeaconDropFromHeightOffset = value;
		}

		[Foldout("Visualizer Settings")] [SerializeField]
		float pointBeaconRadius = 0.5f;
		
		// Gets or sets the point beacon radius.
		[Category("Visualizer Settings")]
		[Description("The point beacon radius.")]
		public float PointBeaconRadius
		{
			get => pointBeaconRadius;
			set => pointBeaconRadius = value;
		}
		
		[Foldout("Visualizer Settings")] [SerializeField]
		float pointBeaconHeight = 5f;
		
		// Gets or sets the point beacon height.
		[Category("Visualizer Settings")]
		[Description("The point beacon height.")]
		public float PointBeaconHeight
		{
			get => pointBeaconHeight;
			set => pointBeaconHeight = value;
		}

		[Foldout("Visualizer Settings")] [SerializeField]
		float pointBeaconSurfaceOffset = 0f;
		
		// Gets or sets the offset from the surface to drop the point beacon.
		[Category("Visualizer Settings")]
		[Description("The offset from the surface to drop the point beacon.")]
		public float PointBeaconSurfaceOffset
		{
			get => pointBeaconSurfaceOffset;
			set => pointBeaconSurfaceOffset = value;
		}
		
		#endregion Point Beacon Settings
		
		#region Edge Beacon Settings

		[Foldout("Visualizer Settings")] [SerializeField]
		float edgeBeaconDropFromHeightOffset = 10f; // should be higher than the highest point.
		
		// Gets or sets the height offset to drop an edge beacon from.
		[Category("Visualizer Settings")]
		[Description("The height offset to drop an edge beacon from.")]
		public float EdgeBeaconDropFromHeightOffset
		{
			get => edgeBeaconDropFromHeightOffset;
			set => edgeBeaconDropFromHeightOffset = value;
		}

		[Foldout("Visualizer Settings")] [SerializeField]
		float edgeBeaconRadius = 0.5f;
		
		// Gets or sets the edge beacon radius.
		[Category("Visualizer Settings")]
		[Description("The edge beacon radius.")]
		public float EdgeBeaconRadius
		{
			get => edgeBeaconRadius;
			set => edgeBeaconRadius = value;
		}

		[Foldout("Visualizer Settings")] [SerializeField]
		float edgeBeaconSurfaceOffset = 0f;
		
		// Gets or sets the offset from the surface to drop the edge beacon.
		[Category("Visualizer Settings")]
		[Description("The offset from the surface to drop the edge beacon.")]
		public float EdgeBeaconSurfaceOffset
		{
			get => edgeBeaconSurfaceOffset;
			set => edgeBeaconSurfaceOffset = value;
		}
		
		#endregion Edge Beacon Settings

		#endregion Visualizer Settings

		#region Obstacle Settings

		[Foldout("Obstacle Settings")] [SerializeField]
		LayerMask obstacleLayerMask;
		
		// Gets the obstacle layer mask. There is currently no setter. We are assuming for now
		// that all obstacles are on one layer. This could be changed in the future.
		[Category("Obstacle Settings")]
		[Description("The obstacle layer.")]
		public LayerMask ObstacleLayerMask
		{
			get => obstacleLayerMask;
			set => obstacleLayerMask = value;
		}
		
		[Foldout("Ground Settings")] [SerializeField]
		LayerMask groundLayerMask;
		
		// Gets the ground layer mask. There is currently no setter. We are assuming for now
		// that all ground is on one layer. This could be changed in the future.
		[Category("Ground Settings")]
		[Description("The ground layer.")]
		public LayerMask GroundLayerMask
		{
			get => groundLayerMask;
			set => groundLayerMask = value;
		}

		#endregion Obstacle Settings

		#region Navigation Settings
		
		#region Path Management Settings

		[Foldout("Path Management Settings")] [SerializeField]
		public int maximumSearchCyclesPerUpdateStep = 1000;
		
		// Gets or sets the maximum number of search cycles allocated to each path planning search per update.
		[Category("Path Management Settings")]
		[Description("The maximum number of search cycles allocated to each path planning search per update.")]
		public int MaximumSearchCyclesPerUpdateStep
		{
			get => maximumSearchCyclesPerUpdateStep;
			set => maximumSearchCyclesPerUpdateStep = value;
		}

		#endregion Path Management Settings

		#region Search Graph Settings

		#region Node Settings

		[Foldout("Node Settings")] [SerializeField]
		float nodeDropFromHeightOffset = 10f; // should be higher than the highest point.
		
		// Gets or sets the height offset to drop a node from.
		[Category("Node Settings")]
		[Description("The height offset to drop a node from.")]
		public float NodeDropFromHeightOffset
		{
			get => nodeDropFromHeightOffset;
			set => nodeDropFromHeightOffset = value;
		}

		[Foldout("Node Settings")] [SerializeField]
		float nodeRadius = 0.5f;
		
		// Gets or sets the node radius.
		[Category("Node Settings")]
		[Description("The node radius.")]
		public float NodeRadius
		{
			get => nodeRadius;
			set => nodeRadius = value;
		}

		[Foldout("Node Settings")] [SerializeField]
		bool nodeUseCapsuleCast;
		
		// Gets or sets whether to use capsule cast for checking node line of sight.
		[Category("Node Settings")]
		[Description("Use capsule cast for connecting node line of sight.")]
		public bool NodeUseCapsuleCast
		{
			get => nodeUseCapsuleCast;
			set => nodeUseCapsuleCast = value;
		}

		[Foldout("Node Settings")] [SerializeField]
		float nodeCastMaximumDistance = 20f;
		
		// Gets or sets maximum distance for checking node line of sight.
		[Category("Node Settings")]
		[Description("Maximum distance for checking node line of sight.")]
		public float NodeCastMaximumDistance
		{
			get => nodeCastMaximumDistance;
			set => nodeCastMaximumDistance = value;
		}

		[Foldout("Node Settings")] [SerializeField]
		float nodeCastPathRadius = 0.5f;
		
		// Gets or sets the radius for checking node line of sight.
		[Category("Node Settings")]
		[Description("Radius for checking node line of sight.")]
		public float NodeCastPathRadius
		{
			get => nodeCastPathRadius;
			set => nodeCastPathRadius = value;
		}

		[Foldout("Node Settings")] [SerializeField]
		float nodeSurfaceOffset = 1.1f;
		
		// Gets or sets the offset from the surface to drop the node.
		[Category("Node Settings")]
		[Description("The offset from the surface to drop the node.")]
		public float NodeSurfaceOffset
		{
			get => nodeSurfaceOffset;
			set => nodeSurfaceOffset = value;
		}

		#endregion Node Settings

		#region Node Collection Settings

		[Foldout("Node Collection Settings")] [SerializeField]
		Color nodeDefaultColor = Color.yellow;
		
		// Gets or sets the default node color.
		[Category("Node Collection Settings")]
		[Description("Default node color.")]
		public Color NodeDefaultColor
		{
			get
			{
				nodeDefaultColor.a = NodeDefaultAlpha;
				return nodeDefaultColor;
			}
			set => nodeDefaultColor = value;
		}

		[Foldout("Node Collection Settings")] [SerializeField]
		float nodeDefaultAlpha = 0.25f;
		
		// Gets or sets the default alpha for node color.
		[Category("Node Collection Settings")]
		[Description("Default alpha for node color.")]
		public float NodeDefaultAlpha
		{
			get => nodeDefaultAlpha;
			set => nodeDefaultAlpha = value;
		}

		[Foldout("Node Collection Settings")] [SerializeField]
		bool nodeCollectionIsVisible = true;
		
		// Gets or sets the whether the nodes in the node collection are visible.
		[Category("Node Collection Settings")]
		[Description("Whether the nodes in the node collection are visible.")]
		public bool NodeCollectionIsVisible
		{
			get => nodeCollectionIsVisible;
			set => nodeCollectionIsVisible = value;
		}

		#endregion Node Collection Settings

		#region Edge Settings

		#endregion Edge Settings

		#region Edge Collection Settings

		[Foldout("Edge Collection Settings")] [SerializeField]
		Color edgeDefaultColor = Color.blue;
		
		// Gets or sets the default edge color.
		[Category("Edge Collection Settings")]
		[Description("Default edge color.")]
		public Color EdgeDefaultColor
		{
			get
			{
				edgeDefaultColor.a = EdgeDefaultAlpha;
				return edgeDefaultColor;
			}
			set => edgeDefaultColor = value;
		}

		[Foldout("Edge Collection Settings")] [SerializeField]
		float edgeDefaultAlpha = 0.25f;
		
		// Gets or sets the default alpha for edge color.
		[Category("Edge Collection Settings")]
		[Description("Default alpha for edge color.")]
		public float EdgeDefaultAlpha
		{
			get => edgeDefaultAlpha;
			set => edgeDefaultAlpha = value;
		}

		[Foldout("Edge Collection Settings")] [SerializeField]
		bool edgeCollectionIsVisible = true;
		
		// Gets or sets the whether the edges in the edge collection are visible.
		[Category("Edge Collection Settings")]
		[Description("Whether the edges in the edge collection are visible.")]
		public bool EdgeCollectionIsVisible
		{
			get => edgeCollectionIsVisible;
			set => edgeCollectionIsVisible = value;
		}

		#endregion Edge Collection Settings

		#region Graph Settings

		[Foldout("Graph Settings")] [SerializeField]
		string nodePrefabPath = "Prefabs/Navigation/SearchGraph/Node";
		
		// Gets or sets the path to the prefab to use for creating new nodes.
		[Category("Graph Settings")]
		[Description("The path to the prefab to use for creating new nodes.")]
		public string NodePrefabPath
		{
			get => nodePrefabPath;
			set => nodePrefabPath = value;
		}
		
		[Foldout("Graph Settings")] [SerializeField]
		string nodeMaterialPath = "Materials/NavigationMaterials/NodeMaterial";
		
		// Gets or sets the path to the material to use for creating new nodes.
		[Category("Graph Settings")]
		[Description("The path to the material to use for creating new nodes.")]
		public string NodeMaterialPath
		{
			get => nodeMaterialPath;
			set => nodeMaterialPath = value;
		}

		[Foldout("Graph Settings")] [SerializeField]
		string edgePrefabPath = "Prefabs/Navigation/SearchGraph/Edge";
		
		// Gets or sets the path to the prefab to use for creating new edges.
		[Category("Graph Settings")]
		[Description("The path to the prefab to use for creating new edges.")]
		public string EdgePrefabPath
		{
			get => edgePrefabPath;
			set => edgePrefabPath = value;
		}
		
		[Foldout("Graph Settings")] [SerializeField]
		string edgeMaterialPath = "Materials/NavigationMaterials/EdgeMaterial";
		
		// Gets or sets the path to the material to use for creating new edges.
		[Category("Graph Settings")]
		[Description("The path to the material to use for creating new edges.")]
		public string EdgeMaterialPath
		{
			get => edgeMaterialPath;
			set => edgeMaterialPath = value;
		}

		#endregion Graph Settings

		#endregion Search Graph Settings

		#endregion Navigation Settings
	}
}