using System.Collections.Generic;
using System.Text;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.Extensions.Lists;
using GameBrains.Extensions.Vectors;
using GameBrains.Visualization;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.PathManagement
{
	// A path is a list of edges from the source to destination or an empty list if no path exists.
	public class Path
    {
	    #region Constructor
	    
		public Path(VectorXZ source, List<Edge> edges, VectorXZ destination)
		{
			Source = source;
			Destination = destination;
			EdgesStartWithSource = 
				edges != null && !edges.IsEmpty() && Source == edges[0].FromNode.Location;
			EdgesEndWithDestination = 
				edges != null && !edges.IsEmpty() && Destination == edges[edges.Count-1].ToNode.Location;
			var waypointCount = edges == null ? 2 : edges.Count + 2;
			quasiEdges = new List<QuasiEdge>(waypointCount-1);
			
			CreateQuasiEdges(edges);
		}
		
		#endregion Constructor

		#region Members and properties
		
   		public VectorXZ Source { get; set; }
		public VectorXZ Destination { get; set; }

		public bool EdgesStartWithSource { get; set; }
		public bool EdgesEndWithDestination { get; set; }
		
		public bool IsEmpty => QuasiEdges == null || QuasiEdges.Count == 0;

		readonly List<QuasiEdge> quasiEdges;
		public List<QuasiEdge> QuasiEdges => quasiEdges;

		#endregion Members and properties

		#region Path Methods

		void CreateQuasiEdges(List<Edge> edges)
		{
			if (!EdgesStartWithSource)
			{
				CreateQuasiEdge(
					false, 
					Source, 
					edges.Count == 0 ? Destination : edges[0].FromNode.Location);
			}
			
			foreach (var edge in edges)
			{
				CreateQuasiEdge(true, edge.FromNode.Location, edge.ToNode.Location);
			}

			if (!EdgesEndWithDestination && edges.Count > 0)
			{
				CreateQuasiEdge(false, edges[edges.Count-1].ToNode.Location, Destination);
			}
		}

		public void CreateQuasiEdge(bool trueEdge, VectorXZ fromLocation, VectorXZ toLocation)
		{
			var edgeBeacon = CreateEdgeBeacon(fromLocation, toLocation);
			var toPointMarker = CreatePointMarker(toLocation);
			var toPointBeacon = CreatePointBeacon(toLocation);
			
			quasiEdges.Add(
				new QuasiEdge(
					trueEdge, 
					fromLocation,
					edgeBeacon, 
					toLocation, 
					toPointMarker, 
					toPointBeacon));
		}

		public void ReplaceQuasiEdge(int index, bool trueEdge, VectorXZ fromLocation, VectorXZ toLocation)
		{
			if (index < quasiEdges.Count)
			{
				if (quasiEdges[index] != null)
				{
					quasiEdges[index].Show(false);
					quasiEdges[index].CleanUp();
				}

				var edgeBeacon = CreateEdgeBeacon(fromLocation, toLocation);
				var toPointMarker = CreatePointMarker(toLocation);
				var toPointBeacon = CreatePointBeacon(toLocation);
				
				quasiEdges[index] =
					new QuasiEdge(
						trueEdge, 
						fromLocation,
						edgeBeacon, 
						toLocation, 
						toPointMarker, 
						toPointBeacon);
			}
			else
			{
				Debug.LogWarning("ReplaceQuasiEdge Index out of range.");
			}
		}

		PointMarker CreatePointMarker(VectorXZ location)
		{
			var pointMarker = ScriptableObject.CreateInstance<PointMarker>();
			pointMarker.Draw(location);
			pointMarker.Hide(true);
			return pointMarker;
		}

		PointBeacon CreatePointBeacon(VectorXZ location)
		{
			var pointBeacon = ScriptableObject.CreateInstance<PointBeacon>();
			pointBeacon.Draw(location);
			pointBeacon.Hide(true);
			return pointBeacon;
		}
		
		EdgeBeacon CreateEdgeBeacon(VectorXZ fromLocation, VectorXZ toLocation)
		{
			var edgeBeacon = ScriptableObject.CreateInstance<EdgeBeacon>();
			edgeBeacon.Draw(fromLocation, toLocation);
			edgeBeacon.Hide(true);
			return edgeBeacon;
		}

		public QuasiEdge Dequeue()
		{
			if (IsEmpty) { return null; }

			QuasiEdge firstEdge = QuasiEdges[0];
			QuasiEdges.RemoveAt(0);
			return firstEdge;
		}
		
		public QuasiEdge Peek()
		{
			if (IsEmpty) { return null; }
		
			QuasiEdge firstEdge = QuasiEdges[0];
			return firstEdge;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Path:");
			sb.Append(QuasiEdges.ToNumberedItemsString());
			return sb.ToString();
		}

		public void Show(bool show)
		{
			foreach (var quasiEdge in quasiEdges)
			{
				quasiEdge.Show(show);
			}
		}

		public void CleanUp()
		{
			for (var index = quasiEdges.Count - 1; index >= 0 ; index--)
			{
				var quasiEdge = quasiEdges[index];
				quasiEdge.Show(false);
				quasiEdge.CleanUp();
				quasiEdges.RemoveAt(index);
			}
		}

		#endregion Path Methods
    }
}