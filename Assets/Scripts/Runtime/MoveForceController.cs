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

        [Header("Multipliers")]
        [Min(1f)]
        [SerializeField]
        private float speedMultiplier = 10f;

        // While object is on ground.
        [Header("Ground Locomotion")]
        [Min(0f)]
        [SerializeField]
        private float groundMoveSpeed = 6f;

        [SerializeField]
        private float groundDrag = 6f;

        // While object is in air.
        [Header("Air Locomotion")]
        [Min(0f)]
        [SerializeField]
        private float airMoveSpeed = 6f;

        [SerializeField]
        private float airDrag = 6f;

        private bool onGround;

        // The main method of the class which will calculate and add velocity to the rigid body of the object.
        public void AddMoveForce(Vector3 moveDirection)
        {
            var groundSpeed = groundMoveSpeed * speedMultiplier;
            var airSpeed = airMoveSpeed * speedMultiplier;
            if (onGround)
            {
                objectRigidbody.AddForce(moveDirection * groundSpeed, ForceMode.Acceleration);
            }
            else
            {
                objectRigidbody.AddForce(moveDirection * airSpeed, ForceMode.Acceleration);
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
    }
}
