using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using UnityEditor;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	public class NodeCollection : ExtendedMonoBehaviour
	{
		// Lock to prevent changes to the node collection.
		public bool IsLocked => locked || (Graph && Graph.IsLocked);
		[HideInInspector] [SerializeField] bool locked;

		// Whether node in collection should be rendered.
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				foreach (Node node in Nodes) { if (!node.IsLocked) node.IsVisible = value; }
			}
		}
		[HideInInspector] [SerializeField] bool isVisible = true;

		// Unless locked, the initial node color is set from this
		// default color when applying parameters.
		public Color nodeDefaultColor;

		// Note: this assumes the node collection never changes its parent.
		public Graph Graph
		{
			get
			{
				if (graph != null) { return graph; }
				if (transform.parent == null) { return graph; }
				return graph = transform.parent.GetComponent<Graph>();
			}
		}
		[HideInInspector] [SerializeField] Graph graph;

		//TODO: Should have a serialized field and update it when edges are added or removed.
		public Node[] Nodes => GetComponentsInChildren<Node>();

		#region Used by Editor (methods not available in play mode)

#if UNITY_EDITOR

		public void ApplyParametersToNodes()
		{
			if (!IsLocked) { foreach (Node node in Nodes) { ApplyParametersToNode(node); } }
		}

		public void ApplyParametersToNode(Node node)
		{
			if (IsLocked || node.IsLocked) return;

			node.color = nodeDefaultColor;
			node.SharedMaterial.color = nodeDefaultColor;
			node.IsVisible = IsVisible;
		}
		
		// Add a new node. Optionally use a camera to determine placement.
		public GameObject AddNode(Camera placementCamera = null)
		{
			if (IsLocked || Graph == null || Graph.NodePrefab == null) { return null; }

			//var nodeObject = PrefabUtility.InstantiatePrefab(Graph.NodePrefab) as GameObject;
			GameObject nodeObject = Instantiate(Graph.NodePrefab, transform, true);

			if (nodeObject == null) { return null; }

			var node = nodeObject.GetComponent<Node>();

			if (node != null)
			{
				if (placementCamera != null)
				{
					Transform sceneViewCameraTransform = placementCamera.transform;
					Vector3 fromPosition = sceneViewCameraTransform.position;
					Vector3 direction = sceneViewCameraTransform.forward;
					node.CastToCollider(fromPosition, direction);
				}
				
				node.DropToSurface();
				node.GenerateNameFromPosition();
				ApplyParametersToNode(node);
			}

			return nodeObject;
		}

		public GameObject AddNode(VectorXZ location)
		{
			if (IsLocked || Graph == null || Graph.NodePrefab == null) { return null; }
			
			GameObject nodeObject = Instantiate(Graph.NodePrefab, transform, true);

			if (nodeObject == null) { return null; }

			var node = nodeObject.GetComponent<Node>();

			if (node != null)
			{
				nodeObject.transform.position = (Vector3)location;
				node.DropToSurface();
				node.GenerateNameFromPosition();
				ApplyParametersToNode(node);
			}

			return nodeObject;
		}

		// Reposition all nodes to be offset from the ground or obstacle.
		public void DropToSurface()
		{
			if (IsLocked) { return; }

			foreach (Node node in Nodes)
			{
				if (node.IsLocked) { continue; }

				node.DropToSurface();
			}
		}

		// Set all the node's names to match their position unless their name is locked.
		//TODO: This method is not necessary as names are updated automatically in Node.Update.
		public void GenerateNamesFromPosition()
		{
			if (IsLocked) { return; }

			foreach (Node node in Nodes)
			{
				if (node.IsLocked) { continue; }

				node.GenerateNameFromPosition();
			}
		}

		// Use capsule cast or sphere cast to find unobstructed connections between all
		// nodes. The range is limited by NodeCastMaximumDistance parameter.
		// Existing connections are removed before the cast.
		public void RaycastNodes()
		{
			if (IsLocked) { return; }

			foreach (Node fromNode in Nodes)
			{
				if (!fromNode.IsLocked) { fromNode.RaycastNeighbours(Nodes, true); }
			}
		}
		
#endif
		#endregion Used by Editor (methods not available in play mode)

		#region NODE COLLECTION EDITOR

