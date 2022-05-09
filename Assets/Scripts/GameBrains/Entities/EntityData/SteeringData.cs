using System.Collections.Generic;
using GameBrains.Actuators.Motion.Steering;
using GameBrains.Extensions.Vectors;
using UnityEngine;

namespace GameBrains.Entities.EntityData
{
    [System.Serializable]
    public class SteeringData : KinematicData
    {
        #region Accumulated data used to aggregate steering outputs

        VectorXZ accumulatedVelocity;
        public VectorXZ AccumulatedVelocity => accumulatedVelocity;

        VectorXZ accumulatedAcceleration;
        public VectorXZ AccumulatedAcceleration => accumulatedAcceleration;

        float accumulatedAngularVelocity;
        public float AccumulatedAngularVelocity => accumulatedAngularVelocity;

        float accumulatedAngularAcceleration;
        public float AccumulatedAngularAcceleration => accumulatedAngularAcceleration;

        int accumulationVelocityCount;
        int accumulationAngularVelocityCount;
        int accumulationAccelerationCount;
        int accumulationAngularAccelerationCount;

        bool doApplyAccumulatedVelocities;
        public bool DoApplyAccumulatedVelocities => doApplyAccumulatedVelocities;

        #endregion

        #region Creators

        public static SteeringData CreateSteeringDataInstance(Transform t)
        {
            SteeringData steeringData = CreateInstance<SteeringData>();
            InitializeSteeringData(t, steeringData);
            return steeringData;
        }

        protected static void InitializeSteeringData(Transform t, SteeringData steeringData)
        {
            InitializeKinematicData(t, steeringData);
            steeringData.SteeringBehaviours = new Dictionary<int, SteeringBehaviour>();
            steeringData.ResetSteeringData();
        }
        
        public void ResetSteeringData()
        {
            ResetAccumulatedData();
            doApplyAccumulatedVelocities = false;
            SteeringBehaviours.Clear();
        }

        #endregion Creators

        #region Casting

        public new SteerableAgent Owner => owner as SteerableAgent;

        public static implicit operator SteeringData(Transform t)
        {
            return CreateSteeringDataInstance(t);
        }

        #endregion Casting

        #region Steering
        
        public enum CombiningMethods
        {
            Weighted,
            Prioritized,
            Dithered
        }
        
        public CombiningMethods CombiningMethod { get; set; } = CombiningMethods.Weighted;

        #region Steering Behaviours
        
        public Dictionary<int, SteeringBehaviour> SteeringBehaviours { get; protected set; }

        public int AddSteeringBehaviour(SteeringBehaviour steeringBehaviour)
        {
            if (steeringBehaviour == null) { return -1; }

            if (!SteeringBehaviours.ContainsKey(steeringBehaviour.ID))
            {
                SteeringBehaviours.Add(steeringBehaviour.ID, steeringBehaviour);
            }
            return steeringBehaviour.ID;
        }

        public void RemoveSteeringBehaviour(SteeringBehaviour steeringBehaviour)
        {
            if (steeringBehaviour != null)
            {
                SteeringBehaviours.Remove(steeringBehaviour.ID);
            }
        }

        public void RemoveSteeringBehaviour(int id)
        {
            SteeringBehaviours.Remove(id);
        }

        public bool GetSteeringBehaviour(int id, out SteeringBehaviour steeringBehaviour)
        {
            return SteeringBehaviours.TryGetValue(id, out steeringBehaviour);
        }
        
        #endregion Steering Behaviours

        public void CalculateSteering(CombiningMethods combiningMethod = CombiningMethods.Weighted)
        {
            switch (combiningMethod)
            {
                case CombiningMethods.Weighted: CalculateWeightedSteering();
                    break;

                case CombiningMethods.Prioritized: CalculatePrioritizedSteering();
                    break;

                case CombiningMethods.Dithered: CalculateDitheredSteering();
                    break;
            }
        }

        public void CalculateWeightedSteering()
        {
            foreach (var steeringBehaviour in SteeringBehaviours.Values)
            {
                AccumulateSteeringOutput(steeringBehaviour.Steer());
            }
        }

        public void CalculatePrioritizedSteering()
        {
            Log.Debug("CalculatePrioritizedSteering not implemented");
        }

        public void CalculateDitheredSteering()
        {
            Log.Debug("CalculateDitheredSteering not implemented");
        }
        
        public void SetSteeringOutput(SteeringOutput steeringOutput)
        {
            switch (steeringOutput.Type)
            {
                case SteeringOutput.Types.Velocities:
                    SetAccumulatedVelocities(steeringOutput.Linear, steeringOutput.Angular);
                    break;
                case SteeringOutput.Types.Accelerations:
                    SetAccumulatedAccelerations(steeringOutput.Linear, steeringOutput.Angular);
                    break;
                case SteeringOutput.Types.Forces:
                    SetAccumulatedForces(steeringOutput.Linear, steeringOutput.Angular);
                    break;
            }
        }

