using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.GameManagement;
using GameBrains.Visualization;
using UnityEditor;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.SearchGraph
{
	public class Graph : ExtendedMonoBehaviour
	{
		#region Members and Properties
		
		// Lock to prevent changes to the graph.
		public bool IsLocked { get => locked; set => locked = value; }
		[HideInInspector] [SerializeField] bool locked;

		// Whether nodes and edges in the graph should be rendered.
		public bool IsVisible
		{
			get
			{
				isVisible
					= (NodeCollection == null || NodeCollection.IsVisible)
					  && (EdgeCollection == null || EdgeCollection.IsVisible);
				return isVisible;
			}
			set
			{
				isVisible = value;
				if (NodeCollection != null && !NodeCollection.IsLocked)
				{
					NodeCollection.IsVisible = value;
				}

				if (EdgeCollection != null && !EdgeCollection.IsLocked)
				{
					EdgeCollection.IsVisible = value;
				}
			}
		}
		[HideInInspector] [SerializeField] bool isVisible = true;

		// The prefab to use to create new nodes.
		public GameObject NodePrefab
		{
			get
			{
				if (!nodePrefab)
				{
					nodePrefab = Resources.Load<GameObject>(Parameters.Instance.NodePrefabPath);
				}
				return nodePrefab;
			}
		}
		//[HideInInspector] [SerializeField] 
		GameObject nodePrefab;

		// The prefab to use to create new edges.
		public GameObject EdgePrefab
		{
			get
			{
				if (!edgePrefab)
				{
					edgePrefab = Resources.Load<GameObject>(Parameters.Instance.EdgePrefabPath);
				}
				return edgePrefab;
			}
		}
		//[HideInInspector] [SerializeField] 
		GameObject edgePrefab;
		
		public GameObject NodeCollectionObject => nodeCollectionObject;
		[HideInInspector] [SerializeField] GameObject nodeCollectionObject;

		public GameObject EdgeCollectionObject => edgeCollectionObject;
		[HideInInspector] [SerializeField] GameObject edgeCollectionObject;
		public NodeCollection NodeCollection => nodeCollection;
		[HideInInspector] [SerializeField] NodeCollection nodeCollection;

		public EdgeCollection EdgeCollection => edgeCollection;
		[HideInInspector] [SerializeField] EdgeCollection edgeCollection;

		#endregion Members and Properties

		#region GRAPH EDITOR

#if UNITY_EDITOR

		[CustomEditor(typeof(Graph))] [CanEditMultipleObjects]
		public class GraphEditor : UnityEditor.Editor
		{
			Graph graph;
			SerializedProperty nodeCollectionObjectProperty;
			SerializedProperty edgeCollectionObjectProperty;
			SerializedProperty nodeCollectionProperty;
			SerializedProperty edgeCollectionProperty;

			void OnEnable()
			{
				graph = target as Graph;
				nodeCollectionObjectProperty =
					serializedObject.FindProperty("nodeCollectionObject");
				edgeCollectionObjectProperty =
					serializedObject.FindProperty("edgeCollectionObject");
				nodeCollectionProperty =
					serializedObject.FindProperty("nodeCollection");
				edgeCollectionProperty =
					serializedObject.FindProperty("edgeCollection");
			}

			public override void OnInspectorGUI()
			{
				serializedObject.Update();

				bool isLocked
					= GraphEditingSupport.DetermineIfLocked(
						serializedObject,
						"graph",
						"graphs",
						false);

				if (graph == null) { return; }

				Object[] filtered
					= Selection.GetFiltered(typeof(Graph), SelectionMode.TopLevel);
				Graph Converter(Object element) => (Graph) element;
				Graph[] selectedGraphs = System.Array.ConvertAll(filtered, Converter);

				if (isLocked || selectedGraphs.Length != 1) { return; }

				if (graph.nodePrefab == null) { graph.nodePrefab = graph.NodePrefab; }
				if (graph.edgePrefab == null) { graph.edgePrefab = graph.EdgePrefab; }

				// No node or edge collection
				if (graph.NodeCollection == null &&
				    graph.EdgeCollection == null &&
				    GUILayout.Button("Create new node and edge collections"))
				{
					CreateNewNodeCollection();
					CreateNewEdgeCollection();
				}

				// Edge collection and no node collection
				if (graph.NodeCollection == null &&
				    graph.EdgeCollection != null &&
				    GUILayout.Button("Create a new node collection"))
				{
					CreateNewNodeCollection();
				}

				// Node collection and no edge collection
				if (graph.NodeCollection != null &&
				    graph.EdgeCollection == null &&
				    GUILayout.Button("Create a new edge collection"))
				{
					CreateNewEdgeCollection();
				}

				// Node collection
				if (graph.nodeCollection != null &&
				    GUILayout.Button("Edit node collection"))
				{
					Selection.activeGameObject
						= nodeCollectionObjectProperty.objectReferenceValue as GameObject;
				}

				// Edge collection
				if (graph.edgeCollection != null &&
				    GUILayout.Button("Edit edge collection"))
				{
					Selection.activeGameObject
						= edgeCollectionObjectProperty.objectReferenceValue as GameObject;
				}

				// node collection or edge collection
				if (graph.NodeCollection != null || graph.EdgeCollection != null)
				{
					// non-empty node collection or non-empty edge collection
					if ((graph.NodeCollection != null && graph.NodeCollection.Nodes.Length != 0)
					    || (graph.EdgeCollection != null && graph.EdgeCollection.Edges.Length != 0))
					{
						if (graph.IsVisible)
						{
							if (GUILayout.Button("Hide graph"))
							{
								if (graph.NodeCollection != null)
								{
									graph.NodeCollection.IsVisible = false;
								}

								if (graph.EdgeCollection != null)
								{
									graph.EdgeCollection.IsVisible = false;
								}
							}
						}
						else if (GUILayout.Button("Show graph"))
						{
							if (graph.NodeCollection != null) { graph.NodeCollection.IsVisible = true; }
							if (graph.EdgeCollection != null) { graph.EdgeCollection.IsVisible = true; }
						}
					}
				}

				serializedObject.ApplyModifiedProperties();
			}

			void CreateNewNodeCollection()
			{
				GameObject nodeCollectionObject = CreateNewCollection("Nodes", graph.transform);

				if (!nodeCollectionObject) { return; }

				var nodeCollection = nodeCollectionObject.AddComponent<NodeCollection>();
				
				nodeCollection.nodeDefaultColor = Parameters.Instance.NodeDefaultColor;
				nodeCollection.IsVisible = Parameters.Instance.NodeCollectionIsVisible;

				nodeCollection.ApplyParametersToNodes();

				nodeCollectionObjectProperty.objectReferenceValue = nodeCollectionObject;
				nodeCollectionProperty.objectReferenceValue = nodeCollection;
			}

			void CreateNewEdgeCollection()
			{
				GameObject edgeCollectionObject = CreateNewCollection("Edges", graph.transform);

				if (!edgeCollectionObject) { return; }

				var edgeCollection = edgeCollectionObject.AddComponent<EdgeCollection>();
				
				edgeCollection.edgeDefaultColor = Parameters.Instance.EdgeDefaultColor;
				edgeCollection.IsVisible = Parameters.Instance.NodeCollectionIsVisible;

				edgeCollection.ApplyParametersToEdges();

				edgeCollectionObjectProperty.objectReferenceValue = edgeCollectionObject;
				edgeCollectionProperty.objectReferenceValue = edgeCollection;
			}

			GameObject CreateNewCollection(string defaultCollectionName, Transform parent)
			{
				string collectionName = defaultCollectionName;
				int nameSuffix = 1;

				while (GameObject.Find(collectionName))
				{
					collectionName = defaultCollectionName + nameSuffix++;
				}

				GameObject collectionObject = new GameObject(collectionName)
				{
					transform = { position = Vector3.zero, parent = parent}
				};

				return collectionObject;
			}
		}

#endif

		#endregion GRAPH EDITOR
	}
}