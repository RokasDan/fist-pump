using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    // Class for hovering your character controller or any other character.
    // Class inspired by: https://www.youtube.com/watch?v=qdskE8PJy6Q
    internal sealed class HoverController : MonoBehaviour
    {
        // Method for adding hover force to this game object.
        public void Hover(Rigidbody targetBody, float force)
        {
            // Our player velocity and vector direction of the raycast.
            Vector3 rayDirection = transform.TransformDirection(Vector3.down);

            // Adding force to Y for hover.
            targetBody.AddForce(rayDirection * force);
        }

        // Method which pushes other rigid body objects if hovering on them.
        public void PushDown(RaycastHit rayHit, float force)
        {
            // Getting the rigid body of the hit object.
            Rigidbody otherHitBody = rayHit.rigidbody;

            // Our player velocity and vector direction of the raycast.
            Vector3 rayDirection = transform.TransformDirection(Vector3.down);

            if (otherHitBody != null)
            {
                otherHitBody.AddForceAtPosition(rayDirection * -force, rayHit.point);
            }
        }
    }
}
