using GameBrains.Extensions.MonoBehaviours;
using UnityEditor;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	public class EdgeCollection : ExtendedMonoBehaviour
	{
		// Lock to prevent changes to the edge collection.
		public bool IsLocked => locked || (Graph && Graph.IsLocked);
		[HideInInspector] [SerializeField] bool locked;

		// Whether edges in collection should be rendered.
		public bool IsVisible
		{
			get => isVisible;
			set
			{
				isVisible = value;
				foreach (Edge edge in Edges) { if (!edge.IsLocked) edge.IsVisible = value; }
			}
		}
		[HideInInspector] [SerializeField] bool isVisible = true;

		// Unless locked, the initial edge color is set from this
		// default color when applying parameters.
		public Color edgeDefaultColor;

		// Note: this assumes the edge collection never changes its parent.
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
		public Edge[] Edges => GetComponentsInChildren<Edge>();
		
		#region Used by Editor (methods not available in play mode)
		
#if UNITY_EDITOR
		
		public void ApplyParametersToEdges()
		{
			if (!IsLocked) { foreach (Edge edge in Edges) { ApplyParametersToEdge(edge); } }
		}

		void ApplyParametersToEdge(Edge edge)
		{
			if (IsLocked || edge.IsLocked) { return; }

			edge.color = edgeDefaultColor;
			edge.SharedMaterial.color = edgeDefaultColor;
			edge.IsVisible = IsVisible;
		}
		
#endif
		#endregion Used by Editor (methods not available in play mode)

		#region EDGE COLLECTION EDITOR

#if UNITY_EDITOR

		[CustomEditor(typeof(EdgeCollection))] [CanEditMultipleObjects]
		public class EdgeCollectionEditor : UnityEditor.Editor
		{
			EdgeCollection edgeCollection;
			bool edgesFoldoutStatus;

			void OnEnable()
			{
				edgeCollection = target as EdgeCollection;
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
						"edge collection",
						"edge collections",
						isExternallyLocked);

				if (isExternallyLocked)
				{
					GraphEditingSupport.HandleExternallyLocked(
						serializedObject,
						"edge collection",
						"edge collections",
						"graph",
						"graphs",
						"graph");
				}

				serializedObject.ApplyModifiedProperties();

				if (edgeCollection == null) { return; }

				Object[] filtered
					= Selection.GetFiltered(typeof(EdgeCollection), SelectionMode.TopLevel);
				EdgeCollection Converter(Object element) => (EdgeCollection) element;
				EdgeCollection[] selectedEdgeCollections
					= System.Array.ConvertAll(filtered, Converter);

				if (selectedEdgeCollections.Length != 1) { return; }

				if (!edgeCollection.IsLocked && edgeCollection.Edges != null)
				{
					edgesFoldoutStatus
						= EditorGUILayout.Foldout(edgesFoldoutStatus, "Edges");

					if (edgesFoldoutStatus)
					{
						EditorGUI.indentLevel += 1;

						EditorGUILayout.LabelField(
							"Count",
							edgeCollection.Edges.Length.ToString());

						for (int i = 0; i < edgeCollection.Edges.Length; i++)
						{
							UnityEngine.GUI.enabled = false;
							edgeCollection.Edges[i]
								= (Edge) EditorGUILayout.ObjectField(
									"Edge " + i,
									edgeCollection.Edges[i],
									typeof(Edge),
									true);
							UnityEngine.GUI.enabled = true;
						}

						EditorGUI.indentLevel -= 1;
					}
				}

				if (edgeCollection.Graph != null)
				{
					if (GUILayout.Button("Edit graph"))
					{
						Selection.activeGameObject = edgeCollection.Graph.gameObject;
					}

					if (!edgeCollection.Graph.IsLocked &&
					    edgeCollection.Graph.NodeCollectionObject != null &&
					    GUILayout.Button("Edit node collection"))
					{
						Selection.activeGameObject = edgeCollection.Graph.NodeCollectionObject;
					}
				}

				if (!edgeCollection.IsLocked)
				{
					var edges = edgeCollection.Edges;
					if (edges != null && edges.Length != 0)
					{
						if (GUILayout.Button("Apply parameters to all edges"))
						{
							edgeCollection.ApplyParametersToEdges();
						}

						if (GUILayout.Button("Remove all edges"))
						{
							foreach (var edge in edges)
							{
								edge.RemoveEdge();
							}
						}
						
						if (edgeCollection.IsVisible)
						{
							if (GUILayout.Button("Hide edges")) edgeCollection.IsVisible = false;
						}
						else
						{
							if (GUILayout.Button("Show edges")) edgeCollection.IsVisible = true;
						}
					}
				}
			}
		}

		public GameObject AddEdge(Node fromNode, Node toNode)
		{
			if (IsLocked
			    || fromNode.Graph == null
			    || fromNode.Graph != toNode.Graph
			    || fromNode.Graph.EdgePrefab == null)
			{
				return null;
			}
			
			//var edgeObject = PrefabUtility.InstantiatePrefab(fromNode.Graph.EdgePrefab) as GameObject;
			GameObject edgeObject = Instantiate(fromNode.Graph.EdgePrefab);

			if (edgeObject != null)
			{
				var edge = edgeObject.GetComponent<Edge>();

				if (edge != null)
				{
					edge.FromNode = fromNode;
					edge.ToNode = toNode;
					edge.CalculateCost();
					edge.GenerateNameFromNodes();
					ApplyParametersToEdge(edge);
					edgeObject.transform.parent = transform;
				}
				else
				{
					Destroy(edgeObject);
					edgeObject = null;
				}
			}

			return edgeObject;
		}

#endif

		#endregion EDGE COLLECTION EDITOR
	}
}