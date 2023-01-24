using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RokasDan.FistPump.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private Rigidbody rigidBody;

        [Header("Forces")]
        [Min(0f)]
        [SerializeField]
        private float moveForce = 100f;

        [Min(0f)]
        [SerializeField]
        private float jumpSpeed = 20f;

        [Header("Inputs")]
        [SerializeField]
        private InputActionReference jumpInputAction;

        [SerializeField]
        private InputActionReference moveInputAction;


        private Vector3 absoluteMoveDirection;

        private void OnEnable()
        {
            //Subscribing the methods to prevent memory leaks
            jumpInputAction.action.performed += OnJumpPerformed;

            moveInputAction.action.performed += OnMovePerformed;
            moveInputAction.action.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            //Unsubscribing the methods to prevent memory leaks
            jumpInputAction.action.performed -= OnJumpPerformed;

            moveInputAction.action.performed -= OnMovePerformed;
            moveInputAction.action.canceled -= OnMoveCanceled;

            absoluteMoveDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            // If vector3 is not 0 0 0 we apply the move function.
            if (absoluteMoveDirection != Vector3.zero)
            {
                Move();
            }
        }

        // If WASD or arrows are pressed we start apply a Vector3 to our move direction.
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Move Performed!", this);
            var axis = context.ReadValue<Vector2>();
            absoluteMoveDirection = new Vector3(axis.x, 0f, axis.y);
        }


        // If WASD or arrows are not pressed the vector3.zero is applied to our move direction.
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            Debug.Log("Move Canceled!", this);
            absoluteMoveDirection = Vector3.zero;
        }

        // Listening for the space bar input, if pressed jump method is preformed.
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Jump Performed!", this);
            Jump();
        }

        // Our move function which adds velocity to our 3D vector.
        private void Move()
        {
            rigidBody.AddForce(absoluteMoveDirection * moveForce);
        }
        // Our Jump function adds velocity to Y vector.
        private void Jump()
        {
            var currentVelocity = rigidBody.velocity;
            currentVelocity.y += jumpSpeed;
            rigidBody.velocity = currentVelocity;
        }
    }
}
