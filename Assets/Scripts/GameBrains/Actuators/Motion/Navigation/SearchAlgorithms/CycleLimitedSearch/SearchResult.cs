namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms.CycleLimitedSearch
{
	// A search can Succeed (find a path), or Fail (not find a path).
	// Cycle limited searches that do not complete in the current cycle return Running.
	public enum SearchResults
    {
		Success, 
		Failure,
		Running
    }
}