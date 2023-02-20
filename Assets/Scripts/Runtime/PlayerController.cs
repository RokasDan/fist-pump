using UnityEngine;
using UnityEngine.InputSystem;

namespace RokasDan.FistPump.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Object Dependencies")]
        [SerializeField]
        private Rigidbody rigidBody;

        [SerializeField]
        private Transform cameraTransform;

        [Header("Raycast Dependencies")]
        [SerializeField]
        private GroundCheck groundCheck;

        [Header("Hover Dependencies")]
        [SerializeField]
        private HoverController hoverController;

        [Header("Locomotion Dependencies")]
        [SerializeField]
        private MoveForceController moveForceController;

        [SerializeField]
        private JumpForceController jumpForceController;

        [Header("Inputs")]
        [SerializeField]
        private InputActionReference jumpInputAction;

        [SerializeField]
        private InputActionReference moveInputAction;

        [SerializeField]
        private InputActionReference sprintInputAction;

        [Header("Input Gizmos")]
        [SerializeField]
        private float gizmoLenght = 3f;

        private Vector3 absoluteMoveDirection;
        private float radius;
        private int jumpNumberHelper;
        private bool isSprinting;

        private void OnDrawGizmos()
        {
            var moveDirection = GetMoveDirection();
            var position = transform.position;

            //Camera look directly from camera transform.
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position + new Vector3(0, 0.45f, 0), cameraTransform.forward.normalized * gizmoLenght);

            // Relative Move direction of the WASD absolute vectors when the rotation of the camera forward vector is applied.
            Gizmos.color = Color.red;
            Gizmos.DrawRay(position, moveDirection.normalized * gizmoLenght);
        }

        private void OnEnable()
        {
            //Subscribing the methods to prevent memory leaks
            jumpInputAction.action.performed += OnJumpPerformed;

            sprintInputAction.action.performed += OnSprintPerformed;
            sprintInputAction.action.canceled += OnSprintCanceled;

            moveInputAction.action.performed += OnMovePerformed;
            moveInputAction.action.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            //Unsubscribing the methods to prevent memory leaks
            jumpInputAction.action.performed -= OnJumpPerformed;

            sprintInputAction.action.performed -= OnSprintPerformed;
            sprintInputAction.action.canceled -= OnSprintCanceled;

            moveInputAction.action.performed -= OnMovePerformed;
            moveInputAction.action.canceled -= OnMoveCanceled;

            absoluteMoveDirection = Vector3.zero;
        }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void FixedUpdate()
        {
            // Handling ground check.
            groundCheck.UpdateGrounded();

            // Handling Hovering.
            hoverController.ObjectHitTop();
            hoverController.UpdateHover();

            // Handling Moving.
            // If vector3 is not 0 0 0 we apply the move function.
            if (absoluteMoveDirection != Vector3.zero)
            {
                UpdateMove();
                UpdateSprint();
            }
            else
            {
                moveForceController.StopObject();
            }

            // Handling Locomotion mode and jump reset.
            if (hoverController.IsInHoveringRange && hoverController.IsOverHoverThreshold)
            {
                moveForceController.LocomotionGround();
                jumpForceController.JumpNumberReset();
            }
            else
            {
                moveForceController.LocomotionAir();
            }
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
            absoluteMoveDirection = Vector3.zero;
        }

        // Listening for the space bar input, if pressed jump method is preformed.
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            UpdateJump();
        }

        // Activating sprint bool.
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            isSprinting = true;
        }

        // Deactivating sprint bool.
        private void OnSprintCanceled(InputAction.CallbackContext context)
        {
            isSprinting = false;
        }

        // Our move function which adds velocity to our 3D vector.
        private void UpdateMove()
        {
            moveForceController.LerpObjectVelocity(GetMoveDirection());
        }

        // Our jump method which sees if we can jump and applies jump force.
        private void UpdateJump()
        {
            var jumpCheck = jumpForceController.JumpNumberCheck();
            if (jumpCheck)
            {
                hoverController.HoverOff();
                jumpForceController.AddJumpForce(rigidBody);
                jumpForceController.JumpNumberSubtract();
            }
        }

        private void UpdateSprint()
        {
            if (absoluteMoveDirection.z >= 0.70 &&
                isSprinting &&
                hoverController.IsInHoveringRange &&
                hoverController.IsOverHoverThreshold)
            {
                moveForceController.AddSprintForce();
            }
            else
            {
                moveForceController.StopSprintForce();
            }
        }

        private Vector3 GetMoveDirection()
        {
            var rotation = GetMoveRotation();
            var relativeMoveDirection = rotation * absoluteMoveDirection;

            return relativeMoveDirection;
        }

        private Quaternion GetMoveRotation()
        {
            //Getting camera forward transform.
            var cameraDirection = cameraTransform.forward;

            //Flattening the camera forward transform to the Y plane.
            var flattenedLookDirection = Vector3.ProjectOnPlane(cameraDirection, Vector3.up);

            // Getting the angle which will rotate the absolute vectors from WASD keys.
            var flattenedLookRotation = Quaternion.LookRotation(flattenedLookDirection);

            return flattenedLookRotation;
        }
    }
}
