using System.Collections.Generic;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;

namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.OneCyclePerUpdateSearch
{
	public abstract class Search
    {
		protected Search(SearchTypes searchType, Node source)
        {
            SearchType = searchType;
			Source = source;		
        }
		
		public SearchTypes SearchType { get; }
		
		public Node Source { get; }
		
		public List<Edge> Solution { get; protected set; }
		
		public abstract SearchResults DoOneCycleOfSearch();
		
		public abstract SearchResults DoSearch();

		// Extracts the list of edges in a path by working backwards from the current node.
		protected static List<Edge> ExtractPath(PathData current)
		{
			var path = new List<Edge>();
			
			while (current.edgeFromParent)
			{
				path.Add(current.edgeFromParent);
				current = current.parentPathData;
			}
			
			path.Reverse();
			return path;
		}
    }
}