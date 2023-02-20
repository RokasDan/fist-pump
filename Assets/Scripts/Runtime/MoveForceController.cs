using System;
using UnityEngine;
using UnityEngine.Serialization;


namespace RokasDan.FistPump.Runtime
{
    // Class for adding force to the move vector. Specifically for player moving around.
    public class MoveForceController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private Rigidbody objectRigidbody;

        [Header("Modifiers")]
        [SerializeField]
        private float sprintModifier;

        [SerializeField]
        private float sprintAcceleration;

        [Header("Air Control")]
        // While object is in air.
        [SerializeField]
        private float airSpeed;

        [SerializeField]
        private float airAcceleration;

        [SerializeField]
        private float airDrag;

        [FormerlySerializedAs("ratioAirAcceleration")]
        [Header("Air Ratio")]
        [SerializeField]
        private bool useRatioAirAcceleration;

        [Range(0.1f, 1f)]
        [SerializeField]
        private float airRatioStrength = 0.1f;

        [Min(0)]
        [SerializeField]
        private float airMinAcceleration;

        // While object is on ground.
        [Header("Ground Locomotion")]
        [SerializeField]
        private float groundSpeed;

        [SerializeField]
        private float groundAcceleration;

        [SerializeField]
        private float groundDeceleration;

        [SerializeField]
        private float groundDrag;

        // Ratio for when user is running
        [Header("Ground Ratio")]
        [SerializeField]
        private bool useRatioGroundAcceleration;

        [Range(0.1f, 1f)]
        [SerializeField]
        private float groundRatioStrength = 0.1f;

        [Min(0)]
        [SerializeField]
        private float groundMinAcceleration;

        // Ratio for when user is not moving but is pushed regardless.
        [Header("Stop Ratio")]
        [SerializeField]
        private bool useRatioStopAcceleration;

        [Range(0.1f, 1f)]
        [SerializeField]
        private float stopRatioStrength = 0.1f;

        [Min(0)]
        [SerializeField]
        private float stopMinDeceleration;

        private bool onGround;
        private bool isSprinting;

        // Making sure ratio min accelerations are not bigger than actual accelerations.
        private void OnValidate()
        {
            if (groundMinAcceleration > groundAcceleration)
            {
                groundMinAcceleration = groundAcceleration;
            }

            if (airMinAcceleration > airAcceleration)
            {
                airMinAcceleration = airAcceleration;
            }

            if (stopMinDeceleration > groundDeceleration)
            {
                stopMinDeceleration = groundDeceleration;
            }
        }

        // Method which switches between ground and air locomotion. Should be placed
        // within the ground check of the player controller when hover starts.
        public void LocomotionGround()
        {
            onGround = true;
            objectRigidbody.drag = groundDrag;
        }

        // Method should be placed when jump is successful.
        public void LocomotionAir()
        {
            onGround = false;
            objectRigidbody.drag = airDrag;
        }

