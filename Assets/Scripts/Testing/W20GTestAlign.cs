using GameBrains.Entities;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.Motion.Steering.VelocityBased;
using UnityEngine;

namespace Testing
{
    public sealed class W20GTestAlign : ExtendedMonoBehaviour
    {
        public bool respawn;
        public bool respawnTarget;
        public bool setAngularVelocity;
        public bool setAngularAcceleration;
        public bool setTargetAngularVelocity;
        public bool testUsingTargetOrientation;
        public bool testUsingTargetTransform;
        public bool testUsingTargetMovingAgent;
        public bool removeAlignFromSteeringBehaviours;
        public VectorXZ spawnLocation = new VectorXZ(10f, 0f);
        public VectorXZ targetSpawnLocation = new VectorXZ(0f, 0f);
        public float targetAngularVelocity = 45f;
        public float angularVelocity = 45f;
        public float angularAcceleration = 5f;
        public float targetOrientation = 90f;
        public Transform targetTransform;
        public MovingAgent targetMovingAgent;
        public SteerableAgent steerableAgent;
        Align align;
        public bool noStop;
        public bool noSlow;
        public bool neverCompletes;
        public float angularStopAtSpeed = 0.1f;
        public float slowEnoughAngularVelocity = 5f;
        public float angularDrag = 1.1f;
        public float closeEnoughAngle = 5f;

        public override void Update()
        {
            base.Update();
            
            if (steerableAgent == null) { return; }

            if (respawn)
            {
                respawn = false;
                steerableAgent.Spawn((VectorXYZ)spawnLocation);
            }
            
            if (respawnTarget)
            {
                respawnTarget = false;
                targetMovingAgent.Spawn((VectorXYZ)targetSpawnLocation);
            }

            if (setAngularVelocity)
            {
                setAngularVelocity = false;
                steerableAgent.Data.AngularVelocity = angularVelocity;
            }

            if (setAngularAcceleration)
            {
                setAngularAcceleration = false;
                steerableAgent.Data.AngularAcceleration = angularAcceleration;
            }

            if (removeAlignFromSteeringBehaviours)
            {
                removeAlignFromSteeringBehaviours = false;
                RemoveAndDestroyAlign();
            }
            
            if (setTargetAngularVelocity)
            {
                setTargetAngularVelocity = false;

                if (targetMovingAgent != null)
                {
                    targetMovingAgent.Data.AngularVelocity = targetAngularVelocity;
                }
            }

            if (testUsingTargetMovingAgent)
            {
                testUsingTargetMovingAgent = false;

                if (targetMovingAgent != null)
                {
                    RemoveAndDestroyAlign();
                    align = Align.CreateInstance(steerableAgent.Data, targetMovingAgent.Data);
                    SetParameters(align);
                    steerableAgent.Data.AddSteeringBehaviour(align);
                }
            }

            if (testUsingTargetTransform)
            {
                testUsingTargetTransform = false;

                if (targetTransform != null)
                {
                    RemoveAndDestroyAlign();
                    align = Align.CreateInstance(steerableAgent.Data, targetTransform);
                    SetParameters(align);
                    steerableAgent.Data.AddSteeringBehaviour(align);
                }
            }

            if (testUsingTargetOrientation)
            {
                testUsingTargetOrientation = false;
                RemoveAndDestroyAlign();
                align = Align.CreateInstance(steerableAgent.Data, targetOrientation);
                SetParameters(align);
                steerableAgent.Data.AddSteeringBehaviour(align);
            }
        }
        
        void SetParameters(Align sb)
        {
            sb.NoStop = noStop;
            sb.NoSlow = noSlow;
            sb.NeverCompletes = neverCompletes;
            sb.AngularStopAtSpeed = angularStopAtSpeed;
            sb.SlowEnoughAngularVelocity = slowEnoughAngularVelocity;
            sb.AngularDrag = angularDrag;
            sb.CloseEnoughAngle = closeEnoughAngle;
        }
        
        void RemoveAndDestroyAlign()
        {
            if (align != null)
            {
                steerableAgent.Data.RemoveSteeringBehaviour(align);
                Destroy(align);
                align = null;
            }
        }
    }
}