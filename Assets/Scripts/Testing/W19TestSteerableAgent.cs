using System.Collections.Generic;
using GameBrains.Entities;
using GameBrains.Extensions.Attributes;
using GameBrains.Extensions.MonoBehaviours;
using GameBrains.Extensions.Vectors;
using GameBrains.Motion.Steering.VelocityBased;
using UnityEngine;

namespace Testing
{
    public sealed class W19TestSteerableAgent : ExtendedMonoBehaviour
    {
        public bool respawn;
        public bool testSeekUsingTargetLocation;
        public bool testSeekUsingTargetTransform;
        public bool testSeekUsingTargetMovingAgent;
        public bool testLinearStop;
        public bool removeLinearStopFromSteeringBehaviours;
        public bool removeSeekFromSteeringBehaviours;
        public bool clearSteeringBehaviours;
        public VectorXZ spawnLocation = VectorXZ.zero;
        public VectorXZ targetLocation = new VectorXZ(0, 10);
        public List<Transform> targetTransforms;
        [ReadOnlyInPlaymode]
        [SerializeField] Transform targetTransform;
        public MovingAgent targetMovingAgent;
        public SteerableAgent steerableAgent;
        Seek seek;
        LinearStop linearStop;

        public override void Update()
        {
            base.Update();

            for (int i = 0; i < targetTransforms.Count; i++)
            {
                if (targetTransforms[i] == null)
                {
                    targetTransforms.RemoveAt(i);
                    i--;
                }
            }

            if (targetTransforms.Count > 0 && targetTransform == null)
            {
                targetTransform = targetTransforms[targetTransforms.Count - 1];
                targetTransforms.RemoveAt(targetTransforms.Count - 1);
            }

            if (respawn)
            {
                respawn = false;
                steerableAgent.Spawn((VectorXYZ)spawnLocation);
                steerableAgent.Data.SteeringBehaviours.Clear();
            }

            if (clearSteeringBehaviours)
            {
                clearSteeringBehaviours = false;
                steerableAgent.Data.SteeringBehaviours.Clear();
            }
            
            if (testLinearStop)
            {
                testLinearStop = false;
                linearStop = LinearStop.CreateInstance(steerableAgent.Data);
                steerableAgent.Data.AddSteeringBehaviour(linearStop);
            }
            
            if (removeLinearStopFromSteeringBehaviours)
            {
                removeLinearStopFromSteeringBehaviours = false;
                steerableAgent.Data.RemoveSteeringBehaviour(linearStop);
            }

            if (removeSeekFromSteeringBehaviours)
            {
                removeSeekFromSteeringBehaviours = false;
                steerableAgent.Data.RemoveSteeringBehaviour(seek);
            }

            if (testSeekUsingTargetMovingAgent)
            {
                testSeekUsingTargetMovingAgent = false;
                if (targetMovingAgent != null)
                {
                    steerableAgent.Data.RemoveSteeringBehaviour(seek);
                    seek = Seek.CreateInstance(steerableAgent.Data, targetMovingAgent.Data);
                    steerableAgent.Data.AddSteeringBehaviour(seek);
                }
            }

            if (testSeekUsingTargetTransform)
            {
                testSeekUsingTargetTransform = false;
                if (targetTransform != null)
                {
                    steerableAgent.Data.RemoveSteeringBehaviour(seek);
                    seek = Seek.CreateInstance(steerableAgent.Data, targetTransform);
                    steerableAgent.Data.AddSteeringBehaviour(seek);
                }
            }

            if (testSeekUsingTargetLocation)
            {
                testSeekUsingTargetLocation = false;
                steerableAgent.Data.RemoveSteeringBehaviour(seek);
                seek = Seek.CreateInstance(steerableAgent.Data, targetLocation);
                steerableAgent.Data.AddSteeringBehaviour(seek);
            }
        }
    }
}