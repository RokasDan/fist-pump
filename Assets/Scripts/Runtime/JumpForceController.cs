using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class JumpForceController : MonoBehaviour
    {
        [Header("Jumping")]
        [Min(0f)]
        [SerializeField]
        private float jumpSpeed = 20f;

        [SerializeField]
        private int jumpNumber;

        private int currentJumpNumber;

        public void AddJumpForce(Rigidbody objectRigidbody)
        {
            // Applying force.
            var currentVelocity = objectRigidbody.velocity;

            //Zeroing up velocity to make jumps consistent.
            currentVelocity.y = 0;

            //Adding our jump velocity.
            currentVelocity.y += jumpSpeed;
            objectRigidbody.velocity = currentVelocity;
        }

        // A check method to see if we didnt use all of our jumps before touching the ground again.
        public bool JumpNumberCheck()
        {
            return currentJumpNumber != 0;
        }

        // Method for resetting the jump number once on the ground.
        public void JumpNumberReset()
        {
            currentJumpNumber = jumpNumber;
        }

        // Method for subtracting jump number before touching ground again.
        public void JumpNumberSubtract()
        {
            currentJumpNumber -= 1;
        }
    }
}
