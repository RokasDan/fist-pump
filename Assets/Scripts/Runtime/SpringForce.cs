using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class SpringForce : MonoBehaviour
    {
        [Header("Spring Force Controls")]
        [SerializeField]
        private float rideSpringStrength;

        [SerializeField]
        private float rideSpringDamper;

        public float GetOutput(RaycastHit rayHit, Rigidbody targetBody, float rideHeight)
        {
            //Our player velocity and vector direction of the raycast.
            Vector3 playerVelocity = targetBody.velocity;
            Vector3 rayDirection = transform.TransformDirection(Vector3.down);

            // Velocity of foreign bodies our player hits with the raycast.
            Vector3 otherObjectVelocity = Vector3.zero;
            Rigidbody otherHitBody = rayHit.rigidbody;

            // Considering the other body velocity when we hitting it.
            if (otherHitBody != null)
            {
                otherObjectVelocity = otherHitBody.velocity;
            }

            float rayDirectionVelocity = Vector3.Dot(rayDirection, playerVelocity);
            float otherDirectionVelocity = Vector3.Dot(rayDirection, otherObjectVelocity);

            float releaseVelocity = rayDirectionVelocity - otherDirectionVelocity;

            float x = rayHit.distance - rideHeight;

            float springForce = (x * rideSpringStrength) - (releaseVelocity * rideSpringDamper);

            return springForce;
        }
    }
}
