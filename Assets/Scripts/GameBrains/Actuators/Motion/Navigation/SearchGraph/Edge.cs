using GameBrains.Editor.Tools;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using UnityEditor;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	[ExecuteAlways]
	public class Edge : ExtendedMonoBehaviour
	{
		// Lock to prevent changes to the edge.
		public bool IsLocked => locked || (EdgeCollection && EdgeCollection.IsLocked);
		[HideInInspector] [SerializeField] bool locked;

		// Whether edge should be rendered.
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				if (!IsLocked)
				{
					isVisible = value;
					EdgeRenderer.enabled = value;
				}
			}
		}
		[HideInInspector] [SerializeField] bool isVisible = true;

		// Whether to prevent the edge's name from changing if its from or to nodes move.
		[SerializeField] bool nameLocked;

		// Unless locked, the initial edge color is set by the EdgeCollection
		// default color when applying parameters.
		public Color color;

		public Renderer EdgeRenderer
		{
			get
			{
				if (edgeRenderer == null) { edgeRenderer = GetComponentInChildren<Renderer>(); }
				return edgeRenderer;
			}
		}
		Renderer edgeRenderer;

		public Material SharedMaterial
		{
			get
			{
				if (sharedMaterial == null) { sharedMaterial = EdgeRenderer.sharedMaterial; }
				return sharedMaterial;
			}
		}
		// a place to save the shared material for possible restoration.
		Material sharedMaterial;
		// save and reuse this instance when making repeated changes.
		Material instanceMaterial;

		// Note: this assumes the edge collection never changes its parent.
		public Graph Graph
		{
			get
			{
				if (graph != null) { return graph; }
				if (EdgeCollection == null) { return graph; }
				if (EdgeCollection.transform.parent == null) { return graph; }
				return graph = EdgeCollection.transform.parent.GetComponent<Graph>();
			}
		}
		[HideInInspector] [SerializeField] Graph graph;

		// Note: this assumes the edge never changes its parent.
		public EdgeCollection EdgeCollection
		{
			get
			{
				if (edgeCollection != null) { return edgeCollection; }
				if (transform.parent == null) { return edgeCollection; }
				return edgeCollection = transform.parent.GetComponent<EdgeCollection>();
			}
		}
		[HideInInspector] [SerializeField] EdgeCollection edgeCollection;

		[SerializeField] Node fromNode;
		public Node FromNode { get => fromNode; set => fromNode = value; }

		[SerializeField] Node toNode;
		public Node ToNode { get => toNode; set => toNode = value; }

		[SerializeField] float cost;
		public float Cost { get => cost; set => cost = value; }
		
		public VectorXYZ Position => transform.position;
		public VectorXZ Location => (VectorXZ)transform.position;
		
		public override string ToString() { return name; }

		public override void Update()
		{
			base.Update();
			
			// TODO: use some kind of set dirty to avoid calling on every update
			if (!EditorTools.PrefabMode)
			{
				CalculateCost();
				UpdatePosition();
				UpdateRotation();
				UpdateScale();

				// Update name to match changes in cost when to or from nodes moved.
				GenerateNameFromNodes();
			}
		}
		
		// The cost is automatically calculated based on the distance between the from and to nodes.
		public void CalculateCost()
		{
			if (IsLocked) return;

			Cost = FromNode && ToNode
				? Vector3.Distance(FromNode.Position, ToNode.Position)
				: 0;
		}

		// The position is automatically calculated based on the position of the from and to nodes.
		public void UpdatePosition()
		{
			if (IsLocked || FromNode == null || ToNode == null) { return; }

			transform.position = (FromNode.Position + ToNode.Position) / 2;
		}

		// The position is automatically calculated based on the position of the to node.
		public void UpdateRotation()
		{
			if (IsLocked || FromNode == null || ToNode == null) { return; }

			transform.LookAt(ToNode.Position);
			transform.Rotate(Vector3.right, 90);
		}

		// The scale is automatically calculated based on the distance
		// between the from and to nodes.
		public void UpdateScale()
		{
			if (IsLocked || FromNode == null || ToNode == null) { return; }

			Vector3 scale = transform.localScale;
			scale.y = Vector3.Distance(FromNode.Position, ToNode.Position) / 2;
			transform.localScale = scale;
		}

		// Automatically set the name to match the edges' from and to nodes and cost
		// unless the name is locked.
		public void GenerateNameFromNodes()
		{
			if (IsLocked || nameLocked) { return; }

			name =
				"Edge (" +
				(FromNode == null ? "NONE" : FromNode.name) +
				" --[" +
				Cost.ToString("F1") +
				"]--> " +
				(ToNode == null ? "NONE" : ToNode.name) +
				")";
		}

		#region Used by Editor (methods not available in play mode)

