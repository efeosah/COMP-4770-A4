using GameBrains.Actuators.Motion.Steering.VelocityBased;
using GameBrains.Entities;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using UnityEngine;

namespace Testing
{
    public sealed class W20HTestFace : ExtendedMonoBehaviour
    {
        public bool respawn;
        public bool respawnTarget;
        public bool setTargetVelocity;
        public bool setAngularVelocity;
        public bool setAngularAcceleration;
        public bool testUsingTargetLocation;
        public bool testUsingTargetTransform;
        public bool testUsingTargetMovingAgent;
        public bool removeFaceFromSteeringBehaviours;
        public VectorXZ spawnLocation = new VectorXZ(10f, 0f);
        public VectorXZ targetSpawnLocation = new VectorXZ(0f, 0f);
        public float angularVelocity = 45f;
        public float angularAcceleration = 5f;
        public VectorXZ targetLocation = new VectorXZ(0f, 10f);
        public VectorXZ targetVelocity = new VectorXZ(0f, 2f);
        public Transform targetTransform;
        public MovingAgent targetMovingAgent;
        public SteerableAgent steerableAgent;
        Face face;
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

            if (removeFaceFromSteeringBehaviours)
            {
                removeFaceFromSteeringBehaviours = false;
                RemoveAndDestroyFace();
            }
            
            if (setTargetVelocity)
            {
                setTargetVelocity = false;

                if (targetMovingAgent != null)
                {
                    targetMovingAgent.Data.Velocity = targetVelocity;
                }
            }

            if (testUsingTargetMovingAgent)
            {
                testUsingTargetMovingAgent = false;

                if (targetMovingAgent != null)
                {
                    RemoveAndDestroyFace();
                    face = Face.CreateInstance(steerableAgent.Data, targetMovingAgent.Data);
                    SetParameters(face);
                    steerableAgent.Data.AddSteeringBehaviour(face);
                }
            }

            if (testUsingTargetTransform)
            {
                testUsingTargetTransform = false;

                if (targetTransform != null)
                {
                    RemoveAndDestroyFace();
                    face = Face.CreateInstance(steerableAgent.Data, targetTransform);
                    SetParameters(face);
                    steerableAgent.Data.AddSteeringBehaviour(face);
                }
            }

            if (testUsingTargetLocation)
            {
                testUsingTargetLocation = false;
                RemoveAndDestroyFace();
                face = Face.CreateInstance(steerableAgent.Data, targetLocation);
                SetParameters(face);
                steerableAgent.Data.AddSteeringBehaviour(face);
            }
        }
        
        void SetParameters(Face sb)
        {
            sb.NoStop = noStop;
            sb.NoSlow = noSlow;
            sb.NeverCompletes = neverCompletes;
            sb.AngularStopAtSpeed = angularStopAtSpeed;
            sb.SlowEnoughAngularVelocity = slowEnoughAngularVelocity;
            sb.AngularDrag = angularDrag;
            sb.CloseEnoughAngle = closeEnoughAngle;
        }

        void RemoveAndDestroyFace()
        {
            if (face != null)
            {
                steerableAgent.Data.RemoveSteeringBehaviour(face);
                Destroy(face);
                face = null;
            }
        }
    }
}