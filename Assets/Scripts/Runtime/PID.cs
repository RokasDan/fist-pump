using System;
using UnityEngine;
using System.Collections;

namespace RokasDan.FistPump.Runtime
{
    [Serializable]
    public class PID
    {
        private float proportional, integral, derivative;
        private float kProportional, kIntegral, kDerivative;
        private float previousError;


        // Constant proportion
        public float Kp
        {
            get
            {
                return kProportional;
            }
            set
            {
                kProportional = value;
            }
        }

        // Constant integral

        public float Ki
        {
            get
            {
                return kIntegral;
            }
            set
            {
                kIntegral = value;
            }
        }

        // Constant derivative

        public float Kd
        {
            get
            {
                return kDerivative;
            }
            set
            {
                kDerivative = value;
            }
        }

        public PID(float p, float i, float d)
        {
            kProportional = p;
            kIntegral = i;
            kDerivative = d;
        }

        // Based on the code from Brian-Stone on the Unity forums
        // https://forum.unity.com/threads/rigidbody-lookat-torque.146625/#post-1005645

        public float GetOutput(float currentError, float deltaTime)
        {
            proportional = currentError;
            integral += proportional * deltaTime;
            derivative = (proportional - previousError) / deltaTime;
            previousError = currentError;

            return proportional * Kd + integral * Ki + derivative * Kd;
        }
    }
}
