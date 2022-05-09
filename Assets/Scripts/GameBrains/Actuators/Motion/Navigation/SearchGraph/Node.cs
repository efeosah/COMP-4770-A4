using System.Collections.Generic;
using GameBrains.Editor.Tools;
using GameBrains.Extensions.DictionaryExtensions;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using UnityEditor;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	[ExecuteAlways]
	public class Node : ExtendedMonoBehaviour
	{
		// Lock to prevent changes to the node.
		public bool IsLocked => locked || (NodeCollection != null && NodeCollection.IsLocked);
		[HideInInspector] [SerializeField] bool locked;

		// Whether node should be rendered.
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				if (!IsLocked)
				{
					isVisible = value;
					NodeRenderer.enabled = value;
				}
			}
		}
		[HideInInspector] [SerializeField] bool isVisible = true;

		// Whether to prevent the node's name changing when the node is moved.
		[SerializeField] bool nameLocked;

		// Unless locked, the initial node color is set by the NodeCollection
		// default color when applying parameters.
		public Color color;

		public Renderer NodeRenderer
		{
			get
			{
				if (nodeRenderer == null)
				{
					nodeRenderer = GetComponentInChildren<Renderer>();
				}
				return nodeRenderer;
			}
		}
		Renderer nodeRenderer;

		public Material SharedMaterial
		{
			get
			{
				if (sharedMaterial == null) { sharedMaterial = NodeRenderer.sharedMaterial; }
				return sharedMaterial;
			}
		}
		// a place to save the shared material for possible restoration.
		Material sharedMaterial;
		// save and reuse this instance when making repeated changes.
		Material instanceMaterial;

		// Note: this assumes the node collection never changes its parent.
		public Graph Graph
		{
			get
			{
				if (graph != null) { return graph; }
				if (NodeCollection == null) return graph;
				if (NodeCollection.transform.parent == null) { return graph; }
				return graph = NodeCollection.transform.parent.GetComponent<Graph>();
			}
		}
		[HideInInspector] [SerializeField] Graph graph;

		// Note: this assumes the node never changes its parent.
		public NodeCollection NodeCollection
		{
			get
			{
				if (nodeCollection != null) { return nodeCollection; }
				if (transform.parent == null) { return nodeCollection; }
				return nodeCollection = transform.parent.GetComponent<NodeCollection>();
			}
		}
		[HideInInspector] [SerializeField] NodeCollection nodeCollection;

		public VectorXYZ Position => transform.position;
		public VectorXZ Location => (VectorXZ)transform.position;

		public NodeEdgeSerializableDictionary outEdges = new NodeEdgeSerializableDictionary();

		public override void Update()
		{
			base.Update();
			
			// TODO: use some kind of set dirty to avoid calling on every update
			if (!EditorTools.PrefabMode) { GenerateNameFromPosition(); }
		}
		
		// Automatically set the name to match the node's position unless the name is locked.
		public void GenerateNameFromPosition()
		{
			if (EditorTools.PrefabMode || IsLocked || nameLocked) return;

			Vector3 position = transform.position;
			name =
				"Node (" +
				position.x.ToString("F1") +
				", " +
				position.y.ToString("F1") +
				", " +
				position.z.ToString("F1") +
				")";
		}
		
		#region Used by Editor (methods not available in play mode)
		
