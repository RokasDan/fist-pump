using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RokasDan.FistPump.Runtime
{
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

        // Bool for deactivation when jumping.
        public bool isHovering = true;

        // Raycast reference for other raycast applications such as IsGrounded.
        public RaycastHit rayHit;

        private int isInAir;


        //Detection height can't be smaller then ride height. This sets it from doing so.

        private void OnValidate()
        {
            if (detectionHeight < rideHeight)
            {
                detectionHeight = rideHeight;
            }
        }

        // Update is called once per frame
        void Update()
        {
            PlayerHover();
            HoverDeactivateEvent();
        }

        private void PlayerHover()
        {
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out rayHit, detectionHeight);

            if (rayHit.collider != null && isHovering)
            {
                Debug.Log("I hit something");
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

                //Drawing the Ray vector of the float mechanism.
                Debug.DrawLine(transform.position, transform.position + (rayDirection * springForce), Color.yellow);

                playerRigidbody.AddForce(rayDirection * springForce);

                if (otherHitBody != null)
                {
                    otherHitBody.AddForceAtPosition(rayDirection * -springForce, rayHit.point);
                }
            }
        }

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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.down * detectionHeight);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position - new Vector3(0, rideHeight, 0), 0.2f);
        }
    }
}
