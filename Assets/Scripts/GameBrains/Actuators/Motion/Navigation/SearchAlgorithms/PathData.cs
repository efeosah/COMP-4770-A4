using GameBrains.Actuators.Motion.Navigation.SearchGraph;

namespace GameBrains.Actuators.Motion.Navigation.SearchAlgorithms
{
    // Used to store the current best path cost, which edge connects from the parent,
    // and the parent's path data. Together these can be used to extract the best path.
    public class PathData
    {
        public readonly float g;
        public readonly Edge edgeFromParent;
        public readonly PathData parentPathData;

        public PathData(
            float g,
            Edge edgeFromParent = null,
            PathData parentPathData = null)
        {
            this.g = g;
            this.edgeFromParent = edgeFromParent;
            this.parentPathData = parentPathData;
        }
    }
}