        public void AccumulateSteeringOutput(SteeringOutput steeringOutput)
        {
            switch (steeringOutput.Type)
            {
                case SteeringOutput.Types.Velocities:
                    AccumulateVelocities(steeringOutput.Linear, steeringOutput.Angular);
                    break;
                case SteeringOutput.Types.Accelerations:
                    AccumulateAccelerations(steeringOutput.Linear, steeringOutput.Angular);
                    break;
                case SteeringOutput.Types.Forces:
                    AccumulateForces(steeringOutput.Linear, steeringOutput.Angular);
                    break;
            }
        }

        #region Accumulate data
        public void SetAccumulatedVelocity(VectorXZ velocityToAccumulate)
        {
            doApplyAccumulatedVelocities = true;

            accumulatedVelocity = velocityToAccumulate;

            accumulationVelocityCount++;
        }

        public void SetAccumulatedAngularVelocity(float angularVelocityToAccumulate)
        {
            doApplyAccumulatedVelocities = true;

            accumulatedAngularVelocity = angularVelocityToAccumulate;

            accumulationAngularVelocityCount++;
        }

        public void SetAccumulatedVelocities(
            VectorXZ velocityToAccumulate,
            float angularVelocityToAccumulate)
        {
            doApplyAccumulatedVelocities = true;

            accumulatedVelocity = velocityToAccumulate;
            accumulatedAngularVelocity = angularVelocityToAccumulate;

            accumulationVelocityCount++;
            accumulationAngularVelocityCount++;
        }

        public void AccumulateVelocities(
            VectorXZ velocityToAccumulate,
            float angularVelocityToAccumulate)
        {
            doApplyAccumulatedVelocities = true;

            accumulatedVelocity = AccumulatedVelocity + velocityToAccumulate;
            accumulatedAngularVelocity
                = AccumulatedAngularVelocity + angularVelocityToAccumulate;

            accumulationVelocityCount++;
            accumulationAngularVelocityCount++;
        }
        public void SetAccumulatedAccelerations(
            VectorXZ accelerationToAccumulate,
            float angularAccelerationToAccumulate)
        {
            accumulatedAcceleration = accelerationToAccumulate;
            accumulatedAngularAcceleration = angularAccelerationToAccumulate;

            accumulationAccelerationCount++;
            accumulationAngularAccelerationCount++;
        }

        public void AccumulateAccelerations(
            VectorXZ accelerationToAccumulate,
            float angularAccelerationToAccumulate)
        {
            accumulatedAcceleration = AccumulatedAcceleration + accelerationToAccumulate;
            accumulatedAngularAcceleration
                = AccumulatedAngularAcceleration + angularAccelerationToAccumulate;

            accumulationAccelerationCount++;
            accumulationAngularAccelerationCount++;
        }

        public void SetAccumulatedForces(
            VectorXZ forcesToAccumulate,
            float angularForcesToAccumulate)
        {
            Log.Debug("SetAccumulatedForces not implemented.");
        }

        public void AccumulateForces(
            VectorXZ forcesToAccumulate,
            float angularForcesToAccumulate)
        {
            Log.Debug("AccumulateForces not implemented.");
        }

        #endregion

        #endregion Steering

        #region Do Update

        // TODO: Do we need an option to use fixed update?

        public override void DoUpdate(float deltaTime, bool updatePositionAndOrientation = true)
        {
            ApplyAccumulatedVelocities();

            CalculateAcceleration();
            CalculateAngularAcceleration();

            base.DoUpdate(deltaTime, updatePositionAndOrientation);

            ResetAccumulatedData();
        }

        public void ApplyAccumulatedVelocities()
        {
            if (DoApplyAccumulatedVelocities)
            {
                if (accumulationVelocityCount > 0)
                {
                    Velocity += AccumulatedVelocity / accumulationVelocityCount;
                }

                if (accumulationAngularVelocityCount > 0)
                {
                    AngularVelocity += AccumulatedAngularVelocity / accumulationAngularVelocityCount;
                }

                doApplyAccumulatedVelocities = false;
            }
        }

        public void CalculateAcceleration()
        {
            if (accumulationAccelerationCount > 0)
            {
                Acceleration += AccumulatedAcceleration / accumulationAccelerationCount;
            }
        }

        public void CalculateAngularAcceleration()
        {
            if (accumulationAngularAccelerationCount > 0)
            {
                AngularAcceleration += AccumulatedAngularAcceleration / accumulationAngularAccelerationCount;
            }
        }

        public void ResetAccumulatedData()
        {
            accumulatedVelocity = VectorXZ.zero;
            accumulatedAngularVelocity = 0;
            accumulatedAcceleration = VectorXZ.zero;
            accumulatedAngularAcceleration = 0;

            accumulationVelocityCount = 0;
            accumulationAngularVelocityCount = 0;
            accumulationAccelerationCount = 0;
            accumulationAngularAccelerationCount = 0;
        }

        #endregion Do Update
    }
}