using GameBrains.Actuators.Motion.Motors;
using GameBrains.Entities.EntityData;
using GameBrains.Extensions.Vectors;
using UnityEngine;

namespace GameBrains.Entities
{
    public class MovingAgent : Agent
    {
        #region Kinematic Data
        
        public new KinematicData Data => data as KinematicData;

        #endregion Kinematic Data
        
        #region Motor
        
        protected Motor motor;
        
        public Motor Motor
        {
            get => motor;
            set => motor = value;
        }
        
        #endregion Motor

        #region Awake
        
        public override void Awake()
        {
            base.Awake();

            data = (KinematicData)transform;
            
            motor = GetComponentInChildren<Motor>();
        }
        
        #endregion Awake
        
        #region Act

        protected override void Act(float deltaTime)
        {
            base.Act(deltaTime);
            
            if (motor != null && motor.enabled)
            {
                motor.CalculatePhysics(Data, deltaTime);
            }
        }
        
        #endregion Act
        
        #region Spawn

        // Relocate and reactive moving entity. Reset Kinematic Data.
        public override void Spawn(VectorXYZ spawnPoint)
        {
            base.Spawn(spawnPoint);

            Data.ResetKinematicData();

            var characterController = GetComponent<CharacterController>();

            if (characterController != null)
            {
                ((Entity)this).Data.CenterOffset = characterController.center;
                ((Entity)this).Data.Radius = characterController.radius;
                ((Entity)this).Data.Height = characterController.height;
            }
        }

        #endregion
    }
}