#if UNITY_EDITOR
		
		public void ResetColorToCollectionColor()
		{
			if (EditorTools.PrefabMode || IsLocked) { return; }

			EdgeRenderer.sharedMaterial = SharedMaterial;
			color = SharedMaterial.color;
			instanceMaterial = null;
		}

		public void RemoveEdge()
		{
			if (IsLocked) { return; }

			fromNode.RemoveConnection(toNode);
		}

#endif
		#endregion Used by Editor (methods not available in play mode)

		#region EDGE EDITOR

#if UNITY_EDITOR

		[CustomEditor(typeof(Edge))] [CanEditMultipleObjects]
		public class EdgeEditor : UnityEditor.Editor
		{
			Edge edge;

			void OnEnable()
			{
				edge = target as Edge;
			}
			public override void OnInspectorGUI()
			{
				serializedObject.Update();

				bool isLockedByGraph = GraphEditingSupport.GetMultipleBoolInfo(
					serializedObject,
					"graph",
					"locked").anyTrue;

				bool isLockedByEdgeCollection = GraphEditingSupport.GetMultipleBoolInfo(
					serializedObject,
					"edgeCollection",
					"locked").anyTrue;

				bool isLocked
					= GraphEditingSupport.DetermineIfLocked(
						serializedObject,
						"edge",
						"edges",
						isLockedByEdgeCollection || isLockedByGraph);

				if (isLockedByGraph)
				{
					GraphEditingSupport.HandleExternallyLocked(
						serializedObject,
						"edge",
						"edges",
						"graph",
						"graphs",
						"graph");
				}
				else if (isLockedByEdgeCollection)
				{
					GraphEditingSupport.HandleExternallyLocked(
						serializedObject,
						"edge",
						"edges",
						"edge collection",
						"edges collections",
						"edgeCollection");
				}

				serializedObject.ApplyModifiedProperties();

				if (edge == null) { return; }

				Object[] filtered = Selection.GetFiltered(typeof(Edge), SelectionMode.TopLevel);
				Edge Converter(Object element) => (Edge) element;
				Edge[] selectedEdges = System.Array.ConvertAll(filtered, Converter);

				if (selectedEdges.Length == 1)
				{
					if (edge.Graph != null)
					{
						if (GUILayout.Button("Edit graph"))
						{
							Selection.activeGameObject = edge.Graph.gameObject;
						}

						if (edge.Graph.NodeCollectionObject &&
						    GUILayout.Button("Edit node collection"))
						{
							Selection.activeGameObject = edge.Graph.NodeCollectionObject;
						}

						if (!edge.Graph.IsLocked &&
						    edge.Graph.EdgeCollectionObject &&
						    GUILayout.Button("Edit edge collection"))
						{
							Selection.activeGameObject = edge.Graph.EdgeCollectionObject;
						}
					}

					if (!edge.IsLocked)
					{
						if (edge.IsVisible)
						{
							if (GUILayout.Button("Hide edge")) edge.IsVisible = false;
						}
						else
						{
							if (GUILayout.Button("Show edge")) edge.IsVisible = true;
						}

						if (!EditorTools.PrefabMode
						    && selectedEdges.Length == 1
						    && edge.gameObject.scene.rootCount != 0 // not a prefab
						    && edge.EdgeRenderer != null
						    && edge.EdgeRenderer.sharedMaterial != null
						    && edge.EdgeRenderer.sharedMaterial.color != edge.color)
						{
							edge.sharedMaterial = edge.EdgeRenderer.sharedMaterial;
							if (edge.instanceMaterial == null)
							{
								edge.instanceMaterial = new Material(edge.EdgeRenderer.sharedMaterial);
							}
							edge.instanceMaterial.color = edge.color;
							edge.EdgeRenderer.material = edge.instanceMaterial;
						}

						if (!EditorTools.PrefabMode
							&& selectedEdges.Length == 1
							&& edge.EdgeRenderer != null
							&& edge.EdgeRenderer.sharedMaterial != null
						    && edge.EdgeRenderer.sharedMaterial == edge.instanceMaterial
						    && GUILayout.Button("Reset selected edge's color."))
						{
							edge.ResetColorToCollectionColor();
						}

						if (GUILayout.Button("Remove selected edge"))
						{
							edge.RemoveEdge();
							edge = null;
						}
					}
				}
			}
		}

#endif

		#endregion EDGE EDITOR
	}
}