using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.Heuristics
{
	// Euclidean distance heuristic based distance between node positions.
	// Used by heuristic searches such as A* search and TimeSliced-A* search.

	public class EuclideanDistance
	{
		readonly Node goal;

		public EuclideanDistance(Node goal)
		{
			this.goal = goal;
		}

		public float Calculate(Node n)
		{
			return goal ? Vector3.Distance(n.Position, goal.Position) : 0;
		}
	}
}