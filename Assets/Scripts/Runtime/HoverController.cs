using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class HoverController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Rigidbody playerRigidbody;

        [Header("Float Controls")]
        [SerializeField]
        private float rideHeight;

        [SerializeField]
        private float rideSpringStrength;

        [SerializeField]
        private float rideSpringDamper;

        [Min(1f)]
        [SerializeField]
        private float bellowRideHeightRay = 1f;

        public RaycastHit rayHit;


        // private Vector3 otherObjectVelocity = Vector3.zero;

        // Rigidbody otherObjectBody = rayHit.rigidbody;

        // Update is called once per frame
        void Update()
        {
            PlayerHover();
        }

        private void PlayerHover()
        {
            Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out rayHit, rideHeight * bellowRideHeightRay);

            if (rayHit.collider != null)
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, Vector3.down * rideHeight);

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position - new Vector3(0, rideHeight, 0), 0.3f);
        }
    }
}
