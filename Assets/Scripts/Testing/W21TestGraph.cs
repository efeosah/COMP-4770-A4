using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using UnityEngine;

namespace Testing
{
    public sealed class W21TestGraph : MonoBehaviour
    {
        #region Members and Properties

        public bool testToggleGraphLock;
        public bool testToggleGraphVisibility;
        public bool testToggleNodeCollectionVisibility;
        public bool testToggleEdgeCollectionVisibility;
        public bool testNodes;
        public bool testEdges;
        public bool testToggleNodeVisibility;
        public bool testToggleEdgeVisibility;

        Graph graph;

        #endregion Members and Properties

        #region Awake
        
        public void Awake()
        {
            graph = FindObjectOfType<Graph>();
        }

        #endregion Awake

        #region Update
        
        void Update()
        {
            if (graph == null) { return; }

            if (testToggleGraphLock)
            {
                testToggleGraphLock = false;

                graph.IsLocked = !graph.IsLocked;
            }

            if (testToggleGraphVisibility)
            {
                testToggleGraphVisibility = false;

                graph.IsVisible = !graph.IsVisible;
            }

            if (graph.NodeCollection != null)
            {
                if (testToggleNodeCollectionVisibility)
                {
                    testToggleNodeCollectionVisibility = false;

                    graph.NodeCollection.IsVisible = !graph.NodeCollection.IsVisible;
                }

                if (testNodes)
                {
                    testNodes = false;

                    for (var index = 0; index < graph.NodeCollection.Nodes.Length; index++)
                    {
                        Node node = graph.NodeCollection.Nodes[index];
                        Debug.Log($"Node {index}: {node}");
                    }
                }

                if (testToggleNodeVisibility)
                {
                    testToggleNodeVisibility = false;

                    if (graph.NodeCollection.Nodes.Length > 0)
                    {
                        graph.NodeCollection.Nodes[0].IsVisible =
                            !graph.NodeCollection.Nodes[0].IsVisible;
                    }
                }
            }

            if (graph.EdgeCollection != null)
            {
                if (testToggleEdgeCollectionVisibility)
                {
                    testToggleEdgeCollectionVisibility = false;

                    graph.EdgeCollection.IsVisible = !graph.EdgeCollection.IsVisible;
                }

                if (testEdges)
                {
                    testEdges = false;

                    for (var index = 0; index < graph.EdgeCollection.Edges.Length; index++)
                    {
                        Edge edge = graph.EdgeCollection.Edges[index];
                        Debug.Log($"Edge {index}: {edge}");
                    }
                }

                if (testToggleEdgeVisibility)
                {
                    testToggleEdgeVisibility = false;

                    if (graph.EdgeCollection.Edges.Length > 0)
                    {
                        graph.EdgeCollection.Edges[0].IsVisible =
                            !graph.EdgeCollection.Edges[0].IsVisible;
                    }
                }
            }
        }
        
        #endregion Update
    }
}