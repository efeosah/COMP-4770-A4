using System;
using System.Collections.Generic;
using GameBrains.Actuators.Motion.Navigation.Heuristics;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.DataStructures;

namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.OneCyclePerUpdateSearch
{
	using PQItem = MappedPriorityQueue<Node, PathData, float>.PQItem;

	public sealed class AStarSearch : Search
	{
		readonly MappedPriorityQueue<Node, PathData, float> openSet;
		readonly Dictionary<Node, PathData> closedSet;

		public AStarSearch(Node source, Node destination)
			: this(
				source,
				node => node == destination,
				new EuclideanDistance(destination).Calculate)
		{
		}

		public AStarSearch(Node source, Node destination, HeuristicDelegate heuristicDelegate)
			: this(
				source,
				node => node == destination,
				heuristicDelegate)
		{
		}

		public AStarSearch(Node source, Predicate<Node> isGoal, HeuristicDelegate heuristic)
			: base(SearchTypes.AStar, source)
		{
			IsGoal = isGoal;
			H = heuristic;

			openSet = new MappedPriorityQueue<Node, PathData, float>();
			closedSet = new Dictionary<Node, PathData>();

			float g = 0;
			float h = H(source);
			float f = g + h;

			openSet.Enqueue(new PQItem(source, new PathData(g), f));
		}

		public Predicate<Node> IsGoal { get; }

		public HeuristicDelegate H { get; }

		public override SearchResults DoOneCycleOfSearch() { return DoSearch(); }

		public override SearchResults DoSearch()
		{
			if (!openSet.IsEmpty)
			{
				PQItem currentItem = openSet.Dequeue();
				Node currentNode = currentItem.Key;
				PathData currentPathData = currentItem.Value;

				if (IsGoal(currentNode))
				{
					Solution = ExtractPath(currentPathData);
					return SearchResults.Success;
				}

				closedSet[currentNode] = currentPathData;

				foreach (Edge edgeFromCurrent in currentNode.outEdges.Values)
				{
					Node neighbourNode = edgeFromCurrent.ToNode;
					float h = H(neighbourNode);
					float g = currentPathData.g + edgeFromCurrent.Cost;
					float f = g + h;

					PathData neighbourPathData
						= new PathData(g, edgeFromCurrent, currentItem.Value);

					if (closedSet.ContainsKey(neighbourNode))
					{
						PathData closedPathData = closedSet[neighbourNode];
						if (g < closedPathData.g)
						{
							closedSet.Remove(neighbourNode);
							openSet.Enqueue(new PQItem(neighbourNode, neighbourPathData, f));
						}
					}
					else if (openSet.ContainsKey(neighbourNode))
					{
						PathData openPathData = openSet[neighbourNode].Value;
						if (g < openPathData.g)
						{
							openSet.ChangeValueAndPriority(neighbourNode, neighbourPathData, f);
						}
					}
					else
					{
						openSet.Enqueue(new PQItem(neighbourNode, neighbourPathData, f));
					}
				}

				return SearchResults.Running;
			}

			return SearchResults.Failure;
		}
	}
}