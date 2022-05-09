using GameBrains.Extensions.Vectors;
using GameBrains.Visualization;
using UnityEngine;

namespace GameBrains.Actuators.Motion.Navigation.PathManagement
{
    public class QuasiEdge
    {
        #region Members and Properties

        public bool trueEdge;
        public VectorXZ fromLocation;
        public EdgeBeacon edgeBeacon;
        public VectorXZ toLocation;
        public PointMarker toPointMarker;
        public PointBeacon toPointBeacon;

        #endregion Members and Properties

        #region Contructor

        public QuasiEdge(
            bool trueEdge,
            VectorXZ fromLocation,
            EdgeBeacon edgeBeacon,
            VectorXZ toLocation,
            PointMarker toPointMarker,
            PointBeacon toPointBeacon)
        {
            this.trueEdge = trueEdge;
            this.fromLocation = fromLocation;
            this.edgeBeacon = edgeBeacon;
            this.toLocation = toLocation;
            this.toPointMarker = toPointMarker;
            this.toPointBeacon = toPointBeacon;
        }

        #endregion Contructor

        #region Methods

        public void Show(bool show)
        {
            if (edgeBeacon != null) { edgeBeacon.Hide(!show); }
            if (toPointMarker != null) { toPointMarker.Hide(!show); }
            if (toPointBeacon != null) { toPointBeacon.Hide(!show); }
        }
        
        public void CleanUp()
        {
            Object.Destroy(edgeBeacon);
            edgeBeacon = null;
				
            Object.Destroy(toPointMarker);
            toPointMarker = null;
				
            Object.Destroy(toPointBeacon);
            toPointBeacon = null;
        }

        #endregion Methods
    }
}