#if UNITY_EDITOR
		
		// Whether to use capsule or sphere cast to check for obstructions when connecting nodes.
		public bool UseCapsuleCast => Parameters.Instance.NodeUseCapsuleCast;

		// Maximum distance to cast to check for obstructions when connecting nodes.
		public float NodeCastMaximumDistance => Parameters.Instance.NodeCastMaximumDistance;

		// Radius to use when casting for obstructions.
		public float NodeCastPathRadius => Parameters.Instance.NodeCastPathRadius;

		// Offset from surface to place node centers.
		public float NodeSurfaceOffset => Parameters.Instance.NodeSurfaceOffset;
		
		// Replace the node's shared material with the common one used by nodes in its
		// node collection. If the node is separately colored, it uses an instance of
		// the original material.
		public void ResetColorToCollectionColor()
		{
			if (EditorTools.PrefabMode || IsLocked) return;

			NodeRenderer.sharedMaterial = SharedMaterial;
			color = SharedMaterial.color;
			instanceMaterial = null;
		}

		// Removes the node and all its edges.
		// DestroyImmediate is used in edit mode and the node should not be
		// referenced after deletion.
		public void RemoveNode()
		{
			if (IsLocked) { return; }

			if (Graph != null && Graph.EdgeCollection != null)
			{
				var edges = Graph.EdgeCollection.Edges;
				for (int i = 0; i < edges.Length; i++)
				{
					var edge = edges[i];
					if (edge.ToNode == this || edge.FromNode == this)
					{
						if (edge.ToNode == this)
						{
							edge.FromNode.RemoveConnection(this);
						}
						else if (edge.FromNode == this)
						{
							RemoveConnection(edge.ToNode);
						}
					}
				}
			}

			DestroyImmediate(gameObject);
		}

		// Add a new node with a connection from this node.
		// If the a camera is provided, it is used in the placement of the new node.
		public GameObject AddConnectedNode(bool oneWay, Camera placementCamera = null)
		{
			if (IsLocked || NodeCollection == null || Graph == null || Graph.NodePrefab == null)
			{
				return null;
			}

			GameObject connectedNodeObject = Instantiate(Graph.NodePrefab, transform.parent, true);

			if (connectedNodeObject == null) { return null; }

			var connectedNode = connectedNodeObject.GetComponent<Node>();

			if (connectedNode != null)
			{
				if (placementCamera != null)
				{
					Transform cameraTransform = placementCamera.transform;
					Vector3 position = cameraTransform.position;
					Vector3 direction = cameraTransform.forward;
					connectedNode.CastToCollider(position, direction);
				}
				
				connectedNode.DropToSurface();
				connectedNode.GenerateNameFromPosition();
				NodeCollection.ApplyParametersToNode(connectedNode);
			}

			connectedNodeObject.transform.parent = transform.parent;

			if (!oneWay) { connectedNode.AddConnection(this); }

			AddConnection(connectedNode);

			return connectedNodeObject;
		}

		// Adds a one-way connection from this node to the toNode.
		public void AddConnection(Node toNode)
		{
			if (IsLocked || Graph == null || Graph.EdgeCollection == null) { return; }

			if (toNode == this || toNode.Graph != Graph ) { return; }

			GameObject edgeObject = Graph.EdgeCollection.AddEdge(this, toNode);

			if (edgeObject == null) { return; }
			
			var edge = edgeObject.GetComponent<Edge>();
			
			// This assumes one connection from a node to another node.
			outEdges[toNode] = edge;
		}

		// Does a sphere cast to find a surface / obstacle on which to place a new node
		public void CastToCollider(
			Vector3 fromPosition,
			Vector3 direction,
			float maxDistance = 0)
		{
			if (IsLocked) { return; }

			RaycastHit hitInfo;

			var hit = maxDistance > 0f
				? SphereCastToCollider(fromPosition, direction, out hitInfo, maxDistance)
				: SphereCastToCollider(fromPosition, direction, out hitInfo);

			if (hit) { transform.position = hitInfo.point + Vector3.up * NodeSurfaceOffset; }
		}

		// Sphere cast for obstacles (used in new node placement)
		bool SphereCastToCollider(
			Vector3 origin,
			Vector3 direction,
			out RaycastHit hitInfo,
			float maxDistance = float.MaxValue
			)
		{
			return Physics.SphereCast(
				origin,
				Parameters.Instance.NodeRadius,
				direction,
				out hitInfo,
				maxDistance,
				Parameters.Instance.ObstacleLayerMask);
		}

		// Add one-way connections between the given nodes. If the from node of
		// a connection is locked, it is not added.
		//TODO: should the distance between nodes and obstacles be considered.
		// See RaycastNeighbours method.
		public static void ConnectNodes(Node[] nodes, bool oneWay)
		{
			for (var i = 0; i < nodes.Length; i++)
			{
				Node nodeFrom = nodes[i];
				for (var j = i+1; j < nodes.Length; j++)
				{
					Node nodeTo = nodes[j];
					if (nodeFrom.IsLocked) { continue; }

					if (!oneWay) { nodeTo.AddConnection(nodeFrom); }

					nodeFrom.AddConnection(nodeTo);
				}
			}
		}

		// Cycles between no connection, one-way connection, one-way reverse connection,
		// and two-way connections.
		//TODO: should the distance between nodes and obstacles be considered.
		// See RaycastNeighbours method.
		public static void CycleConnection(Node fromNode, Node toNode)
		{
			if (fromNode.IsLocked || toNode.IsLocked) { return; }

			bool oneConnectedToTwo = fromNode.IsConnectedTo(toNode);
			bool twoConnectedToOne = toNode.IsConnectedTo(fromNode);

			if (oneConnectedToTwo && twoConnectedToOne)
			{
				toNode.RemoveConnection(fromNode);
			}
			else if (oneConnectedToTwo)
			{
				toNode.AddConnection(fromNode);
				fromNode.RemoveConnection(toNode);
			}
			else if (twoConnectedToOne)
			{
				toNode.RemoveConnection(fromNode);
			}
			else
			{
				fromNode.AddConnection(toNode);
				toNode.AddConnection(fromNode);
			}
		}

		// Remove all connections between the given nodes. If the from node of
		// a connection is locked, it is not removed.
		public static void DisconnectNodes(Node[] nodes)
		{
			for (var from = 0; from < nodes.Length; from++)
			{
				Node nodeFrom = nodes[from];
				for (var to = from+1; to < nodes.Length; to++)
				{
					Node nodeTo = nodes[to];
					if (nodeFrom.IsLocked || nodeFrom == nodeTo) { continue; }

					nodeFrom.RemoveConnection(nodeTo);
				}
			}
		}

		// Reposition this node to be offset from the ground or obstacle.
		public void DropToSurface()
		{
			if (IsLocked) { return; }

			Vector3 dropFromPosition = transform.position;
			dropFromPosition.y += Parameters.Instance.NodeDropFromHeightOffset;

			CastToCollider(dropFromPosition, Vector3.down, 0f);
		}

		// Check if this node has a one-way connection to toNode.
		public bool IsConnectedTo(Node toNode) { return outEdges.ContainsKey(toNode); }

		// Use capsule cast or sphere cast to find unobstructed connections between the given
		// nodes. The range is limited by NodeCastMaximumDistance parameter. Optionally,
		// existing connections can be removed before the cast.
		public void RaycastNeighbours(Node[] potentialNeighbours, bool clearConnections)
		{
			if (IsLocked) { return; }

			if (clearConnections) { RemoveAllConnections(); }

			if (potentialNeighbours == null) { return; }

			foreach (Node neighbour in potentialNeighbours)
			{
				if (neighbour == null || neighbour == this) { continue; }

				Vector3 position = transform.position;
				Vector3 direction = neighbour.transform.position - position;

				// If not too far away and no intervening obstacles, add connection(s).
				if (direction.magnitude <= NodeCastMaximumDistance &&
				    (UseCapsuleCast && !CapsuleCastForObstacles(position, direction) ||
				     !UseCapsuleCast && !SphereCastForObstacles(position, direction)))
				{
					AddConnection(neighbour);
				}
			}
		}

		// Use capsule cast to detect obstructions. This is well-suited for capsule-sized
		// characters. The radius of the capsule is given by the NodeCastPathRadius parameter.
		bool CapsuleCastForObstacles(Vector3 origin, Vector3 direction)
		{
			return Physics.CapsuleCast(
				origin - Vector3.up / 2,
				origin + Vector3.up / 2,
				NodeCastPathRadius,
				direction,
				direction.magnitude,
				Parameters.Instance.ObstacleLayerMask);
		}

		// Use sphere cast to detect obstructions. This is best for non-capsule-sized
		// characters. The radius of the sphere is given by the NodeCastPathRadius parameter.
		bool SphereCastForObstacles(Vector3 origin, Vector3 direction)
		{
			return Physics.SphereCast(
				new Ray(origin, direction),
				NodeCastPathRadius,
				direction.magnitude,
				Parameters.Instance.ObstacleLayerMask);
		}

		// Remove all the outgoing connections of this node. This removes edges whether locked or not.
		public void RemoveAllConnections()
		{
			if (IsLocked) { return; }

			foreach (Edge edge in outEdges.Values)
			{
				if (edge != null) { DestroyImmediate(edge.gameObject); }
			}

			outEdges.Clear();
		}

		// Remove the connection from this node to toNode (if present). This removes the edge
		// whether locked or not.
		public void RemoveConnection(Node toNode)
		{
			if (IsLocked) { return; }

			if (outEdges.ContainsKey(toNode))
			{
				Edge edge = outEdges[toNode];
				outEdges.Remove(toNode);
				DestroyImmediate(edge.gameObject);
			}
		}
