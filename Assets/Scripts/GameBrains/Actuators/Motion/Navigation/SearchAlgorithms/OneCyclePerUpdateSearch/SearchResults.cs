namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.OneCyclePerUpdateSearch
{
	// A search can Succeed (find a path), or Fail (not find a path).
	// One cycle per update searches that do not complete in the current cycle return Running.
	public enum SearchResults
    {
		Success, 
		Failure, 
		Running
    }
}