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

        public float GetOutput(Rigidbody hoverSurface, Rigidbody targetBody, float rayHitDistance, float rideHeight)
        {
            //Our player velocity and vector direction of the raycast.
            Vector3 playerVelocity = targetBody.velocity;
            Vector3 rayDirection = transform.TransformDirection(Vector3.down);

            // Velocity of foreign bodies our player hits with the raycast.
            Vector3 otherObjectVelocity = Vector3.zero;

            // Considering the other body velocity when we hitting it.
            // Can I make this default ?
            if (hoverSurface != null)
            {
                otherObjectVelocity = hoverSurface.velocity;
            }

            float rayDirectionVelocity = Vector3.Dot(rayDirection, playerVelocity);
            float otherDirectionVelocity = Vector3.Dot(rayDirection, otherObjectVelocity);

            float releaseVelocity = rayDirectionVelocity - otherDirectionVelocity;

            float x = rayHitDistance - rideHeight;

            float springForce = (x * rideSpringStrength) - (releaseVelocity * rideSpringDamper);

            return springForce;
        }
    }
}