#if UNITY_EDITOR

		[CustomEditor(typeof(NodeCollection))] [CanEditMultipleObjects]
		public class NodeCollectionEditor : UnityEditor.Editor
		{
			NodeCollection nodeCollection;
			bool nodesFoldoutStatus;

			void OnEnable()
			{
				nodeCollection = target as NodeCollection;
			}

			public override void OnInspectorGUI()
			{
				serializedObject.Update();

				bool isExternallyLocked = GraphEditingSupport.GetMultipleBoolInfo(
					serializedObject,
					"graph",
					"locked").anyTrue;

				bool isLocked
					= GraphEditingSupport.DetermineIfLocked(
						serializedObject,
						"node collection",
						"node collections",
						isExternallyLocked);

				if (isExternallyLocked)
				{
					GraphEditingSupport.HandleExternallyLocked(
					serializedObject,
					"node collection",
					"node collections",
					"graph",
					"graphs",
					"graph");
				}

				serializedObject.ApplyModifiedProperties();

				if (nodeCollection == null) { return; }

				Object[] filtered
					= Selection.GetFiltered(typeof(NodeCollection), SelectionMode.TopLevel);
				NodeCollection Converter(Object element) => (NodeCollection) element;
				NodeCollection[] selectedNodeCollections
					= System.Array.ConvertAll(filtered, Converter);

				if (selectedNodeCollections.Length != 1) { return; }

				if (!nodeCollection.IsLocked && nodeCollection.Nodes != null)
				{
					nodesFoldoutStatus
						= EditorGUILayout.Foldout(nodesFoldoutStatus, "Nodes");

					if (nodesFoldoutStatus)
					{
						EditorGUI.indentLevel += 1;

						EditorGUILayout.LabelField(
							"Count",
							nodeCollection.Nodes.Length.ToString());

						for (int i = 0; i < nodeCollection.Nodes.Length; i++)
						{
							UnityEngine.GUI.enabled = false;
							nodeCollection.Nodes[i]
								= (Node) EditorGUILayout.ObjectField(
									"Node " + i,
									nodeCollection.Nodes[i],
									typeof(Node),
									true);
							UnityEngine.GUI.enabled = true;
						}

						EditorGUI.indentLevel -= 1;
					}
				}

				if (nodeCollection.Graph)
				{
					if (GUILayout.Button("Edit graph"))
					{
						Selection.activeGameObject = nodeCollection.Graph.gameObject;
					}

					if (!nodeCollection.Graph.IsLocked &&
					    nodeCollection.Graph.EdgeCollectionObject &&
					    GUILayout.Button("Edit edge collection"))
					{
						Selection.activeGameObject = nodeCollection.Graph.EdgeCollectionObject;
					}
				}

				if (!nodeCollection.IsLocked)
				{
					if (nodeCollection.Graph != null &&
					    nodeCollection.Graph.NodePrefab != null &&
					    GUILayout.Button("Add a node"))
					{
						Selection.activeGameObject
							= nodeCollection.AddNode(SceneView.lastActiveSceneView.camera);
					}

					if (nodeCollection.Nodes != null && nodeCollection.Nodes.Length == 0) { return; }

					if (GUILayout.Button("Apply parameters to all nodes"))
					{
						nodeCollection.ApplyParametersToNodes();
					}

					if (nodeCollection.IsVisible)
					{
						if (GUILayout.Button("Hide nodes")) nodeCollection.IsVisible = false;
					}
					else
					{
						if (GUILayout.Button("Show nodes")) nodeCollection.IsVisible = true;
					}

					if (!nodeCollection.IsLocked &&
					    GUILayout.Button("Drop all nodes to surface"))
					{
						nodeCollection.DropToSurface();
					}
					
					// Only show if there are nodes
					if (!nodeCollection.IsLocked &&
					    nodeCollection != null)
					{
						var nodes = nodeCollection.Nodes;
						if (nodes != null)
						{
							if (GUILayout.Button("Remove all connections of all nodes"))
							{
								foreach (Node node in nodes)
								{
									node.RemoveAllConnections();
								}
							}

							if (GUILayout.Button("Remove all nodes"))
							{
								foreach (Node node in nodes)
								{
									node.RemoveNode();
								}
							}
						}
					}

					if (!nodeCollection.IsLocked &&
					    GUILayout.Button("Raycast connections for all nodes"))
					{
						nodeCollection.RaycastNodes();
					}
				}
			}
		}

#endif

		#endregion NODE COLLECTION EDITOR
	}
}