#endif

		#endregion Used by Editor (methods not available in play mode)

		#region NODE EDITOR

#if UNITY_EDITOR

		[CustomEditor(typeof(Node))] [CanEditMultipleObjects]
		public class NodeEditor : UnityEditor.Editor
		{
			Node node;

			void OnEnable()
			{
				node = target as Node;
			}

			public override void OnInspectorGUI()
			{
				serializedObject.Update();

				bool isLockedByGraph = GraphEditingSupport.GetMultipleBoolInfo(
					serializedObject,
					"graph",
					"locked").anyTrue;

				bool isLockedByNodeCollection = GraphEditingSupport.GetMultipleBoolInfo(
					serializedObject,
					"nodeCollection",
					"locked").anyTrue;

				bool isLocked
					= GraphEditingSupport.DetermineIfLocked(
						serializedObject,
						"node",
						"nodes",
						isLockedByNodeCollection || isLockedByGraph);

				if (isLockedByGraph)
				{
					GraphEditingSupport.HandleExternallyLocked(
						serializedObject,
						"node",
						"nodes",
						"graph",
						"graphs",
						"graph");
				}
				else if (isLockedByNodeCollection)
				{
					GraphEditingSupport.HandleExternallyLocked(
						serializedObject,
						"node",
						"nodes",
						"node collection",
						"node collections",
						"nodeCollection");
				}

				serializedObject.ApplyModifiedProperties();

				if (node == null) { return; }

				Object[] filtered = Selection.GetFiltered(typeof(Node), SelectionMode.TopLevel);
				Node Converter(Object element) => (Node) element;
				Node[] selectedNodes = System.Array.ConvertAll(filtered, Converter);

				if (selectedNodes.Length == 1 && node.Graph != null)
				{
					if (GUILayout.Button("Edit graph"))
					{
						Selection.activeGameObject = node.Graph.gameObject;
					}

					if (node.Graph.NodeCollectionObject &&
					    GUILayout.Button("Edit node collection"))
					{
						Selection.activeGameObject = node.Graph.NodeCollectionObject;
					}

					if (!node.Graph.IsLocked &&
					    node.Graph.EdgeCollectionObject &&
					    GUILayout.Button("Edit edge collection"))
					{
						Selection.activeGameObject = node.Graph.EdgeCollectionObject;
					}
				}

				if (!node.IsLocked)
				{
					if (node.IsVisible)
					{
						if (GUILayout.Button("Hide node")) node.IsVisible = false;
					}
					else
					{
						if (GUILayout.Button("Show node")) node.IsVisible = true;
					}

					if (!EditorTools.PrefabMode 
					    && selectedNodes.Length == 1
					    && node.gameObject.scene.rootCount != 0 // not a prefab
					    && node.NodeRenderer != null
					    && node.NodeRenderer.sharedMaterial != null
					    && node.NodeRenderer.sharedMaterial.color != node.color)
					{
						node.sharedMaterial = node.NodeRenderer.sharedMaterial;
						if (node.instanceMaterial == null)
						{
							node.instanceMaterial = new Material(node.NodeRenderer.sharedMaterial);
						}
						node.instanceMaterial.color = node.color;
						node.NodeRenderer.material = node.instanceMaterial;
					}

					if (!EditorTools.PrefabMode
					    && selectedNodes.Length == 1
					    && node.NodeRenderer != null
					    && node.NodeRenderer.sharedMaterial != null
					    && node.NodeRenderer.sharedMaterial == node.instanceMaterial
					    && GUILayout.Button("Reset selected node's color."))
					{
						node.ResetColorToCollectionColor();
					}

					// Use a cast to a collider to move the selected node to the surface level
					// plus the surface offset parameter.
					// Only show if a single node is selected.
					if (selectedNodes.Length == 1 &&
					    GUILayout.Button("Drop selected node to surface"))
					{
						node.DropToSurface();
					}

					// Create a new node and add a one-way connection from the selected node
					// to the new node.
					// Only show if a single node is selected.
					if (selectedNodes.Length == 1 &&
					    GUILayout.Button("Connect from selected node to new node"))
					{
						Selection.activeGameObject
							= node.AddConnectedNode(
								true,
								SceneView.lastActiveSceneView.camera);
					}

					// Create a new node and add a two-way connection with the selected node.
					// Only show if a single node is selected.
					if (selectedNodes.Length == 1 &&
					    GUILayout.Button("Bi-connect selected node with new node"))
					{
						Selection.activeGameObject
							= node.AddConnectedNode(
								false,
								SceneView.lastActiveSceneView.camera);
					}

					// Add two way connections between selected nodes.
					// Only show if multiple nodes selected.
					if (selectedNodes.Length > 1 &&
					    !CompletelyConnected(selectedNodes) &&
					    GUILayout.Button("Bi-Connect selected nodes"))
					{
						ConnectNodes(selectedNodes, false);
					}

					// Add one way connections between selected nodes.
					// Only show if multiple nodes selected.
					if (selectedNodes.Length > 1 &&
					    !CompletelyConnected(selectedNodes) &&
					    GUILayout.Button("Connect selected nodes"))
					{
						ConnectNodes(selectedNodes, true);
					}

					// Only show if a single node is selected and it has neighbours.
					if (selectedNodes.Length == 1 &&
					    node.outEdges.Count > 0 &&
					    GUILayout.Button("Remove all connections of selected node"))
					{
						node.RemoveAllConnections();
					}

					// Remove connections between selected nodes.
					// Only show if multiple nodes selected and they are connected.
					if (selectedNodes.Length > 1 &&
					    HasAConnection(selectedNodes) &&
					    GUILayout.Button("Disconnect selected nodes"))
					{
						DisconnectNodes(selectedNodes);
					}

					if (selectedNodes.Length == 2)
					{
						if (GUILayout.Button("Cycle selected nodes' connections"))
						{
							Node fromNode = selectedNodes[0];
							Node toNode = selectedNodes[1];
							CycleConnection(fromNode, toNode);
						}
					}

					// Remove selected node and its connections.
					// Only show if a single node is selected.
					if (selectedNodes.Length == 1 && GUILayout.Button("Remove selected node"))
					{
						node.RemoveNode();
						node = null;
					}
				}
			}

			public bool HasAConnection(Node[] nodes)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					for (int j = i+1; j < nodes.Length; j++)
					{
						if (nodes[i].IsConnectedTo(nodes[j])) { return true; }
					}
				}

				return false;
			}

			public bool CompletelyConnected(Node[] nodes)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					for (int j = i+1; j < nodes.Length; j++)
					{
						if (!nodes[i].IsConnectedTo(nodes[j])) { return false; }
					}
				}

				return true;
			}
		}

#endif

		#endregion NODE EDITOR
	}
}