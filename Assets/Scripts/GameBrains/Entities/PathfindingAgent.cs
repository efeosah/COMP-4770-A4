using GameBrains.Entities.EntityData;

namespace GameBrains.Entities
{
    public class PathfindingAgent : SteerableAgent
    {
        #region Motion

        public new PathfindingData Data => data as PathfindingData;

        #endregion Motion

        #region Awake

        public override void Awake()
        {
            base.Awake();

            data = (PathfindingData) transform;
        }
        
        #endregion Awake
        
        #region Act

        protected override void Act(float deltaTime)
        {
            base.Act(deltaTime);
            
            // TODO: Choose a random destination, find and follow path, rinse and repeat.
        }
        
        #endregion Act
    }
}