using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace RokasDan.FistPump.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        private Transform cameraTransform;

        [FormerlySerializedAs("groundCheck")]
        [SerializeField]
        private GroundedController groundedController;

        [FormerlySerializedAs("hoverRay")]
        [SerializeField]
        private HoverController hoverController;

        [SerializeField]
        private MoveForceController moveForceController;

        [Header("Inputs")]
        [SerializeField]
        private InputActionReference jumpInputAction;

        [SerializeField]
        private InputActionReference moveInputAction;

        [SerializeField]
        private InputActionReference lookInputAction;

        [Header("Gizmos")]
        [SerializeField]
        private float gizmoLenght = 3f;

        private Vector3 flattenedLookDirection;
        private Quaternion flattenedLookRotation;
        private Vector3 absoluteMoveDirection;
        private Vector3 relativeMoveDirection;
        private float radius;
        private int jumpNumberHelper;
        private int isInAir = 1;

        private void OnEnable()
        {
            //Subscribing the methods to prevent memory leaks
            jumpInputAction.action.performed += OnJumpPerformed;

            lookInputAction.action.performed += OnLookPreformed;

            moveInputAction.action.performed += OnMovePerformed;
            moveInputAction.action.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            //Unsubscribing the methods to prevent memory leaks
            jumpInputAction.action.performed -= OnJumpPerformed;

            lookInputAction.action.performed -= OnLookPreformed;

            moveInputAction.action.performed -= OnMovePerformed;
            moveInputAction.action.canceled -= OnMoveCanceled;

            absoluteMoveDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            GroundCheck();
            // If vector3 is not 0 0 0 we apply the move function.
            if (absoluteMoveDirection != Vector3.zero)
            {
                Move();
            }
        }

        private void OnLookPreformed(InputAction.CallbackContext context)
        {
            //Getting camera forward transform.
            var cameraDirection = cameraTransform.forward;

            //Flattening the camera forward transform to the Y plane.
            flattenedLookDirection = Vector3.ProjectOnPlane(cameraDirection, Vector3.up);

            // Getting the angle which will rotate the absolute vectors from WASD keys.
            flattenedLookRotation = Quaternion.LookRotation(flattenedLookDirection);
        }

        // If WASD or arrows are pressed we start apply a Vector3 to our move direction.
        private void OnMovePerformed(InputAction.CallbackContext context)
        {
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
            Jump();
        }

        // Our move function which adds velocity to our 3D vector.
        private void Move()
        {
            // Applying the rotation of the camera forward vector to the WASD absolute vector.
            relativeMoveDirection = flattenedLookRotation * absoluteMoveDirection;

            // Adding force through our moveForceController class.
            moveForceController.AddMoveForce(relativeMoveDirection);
        }

        // Our Jump function adds velocity to Y vector.
        private void Jump()
        {
            if (moveForceController.JumpNumberCheck())
            {
                //Deactivating hover so it wouldn't stop one from jumping.
                hoverController.isHovering = false;

                // Switching to air locomotion so the first jump would not be affected by ground drag.
                moveForceController.LocomotionAir();

                // Adding jump force.
                moveForceController.AddJumpForce();

                // Double jump number being brought down before touching ground.
                moveForceController.JumpNumberSubtract();
            }
        }

        private void GroundCheck()
        {
            // Checking if we are grounded with the lenght of out hover ray distance.
            bool grounded = groundedController.IsGrounded(hoverController.rayHit.distance);

            //Creating an event so air jumps would be possible when riding of a cliff.
            //Event prevents jump stack so jump number would only reset when the ray
            //enters the ground.
            if (grounded && isInAir == 1)
            {
                // Resting jump number since raycast enters ground.
                moveForceController.JumpReset();

                // Switching to ground locomotion settings since raycast enters ground.
                moveForceController.LocomotionGround();
                isInAir = 0;
            }

            if (grounded == false)
            {
                // Switching to air locomotion settings since raycast leaves ground.
                moveForceController.LocomotionAir();
                isInAir = 1;
            }
        }

        //Gizmos
        private void OnDrawGizmos()
        {
            //Camera look directly from camera transform.
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position + new Vector3(0, 0.45f, 0), cameraTransform.forward.normalized * gizmoLenght
            );

            //Camera look flattened to the Y coordinate.
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, flattenedLookDirection.normalized * gizmoLenght
            );

            // Relative Move direction of the WASD absolute vectors when the rotation of the camera forward vector is applied.
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, relativeMoveDirection.normalized * gizmoLenght);
        }
    }
}
