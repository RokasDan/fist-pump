using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RokasDan.FistPump.Runtime
{
    // Class for hovering your character controller or any other character.
    // Class inspired by: https://www.youtube.com/watch?v=qdskE8PJy6Q
    public class HoverController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Rigidbody playerRigidbody;

        [Header("Hover Height Controls")]
        [Min(1f)]
        [SerializeField]
        private float detectionHeight = 2.5f;

        [Min(1f)]
        [SerializeField]
        private float rideHeight = 2f;

        [Header("Hover Force Controls")]
        [SerializeField]
        private float rideSpringStrength;

        [SerializeField]
        private float rideSpringDamper;

        [SerializeField]
        private bool deactivateGravityOnHover = false;

        // Bool for deactivation when jumping.
        public bool isHovering = true;

        [FormerlySerializedAs("usePID")]
        [Header("Hover Force Routing")]
        [SerializeField]
        private bool useSpringOnly;


        [SerializeField]
        private bool applyPIDToSpring;

        [SerializeField]
        private bool applyOnlyPID;

        [Header("PID Controls")]
        [SerializeField]
        [Range(-10, 10)]
        private float proportional, integral, derivative;


        // Raycast reference for other raycast applications such as IsGrounded.
        public RaycastHit rayHit;

        // Used for the event to check if the player is grounded or not.
        private int isInAir;

        // Used for the gravity event to check if the player is hovering or not.
        private int isInAirGravity = 1;

        private PID controllerPID;

        // Get the non-mono PID class.
        private void Start()
        {
            controllerPID = new PID(proportional, integral, derivative);
        }

        //Detection height can't be smaller then ride height. This sets it from doing so.
        //Detection for PID routing so two or more variants would not be used.
        private void OnValidate()
        {
            if (detectionHeight < rideHeight)
            {
                detectionHeight = rideHeight;
            }

            if (useSpringOnly)
            {
                applyPIDToSpring = false;
                applyOnlyPID = false;
            }

            if (applyPIDToSpring)
            {
                useSpringOnly = false;
                applyOnlyPID = false;
            }

            if (applyOnlyPID)
            {
                useSpringOnly = false;
                applyPIDToSpring = false;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            PlayerHover();
            GravityOffOnHover();
            HoverDeactivateEvent();
        }

        private void PlayerHover()
        {
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out rayHit, detectionHeight);

            if (rayHit.collider != null && isHovering)
            {
                //Our player velocity and vector direction of the raycast.
                Vector3 rayDirection = transform.TransformDirection(Vector3.down);

                Vector3 playerVelocity = playerRigidbody.velocity;

                // Velocity of foreign bodies our player hits with the raycast.
                Vector3 otherObjectVelocity = Vector3.zero;
                Rigidbody otherHitBody = rayHit.rigidbody;


                float rayDirectionVelocity = Vector3.Dot(rayDirection, playerVelocity);
                float otherDirectionVelocity = Vector3.Dot(rayDirection, otherObjectVelocity);

                float releaseVelocity = rayDirectionVelocity - otherDirectionVelocity;

                float x = rayHit.distance - rideHeight;

                float springForce = (x * rideSpringStrength) - (releaseVelocity * rideSpringDamper);

                // Checking when ever we are using PID to apply force.
                if (applyPIDToSpring)
                {
                    var errorVector = rayDirection * springForce;
                    var error = errorVector.y;
                    var outputPID = controllerPID.GetOutput(error, Time.fixedDeltaTime);

                    Debug.Log(error);

                    //Applying our pid force to the spring force.
                    playerRigidbody.AddForce(new Vector3(0, outputPID, 0));

                    //Drawing the Ray vector of the float mechanism.
                    Debug.DrawLine(transform.position, transform.position + (Vector3.up * outputPID), Color.yellow);

                    // Adding force to other Rigidbodies if hit.
                    if (otherHitBody != null)
                    {
                        otherHitBody.AddForceAtPosition(rayDirection * -springForce, rayHit.point);
                    }
                }

                if (useSpringOnly)
                {
                    //Applying force without PID.
                    playerRigidbody.AddForce(rayDirection * springForce);

                    //Drawing the Ray vector of the float mechanism.
                    Debug.DrawLine(transform.position, transform.position + (rayDirection * springForce), Color.yellow);

                    // Adding force to other Rigidbodies if hit.
                    if (otherHitBody != null)
                    {
                        otherHitBody.AddForceAtPosition(rayDirection * -springForce, rayHit.point);
                    }
                }

                if (applyOnlyPID)
                {
                    var error = rayHit.distance - rideHeight;

                    Debug.Log(error);

                    var outputPID = controllerPID.GetOutput(-error, Time.fixedDeltaTime);

                    //Applying our pid force straight to our rigidbody.
                    playerRigidbody.AddForce(new Vector3(0, outputPID, 0));

                    //Drawing the Ray vector of the float mechanism.
                    Debug.DrawLine(transform.position, transform.position + (Vector3.up * outputPID), Color.yellow);

                    // Adding force to other Rigidbodies if hit.
                    if (otherHitBody != null)
                    {
                        otherHitBody.AddForceAtPosition(rayDirection * -springForce, rayHit.point);
                    }
                }
            }
        }


        // Hover event, turns back on hovering if called.
        private void HoverDeactivateEvent()
        {
            if (rayHit.collider == null && isInAir == 0)
            {
                isInAir = 1;
            }

            if (rayHit.collider != null && isInAir == 1)
            {
                isInAir = 0;
                isHovering = true;
            }
        }

        // Gravity event, turns gravity on and off on hover if used in inspector.
        // This is if you want to exclude gravity forces from the hover application.
        private void GravityOffOnHover()
        {
            if (deactivateGravityOnHover)
            {
                if (rayHit.collider == null && isInAirGravity == 0)
                {
                    isInAirGravity = 1;
                    playerRigidbody.useGravity = true;
                }

                if (rayHit.collider != null && isInAirGravity == 1)
                {
                    isInAirGravity = 0;
                    playerRigidbody.useGravity = false;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.down * detectionHeight);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position - new Vector3(0, rideHeight, 0), 0.2f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position - new Vector3(0, rayHit.distance, 0), 0.2f);
        }
    }
}
