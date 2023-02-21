using UnityEngine;


namespace RokasDan.FistPump.Runtime
{
    // Class for hovering your character controller or any other character.
    // Class inspired by: https://www.youtube.com/watch?v=qdskE8PJy6Q
    internal sealed class HoverController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private Rigidbody targetRigidbody;

        [SerializeField]
        private GroundCheck groundCheck;

        [Header("Forces")]
        [SerializeField]
        private PIDForce pidForce;

        [SerializeField]
        private SpringForce springForce;

        [Header("Hover Controllers")]
        [SerializeField]
        private float hoverHeight;

        [SerializeField]
        private bool disableGravityOnHover;

        // Choose force type for hovering.
        private enum HoverType
        {
            SpringForce,
            PIDForce
        }

        [SerializeField]
        private HoverType hoverType;

        // Choose force type for pushing down objects on which object is hovering.
        private enum PushDownForceType
        {
            SpringForce,
            PIDForce
        }

        [SerializeField]
        private PushDownForceType pushDownForceType;
        // This doesn't work with private, gets hidden in instructor.

        public bool IsOverHoverThreshold { get; private set; }
        public bool IsInHoveringRange { get; private set; }

        private void OnDrawGizmos()
        {
            var position = transform.position;
            // Ride-Hover height sphere.
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(position - new Vector3(0, hoverHeight, 0), 0.3f);
        }


        public void UpdateHover()
        {
            // Checking if we are bellow ride height when
            // hitting something.
            if (IsInHoveringRange)
            {
                HoverHeightThreshold();
            }

            // Disabling gravity if the user wants.
            if (disableGravityOnHover && IsInHoveringRange && IsOverHoverThreshold)
            {
                targetRigidbody.useGravity = false;
            }
            else
            {
                targetRigidbody.useGravity = true;
            }

            // If we are hitting our raycast and are bellow hover ride height
            // we start to hover.
            if (IsInHoveringRange && IsOverHoverThreshold)
            {
                // Choosing a hover type and applying hovering forces.
                if (hoverType == HoverType.SpringForce)
                {
                    var position = transform.position;
                    var hoverForce = springForce.GetOutput(
                        groundCheck.RayHit.rigidbody,
                        targetRigidbody,
                        groundCheck.RayHit.distance,
                        hoverHeight);

                    Hover(targetRigidbody, hoverForce);
                    Debug.DrawLine(position, position + (Vector3.down * hoverForce), Color.yellow);
                }
                else
                {
                    var position = transform.position;
                    var hoverForce = pidForce.GetOutput(
                        Time.deltaTime,
                        hoverHeight,
                        groundCheck.RayHit.distance);

                    Hover(targetRigidbody, hoverForce);
                    Debug.DrawLine(position, position + (Vector3.down * hoverForce), Color.red);
                }

                // Pushing objects down if they have a rigidbody.
                UpdateDownForce();
            }
        }

        private void UpdateDownForce()
        {
            // Choosing push type and pushing down rigid bodies if hovering on them.
            if (pushDownForceType == PushDownForceType.SpringForce)
            {
                var hoverForce = springForce.GetOutput(
                    groundCheck.RayHit.rigidbody,
                    targetRigidbody,
                    groundCheck.RayHit.distance,
                    hoverHeight);

                PushDown(groundCheck.RayHit, hoverForce);
            }
            else
            {
                var hoverForce = pidForce.GetOutput(
                    Time.deltaTime,
                    hoverHeight,
                    groundCheck.RayHit.distance);

                PushDown(groundCheck.RayHit, hoverForce);
            }
        }

        // Resetting hover bool, we want to reset this on the raycast enter event.
        public void HoverReset()
        {
            IsInHoveringRange = true;
        }

        public void HoverOff()
        {
            IsInHoveringRange = false;
        }

        public void HoverHeightThresholdReset()
        {
            // Setting air locomotion, useful when player just rides off cliffs and drops.
            IsOverHoverThreshold = false;
        }

        // Method for turning hovering and ground locomotion if jump failed and hit something
        // before exiting ground check Ray.
        public void ObjectHitTop()
        {
            if (IsInHoveringRange == false &&
                groundCheck.IsGrounded &&
                targetRigidbody.velocity.y <= 0)
            {
                IsInHoveringRange = true;
            }
        }


        // Method for adding hover force to this game object.
        private void Hover(Rigidbody targetBody, float force)
        {
            // Our player velocity and vector direction of the raycast.
            Vector3 rayDirection = transform.TransformDirection(Vector3.down);

            // Adding force to Y for hover.
            targetBody.AddForce(rayDirection * force);
        }

        // Method which pushes other rigid body objects if hovering on them.
        private void PushDown(RaycastHit rayHit, float force)
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

        private void HoverHeightThreshold()
        {
            var startThreshold = groundCheck.RayHit.distance - hoverHeight;
            if (-startThreshold >= 0)
            {
                IsOverHoverThreshold = true;
                // Check if we want to turn of gravity on hover, if so we turn it off.
            }
        }
    }
}
