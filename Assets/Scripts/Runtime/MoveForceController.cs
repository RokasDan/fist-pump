using UnityEngine;


namespace RokasDan.FistPump.Runtime
{
    // Class for adding force to the move vector. Specifically for player moving around.
    // Class inspired by: https://www.youtube.com/watch?v=qdskE8PJy6Q
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

        private bool onGround;
        private bool isSprinting;


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
                objectRigidbody.drag = groundDrag;

                // Checking if we are using sprint modifiers while on ground.
                if (isSprinting)
                {
                    LerpDirection(moveDirection, groundSpeed * sprintModifier,
                        groundAcceleration * sprintAcceleration);
                }
                else
                {
                    LerpDirection(moveDirection, groundSpeed,
                        groundAcceleration);
                }
            }
            else
            {
                if (objectRigidbody.velocity.magnitude > airSpeed)
                {
                    LerpDirection(moveDirection, objectRigidbody.velocity.magnitude,
                        airAcceleration);
                    
                }
                else
                {
                    LerpDirection(moveDirection, airSpeed,
                        airAcceleration);
                }
            }
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
            // When not moving we are gradually stopping the object. The if is for air or ground locomotion.
            if (!onGround) return;
            objectRigidbody.drag = groundDrag;

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
