using GameBrains.Actuators.Motion.Navigation.SearchGraph;

namespace GameBrains.Actuators.Motion.Navigation.Heuristics
{
    // Always zero heuristic using in Dijkstra's search.
    public class Zero
    {
        public float Calculate(Node n) { return 0; }
    }
}