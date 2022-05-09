using System;
using System.Collections.Generic;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;

namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.SingleUpdateSearch
{
	public static class LeastCostPathTable
	{
		const float Tolerance = 0.1f;

		static readonly C5.HashDictionary<NodePair, NodeAndCost> NextNodeAndCostTable
			= new C5.HashDictionary<NodePair, NodeAndCost>();
		
		public static float Cost(Node source, Node destination)
		{
			return NextNodeAndCostTable[new NodePair(source, destination)].cost;
		}
		
		public static bool PathExists(Node source, Node destination)
		{
			return
				Math.Abs(
					NextNodeAndCostTable[new NodePair(source, destination)].cost - float.MaxValue)
				> Tolerance;
		}
		
		public static Node NextNode(Node source, Node destination)
		{
			return NextNodeAndCostTable[new NodePair(source, destination)].node;
		}
		
		public static List<Edge> Path(Node source, Node destination)
		{
			var path = new List<Edge>();
			Node fromNode = source;
			
			while (fromNode != null && fromNode != destination)
			{
				NodeAndCost nextNodeAndCost
					= NextNodeAndCostTable[new NodePair(fromNode, destination)];
				Node toNode = nextNodeAndCost.node;

				if (toNode)
				{
					//path.Add(fromNode.outEdges.Find(element => element.ToNode == toNode));
					path.Add(fromNode.outEdges[toNode]);
				}

				fromNode = toNode;
			}
			
			return path;
		}

		// Create a table containing the next node in a least cost path to every node
		// from every other node using DijkstrasSearch. This can be created once and used
		// instead of realtime searches.
		public static void Create(Graph graph)
		{
			foreach (Node sourceNode in graph.NodeCollection.Nodes)
			{
				foreach (Node destinationNode in graph.NodeCollection.Nodes)
				{
					NextNodeAndCostTable[new NodePair(sourceNode, destinationNode)]
						= new NodeAndCost(null, float.MaxValue);
				}
			}
			
			foreach (Node node in graph.NodeCollection.Nodes)
			{
				DijkstrasSearch(graph.NodeCollection.Nodes, node);
			}
		}
		
		public static void DijkstrasSearch(Node[] nodes, Node source)
		{
			C5.HashDictionary<Node, Entry> spTable = new C5.HashDictionary<Node, Entry>();

			foreach (Node node in nodes)
			{
				spTable.Add(node, new Entry(false, float.MaxValue, null));
			}

			Entry sourceEntry = spTable[source];
			sourceEntry.cost = 0;
			spTable[source] = sourceEntry;

			IComparer<NodeAndCost> costComparer
				= C5.ComparerFactory<NodeAndCost>.CreateComparer(
					(nc1, nc2) => nc1.cost.CompareTo(nc2.cost));

			C5.IntervalHeap<NodeAndCost> priorityQueue
				= new C5.IntervalHeap<NodeAndCost>(costComparer);

			priorityQueue.Add(new NodeAndCost(source, 0));

			while (!priorityQueue.IsEmpty)
			{
				NodeAndCost nodeAndCost = priorityQueue.DeleteMin();

				Node currentNode = nodeAndCost.node;

				if (spTable[currentNode].known) continue;

				Entry currentNodeEntry = spTable[currentNode];
				currentNodeEntry.known = true;
				spTable[currentNode] = currentNodeEntry;

				foreach (Edge edge in currentNode.outEdges.Values)
				{
					Node toNode = edge.ToNode;
					float toNodeCost = spTable[currentNode].cost + edge.Cost;

					if (spTable[toNode].cost <= toNodeCost) { continue; }

					Entry toNodeEntry = spTable[toNode];
					toNodeEntry.cost = toNodeCost;
					toNodeEntry.predecessor = currentNode;
					spTable[toNode] = toNodeEntry;
					priorityQueue.Add(new NodeAndCost(toNode, toNodeCost));
				}
			}

			foreach (Node node in nodes)
			{
				NextNodeAndCostTable[new NodePair(source, node)] =
					ExtractNextNodeFromTable(spTable, source, node);
			}
		}
		
		// Walk back through the predecessors to the one after source.
		static NodeAndCost ExtractNextNodeFromTable(
			C5.HashDictionary<Node, Entry> sptTable,
			Node source,
			Node destination)
		{
			NodeAndCost nextNodeAndCost
				= new NodeAndCost(destination, sptTable[destination].cost);

			while (sptTable[nextNodeAndCost.node].predecessor &&
			       sptTable[nextNodeAndCost.node].predecessor != source)
			{
				nextNodeAndCost.node = sptTable[nextNodeAndCost.node].predecessor;
			}
			
			return nextNodeAndCost;
		}
		
		struct Entry
		{
			public bool known;
			public float cost;
			public Node predecessor;
			
			public Entry(bool known, float cost, Node predecessor)
			{
				this.known = known;
				this.cost = cost;
				this.predecessor = predecessor;
			}
		}
		
		struct NodePair
		{
			public Node source;
			public Node destination;
			
			public NodePair(Node source, Node destination)
			{
				this.source = source;
				this.destination = destination;
			}
		}
		
		struct NodeAndCost
		{
			public Node node;
			public float cost;
			
			public NodeAndCost(Node node, float cost)
			{
				this.node = node;
				this.cost = cost;
			}
		}
	}
}