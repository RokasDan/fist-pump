using System;
using System.Collections;
using System.Collections.Generic;
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
        private float AirMoveSpeed = 6f;

        [SerializeField]
        private float AirDrag = 6f;


        [Header("Jumping")]
        [Min(0f)]
        [SerializeField]
        private float jumpSpeed = 20f;

        [SerializeField]
        private int jumpNumber;

        private int jumpNumberHelper;

        private bool onGround;

        // The main method of the class which will calculate and add velocity to the rigid body of the object.
        public void AddMoveForce(Vector3 moveDirection)
        {
            var groundSpeed = groundMoveSpeed * speedMultiplier;
            var airSpeed = AirMoveSpeed * speedMultiplier;
            if (onGround)
            {
                objectRigidbody.AddForce(moveDirection * groundSpeed, ForceMode.Acceleration);
            }
            else
            {
                objectRigidbody.AddForce(moveDirection * airSpeed, ForceMode.Acceleration);
            }

        }

        public void AddJumpForce()
        {
            // Applying force.
            Debug.Log("Jump Performed!", this);
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
            return jumpNumberHelper != 0;
        }

        // Method for resetting the jump number once on the ground.
        public void JumpReset()
        {
            jumpNumberHelper = jumpNumber;
        }

        // Method for subtracting jump number before touching ground again.
        public void JumpNumberSubtract()
        {
            jumpNumberHelper -= 1;
        }

        // Method which switches between ground and air locomotion. Should be placed
        // within the ground check of the player controller.
        public void LocomotionGround()
        {
            onGround = true;
            objectRigidbody.drag = groundDrag;
        }

        public void LocomotionAir()
        {
            onGround = false;
            objectRigidbody.drag = AirDrag;
        }

    }
}
