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

        // While object is in air.
        [Header("Air Locomotion")]
        [SerializeField]
        private float airSpeed;

        [SerializeField]
        private float airAcceleration;

        [SerializeField]
        private float airDeceleration;

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
            //objectRigidbody.drag = groundDrag;
        }

        // Method should be placed when jump is successful.
        public void LocomotionAir()
        {
            onGround = false;
            //objectRigidbody.drag = airDrag;
        }

        public void LerpObjectVelocity(Vector3 moveDirection)
        {
            if (onGround)
            {
                objectRigidbody.drag = groundDrag;

                if (isSprinting)
                {
                    Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                        moveDirection * (groundSpeed * sprintModifier), groundAcceleration * Time.fixedDeltaTime);
                    desiredMoveDirection.y = objectRigidbody.velocity.y;

                    objectRigidbody.velocity = desiredMoveDirection;
                }
                else
                {
                    Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                        moveDirection * groundSpeed, groundAcceleration * Time.fixedDeltaTime);
                    desiredMoveDirection.y = objectRigidbody.velocity.y;

                    objectRigidbody.velocity = desiredMoveDirection;
                }
            }
            else
            {
                objectRigidbody.drag = airDrag;

                if (isSprinting)
                {
                    Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                        moveDirection * (airSpeed * sprintModifier), airAcceleration * Time.fixedDeltaTime);
                    desiredMoveDirection.y = objectRigidbody.velocity.y;

                    objectRigidbody.velocity = desiredMoveDirection;
                }
                else
                {
                    Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity,
                        moveDirection * airSpeed, airAcceleration * Time.fixedDeltaTime);
                    desiredMoveDirection.y = objectRigidbody.velocity.y;

                    objectRigidbody.velocity = desiredMoveDirection;
                }
            }
        }

        public void StopObject()
        {
            if (onGround)
            {
                objectRigidbody.drag = groundDrag;

                Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity, Vector3.zero, groundDeceleration * Time.fixedDeltaTime);
                desiredMoveDirection.y = objectRigidbody.velocity.y;

                objectRigidbody.velocity = desiredMoveDirection;
            }
            else
            {
                objectRigidbody.drag = airDrag;

                Vector3 desiredMoveDirection = Vector3.Lerp(objectRigidbody.velocity, Vector3.zero, airDeceleration * Time.fixedDeltaTime);
                desiredMoveDirection.y = objectRigidbody.velocity.y;

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
