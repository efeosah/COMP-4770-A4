using System.Collections.Generic;
using GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.CycleLimitedSearch;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms
{
    public static class BestPathTable
    {
        // Used to look up the best path cost and first step of the path for a
        // give source node and destination node.
        static readonly Dictionary<SourceAndDestination, NextAndCost> NextNodeAndPathCostTable;

        static BestPathTable()
        {
            NextNodeAndPathCostTable = new Dictionary<SourceAndDestination, NextAndCost>();
        }
        
        public static void Create(Graph graph)
        {
            if (graph == null || graph.NodeCollection == null || graph.NodeCollection.Nodes.Length == 0) return;

            NextNodeAndPathCostTable.Clear();

            foreach (Node sourceNode in graph.NodeCollection.Nodes)
            {
                var currentSearch = new DijkstrasSearch(sourceNode, node => false);
                currentSearch.DoAllCycleOfSearch();
                Dictionary<Node, PathData> closedSet = currentSearch.closedSet;
                GenerateNextNodeAndCost(sourceNode, closedSet);
            }
        }
        
        public static bool PathExists(Node source, Node destination)
        {
            var sourceAndDestinationNodes = new SourceAndDestination(source, destination);
            float cost = NextNodeAndPathCostTable[sourceAndDestinationNodes].pathCost;
            return !Mathf.Approximately(cost, float.MaxValue);
        }

        public static Node NextNode(Node source, Node destination)
        {
            var sourceAndDestinationNodes = new SourceAndDestination(source, destination);
            return NextNodeAndPathCostTable[sourceAndDestinationNodes].nextNode;
        }

        public static float Cost(Node source, Node destination)
        {
            var sourceAndDestinationNodes = new SourceAndDestination(source, destination);
            return NextNodeAndPathCostTable[sourceAndDestinationNodes].pathCost;
        }

        public static List<Edge> Path(Node source, Node destination)
        {
            var path = new List<Edge>();
            Node fromNode = source;

            while (fromNode != null && fromNode != destination)
            {
                var sourceAndDestination = new SourceAndDestination(fromNode, destination);

                NextAndCost nextAndCost = NextNodeAndPathCostTable[sourceAndDestination];
                Node toNode = nextAndCost.nextNode;

                if (toNode != null) { path.Add(fromNode.outEdges[toNode]); }

                fromNode = toNode;
            }

            return path;
        }

        static void GenerateNextNodeAndCost(Node sourceNode, Dictionary<Node, PathData> closedSet)
        {
            foreach (Node destinationNode in closedSet.Keys)
            {
                PathData pathData = closedSet[destinationNode];
                float pathCost = pathData.g;
                Node nextNode = destinationNode;

                while (pathData != null && pathData.edgeFromParent != null)
                {
                    Edge edgeFromParent = pathData.edgeFromParent;
                    nextNode = edgeFromParent.ToNode;
                    pathData = pathData.parentPathData;
                }

                var sourceAndDestination = new SourceAndDestination(sourceNode, destinationNode);
                var nextNodeAndPathCost = new NextAndCost(nextNode, pathCost);
                NextNodeAndPathCostTable[sourceAndDestination] = nextNodeAndPathCost;
            }
        }
    }

    // The next node on the least cost path and the cost of the path
    public struct NextAndCost
    {
        public Node nextNode;
        public float pathCost;

        public NextAndCost(Node nextNode, float pathCost)
        {
            this.nextNode = nextNode;
            this.pathCost = pathCost;
        }
    }

    // The source and destination of a path
    public struct SourceAndDestination
    {
        public Node source;
        public Node destination;

        public SourceAndDestination(Node source, Node destination)
        {
            this.source = source;
            this.destination = destination;
        }
    }
}