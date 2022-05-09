using GameBrains.Actuators.Motion.Navigation.SearchGraph;

namespace GameBrains.Actuators.Motion.Navigation.Heuristics
{
	// Delegate for heuristic value calculations used in heuristic searches such as
	// A* search and TimeSliced-A* search.
	public delegate float HeuristicDelegate(Node n);
}