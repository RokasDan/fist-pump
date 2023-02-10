using System;
using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    // Stand Alone PID controller class meant to smother out forces applied to rigid body.
    // Code from: https://github.com/vazgriz/PID_Controller/blob/master/Assets/Scripts/PID_Controller.cs
    public class PIDForce : MonoBehaviour
    {
        [Header("PID coefficients")]
        [SerializeField]
        [Range(0, 10)]
        private float proportionalGain;

        [SerializeField]
        [Range(0, 10)]
        private float integralGain;

        [SerializeField]
        [Range(0, 10)]
        private float derivativeGain;

        // Variable used in clamping integralGain. This will help to manage integral windup.
        [SerializeField]
        [Range(0, 10)]
        private float integralSaturation;

        [SerializeField]
        [Min(1f)]
        private float power;

        public enum DerivativeMeasurement
        {
            Velocity,
            ErrorRateOfChange
        }

        public DerivativeMeasurement derivativeMeasurement;

        private float integrationStored;
        private float errorLast;
        private float valueLast;
        private bool derivativeInitialized;
        private readonly float outputMin = -1;
        private readonly float outputMax = 1;

        // Get PID value.
        public float GetOutput(float deltaTime, float currentValue, float targetValue)
        {
            if (deltaTime <= 0) throw new ArgumentOutOfRangeException(nameof(deltaTime));

            float error = targetValue - currentValue;

            // Calculate P Term.
            float P = proportionalGain * error;

            // Calculate I Term.
            // Mathf clamp is used to mitigate integral windup.
            integrationStored = Mathf.Clamp(integrationStored + (error * deltaTime), -integralSaturation, integralSaturation);
            float I = integralGain * integrationStored;

            // Calculate both D Terms.
            float errorRateOfChange = (error - errorLast) / deltaTime;
            errorLast = error;

            float valueRateOfChange = (currentValue - valueLast) / deltaTime;
            valueLast = currentValue;

            //choose D term to use to prevent overshoot.
            float deriveMeasure = 0;

            if (derivativeInitialized)
            {
                if (derivativeMeasurement == DerivativeMeasurement.Velocity)
                {
                    deriveMeasure = -valueRateOfChange;
                }
                else
                {
                    deriveMeasure = errorRateOfChange;
                }
            }
            else
            {
                derivativeInitialized = true;
            }

            float D = derivativeGain * deriveMeasure;

            float result = P + I + D;

            return Mathf.Clamp(result, outputMin, outputMax) * power;
        }
    }
}
