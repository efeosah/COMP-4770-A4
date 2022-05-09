using System.Collections.Generic;
using GameBrains.Actuators.Motion.Navigation.SearchAlgorithms;
using GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.CycleLimitedSearch;
using GameBrains.Actuators.Motion.Navigation.SearchGraph;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.GameManagement;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.PathManagement
{
	public sealed class PathManager : ExtendedMonoBehaviour
	{
		#region Members and Properties

		public LayerMask pathObstaclesLayerMask;

		public Graph graph;
		public float closeEnoughDistance = 1;
		public List<PathPlanner> searchRequests;

		public Graph Graph { get => graph; set => graph = value; }

		public List<PathPlanner> SearchRequests
		{
			get => searchRequests;
			private set => searchRequests = value;
		}

		public LayerMask PathObstaclesLayerMask
		{
			get => pathObstaclesLayerMask;
			set => pathObstaclesLayerMask = value;
		}

		public float CloseEnoughDistance
		{
			get => closeEnoughDistance;
			set => closeEnoughDistance = value;
		}

		public int SearchesPerCycle { get; set; }
		
		static PathManager instance;
	
		public static PathManager Instance
		{
			get
			{
				if (instance == null) { instance = FindObjectOfType<PathManager>(); }
			
				return instance;
			}
		}

		#endregion Members and Properties

		#region Start

		public override void Start()
		{
			base.Start();
			graph = FindObjectOfType<Graph>();
			pathObstaclesLayerMask = Parameters.Instance.ObstacleLayerMask;
			SearchRequests = new List<PathPlanner>();
			SearchesPerCycle = Parameters.Instance.MaximumSearchCyclesPerUpdateStep;
			graph.IsLocked = true;
			BestPathTable.Create(graph);
		}

		#endregion Start

		#region Update

		public override void Update()
		{
			base.Update();
			
			int cyclesRemaining = SearchesPerCycle;
			int currentSearchRequestIndex = 0;

			while (cyclesRemaining > 0 && SearchRequests.Count > 0)
			{
				SearchResults searchResult = SearchRequests[currentSearchRequestIndex].CycleOnce();

				if (searchResult != SearchResults.Running)
				{
					SearchRequests.RemoveAt(currentSearchRequestIndex);
				}
				else
				{
					currentSearchRequestIndex++;
				}

				if (currentSearchRequestIndex >= SearchRequests.Count)
				{
					currentSearchRequestIndex = 0;
				}

				cyclesRemaining--;
			}
		}

		#endregion Update

		#region Path Planner

		public void AddPathPlanner(PathPlanner pathPlanner)
		{
			// make sure the bot does not already have a current search
			if (pathPlanner != null && !SearchRequests.Contains(pathPlanner))
			{
				SearchRequests.Add(pathPlanner);
			}
		}

		public void RemovePathPlanner(PathPlanner pathPlanner)
		{
			if (pathPlanner != null && SearchRequests != null)
			{
				SearchRequests.Remove(pathPlanner);
			}
		}

		#endregion Path Planner
	}
}