        // Method which uses Velocity and Lerp.
        public void LerpObjectVelocity(Vector3 moveDirection)
        {
            if (onGround)
            {
                // Ground Section of moving.
                objectRigidbody.drag = groundDrag;

                // Checking if we are using sprint modifiers while on ground.
                if (isSprinting)
                {
                    // Checking if current velocity is not bigger due to external force then
                    // our set speed with modifiers.
                    if (objectRigidbody.velocity.magnitude > groundSpeed * sprintModifier)
                    {
                        // Looking if we want to apply ratio to the acceleration when current velocity exceeds
                        // top speed due to external force.
                        if (useRatioGroundAcceleration)
                        {
                            // Getting our adjusted acceleration with sprint modifier values.
                            float adjustedAcceleration = AccelerationRatio(groundRatioStrength,
                                groundMinAcceleration,
                                groundSpeed * sprintModifier,
                                groundAcceleration * sprintAcceleration);

                            // Applying the current velocity as speed to our lerp function with adjusted acceleration of added sprint values.
                            LerpDirection(
                                moveDirection,
                                objectRigidbody.velocity.magnitude,
                                adjustedAcceleration);
                        }
                        else
                        {
                            // Applying the current velocity as speed to our lerp function with normal acceleration.
                            // This will only be active is you dont use ratio for acceleration.
                            LerpDirection(
                                moveDirection,
                                objectRigidbody.velocity.magnitude,
                                groundAcceleration * sprintAcceleration);
                        }
                    }
                    else
                    {
                        // Applying sprint as normal.
                        LerpDirection(
                            moveDirection,
                            groundSpeed * sprintModifier,
                            groundAcceleration * sprintAcceleration);
                    }
                }
                else
                {
                    // Movement without the sprint modifiers.

                    if (objectRigidbody.velocity.magnitude > groundSpeed)
                    {
                        // Looking if we want to apply ratio to the acceleration when current velocity exceeds
                        // top speed due to external force.
                        if (useRatioGroundAcceleration)
                        {
                            // Getting our adjusted acceleration.
                            float adjustedAcceleration = AccelerationRatio(groundRatioStrength,
                                groundMinAcceleration,
                                groundSpeed,
                                groundAcceleration);

                            // Applying the current velocity as speed to our lerp function with adjusted acceleration.
                            LerpDirection(
                                moveDirection,
                                objectRigidbody.velocity.magnitude,
                                adjustedAcceleration);
                        }
                        else
                        {
                            // Applying the current velocity as speed to our lerp function with normal acceleration.
                            // This is active only if ratio off acceleration is not on.
                            LerpDirection(
                                moveDirection,
                                objectRigidbody.velocity.magnitude,
                                groundAcceleration);
                        }
                    }
                    else
                    {
                        // Applying ground speed and acceleration as normal if other conditions ar not met.
                        LerpDirection(
                            moveDirection,
                            groundSpeed,
                            groundAcceleration);
                    }
                }
            }
            else
            {
                // Air section of movement!


                if (objectRigidbody.velocity.magnitude > airSpeed)
                {
                    // Looking if we want to apply ratio to the acceleration when current velocity exceeds
                    // top speed due to external force.
                    if (useRatioAirAcceleration)
                    {
                        // Getting our adjusted acceleration.
                        float adjustedAcceleration = AccelerationRatio(airRatioStrength,
                            airMinAcceleration,
                            airSpeed,
                            airAcceleration);

                        // Applying the current velocity as speed to our lerp function with adjusted acceleration.
                        LerpDirection(
                            moveDirection,
                            objectRigidbody.velocity.magnitude,
                            adjustedAcceleration);
                    }
                    else
                    {
                        // Applying the current velocity as speed to our lerp function with normal acceleration.
                        // This will only work if you are not using acceleration ratio.
                        LerpDirection(
                            moveDirection,
                            objectRigidbody.velocity.magnitude,
                            airAcceleration);
                    }
                }
                else
                {
                    // Applying our normal air max speed and acceleration.
                    LerpDirection(
                        moveDirection,
                        airSpeed,
                        airAcceleration);
                }
            }
        }

        // A method which modifies the acceleration based on the over flow of speed.
        private float AccelerationRatio(float ratioStrength, float minAcceleration, float maxSpeed, float currentAcceleration)
        {
            // Getting our ratio between current and max speed. Here we can set how strong we apply the ratio
            // also we can set a safe guard as minimum Acceleration.
            float speedRatio = Math.Clamp(maxSpeed / objectRigidbody.velocity.magnitude, minAcceleration, ratioStrength);

            // Adjusting our acceleration.
            float adjustedAcceleration = currentAcceleration * speedRatio;

            return adjustedAcceleration;
        }

        private void LerpDirection(Vector3 moveDirection, float speed, float acceleration)
        {
            // Calculating the vectors to which we will be lerp.
            var desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                moveDirection * speed,
                acceleration
                * Time.fixedDeltaTime);

            // Making sure we dont override our gravity effect.
            var velocity = objectRigidbody.velocity;
            desiredMoveDirection.y = velocity.y;

            // Moving our object with lerp vectors.
            velocity = desiredMoveDirection;
            objectRigidbody.velocity = velocity;

            // Drawing our velocity vector
            var draw = velocity;
            draw.y = 0;
            var position = transform.position;
            Debug.DrawLine(position, position + draw, Color.green);
        }

        public void StopObject()
        {
            // When not moving we are gradually stopping the object. This works only when on ground.
            if (!onGround) return;
            objectRigidbody.drag = groundDrag;


            // Applying ratio to acceleration if we are using ground acceleration ratio.
            // This is controlled by ground acceleration ratio controls.
            if (objectRigidbody.velocity.magnitude > groundSpeed && useRatioStopAcceleration)
            {
                var adjustedAcceleration = AccelerationRatio(
                    stopRatioStrength,
                    stopMinDeceleration,
                    groundSpeed,
                    groundDeceleration);

                // Getting lerp vectors for a ground stop.
                Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                    Vector3.zero,
                    adjustedAcceleration
                    * Time.fixedDeltaTime);

                // Making sure lerp vectors dont affect gravity.
                desiredMoveDirection.y = objectRigidbody.velocity.y;

                // Moving our object to stop with Lerp vectors.
                objectRigidbody.velocity = desiredMoveDirection;
            }
            else
            {
                // Getting lerp vectors for a ground stop.
                Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                    Vector3.zero,
                    groundDeceleration
                    * Time.fixedDeltaTime);

                // Making sure lerp vectors dont affect gravity.
                desiredMoveDirection.y = objectRigidbody.velocity.y;

                // Moving our object to stop with Lerp vectors.
                objectRigidbody.velocity = desiredMoveDirection;
            }
        }

        public void AddSprintForce()
        {
            isSprinting = true;
        }

        public void StopSprintForce()
        {
            isSprinting = false;
        }
    }
}
