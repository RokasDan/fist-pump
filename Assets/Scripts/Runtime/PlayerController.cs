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
        private GroundedController groundedController;

        [Header("Locomotion Dependencies")]
        [SerializeField]
        private MoveForceController moveForceController;

        [SerializeField]
        private JumpForceController jumpForceController;

        [Header("Hover Dependencies")]
        [SerializeField]
        private HoverController hoverController;

        [SerializeField]
        private SpringForce springForce;

        [SerializeField]
        private PIDForce pidForce;

        [Header("Inputs")]
        [SerializeField]
        private InputActionReference jumpInputAction;

        [SerializeField]
        private InputActionReference moveInputAction;

        [SerializeField]
        private InputActionReference lookInputAction;

        [Header("Input Gizmos")]
        [SerializeField]
        private float gizmoLenght = 3f;

        [Header("Hover Type Controls")]
        // Not sure where to put this?
        [SerializeField]
        private float rideHeight;

        [SerializeField]
        private bool disableGravityOnHover;

        // Choose force type for hovering.
        public enum HoverType
        {
            SpringForce,
            PIDForce
        }

        public HoverType hoverType;

        // Choose force type for pushing down objects on which object is hovering.
        public enum PushDownForceType
        {
            SpringForce,
            PIDForce
        }

        public PushDownForceType pushDownForceType;
        //Not sure how to make these private and serializedField.

        private Vector3 flattenedLookDirection;
        private Quaternion flattenedLookRotation;
        private Vector3 absoluteMoveDirection;
        private Vector3 relativeMoveDirection;
        private float radius;
        private int jumpNumberHelper;
        private bool rideThreshold;
        private bool isHovering;

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
            groundedController.UpdateGrounded();
            RideHeightThreshold();
            PlayerHitTop();

            // Hover Forces.
            if (rideThreshold && isHovering)
            {
                // Hovering starts, we reset jump number.
                jumpForceController.JumpReset();

                // Switching to ground locomotion since we started hovering.
                moveForceController.LocomotionGround();

                // Turn off gravity if toggled.
                GravityOffOnHover();

                // Apply hover forces to the player.
                UpdateHover();

                // Apply down force to rigid bodies this object hits with ray.
                UpdateDownForce();
            }

            // Move Forces
            // If vector3 is not 0 0 0 we apply the move function.
            if (absoluteMoveDirection != Vector3.zero)
            {
                UpdateMove();
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
            absoluteMoveDirection = Vector3.zero;
        }

        // Listening for the space bar input, if pressed jump method is preformed.
        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            UpdateJump();
        }

        // Resetting hover bool, we want to reset this on the raycast enter event.
        public void HoverReset()
        {
            isHovering = true;
        }

        // Our move function which adds velocity to our 3D vector.
        private void UpdateMove()
        {
            // Applying the rotation of the camera forward vector to the WASD absolute vector.
            relativeMoveDirection = flattenedLookRotation * absoluteMoveDirection;

            // Adding force through our moveForceController class.
            moveForceController.AddMoveForce(relativeMoveDirection);
        }

        // Our jump method which sees if we can jump and applies jump force.
        private void UpdateJump()
        {
            var jumpCheck = jumpForceController.JumpNumberCheck();
            if (jumpCheck)
            {
                // Enable gravity if previously disabled by the disable gravity on hover toggle.
                GravityOnJump();

                isHovering = false;
                moveForceController.LocomotionAir();
                jumpForceController.AddJumpForce(rigidBody);
                jumpForceController.JumpNumberSubtract();
            }
        }

        private void UpdateHover()
        {
            // Choosing a hover type and applying hovering forces.
            if (hoverType == HoverType.SpringForce)
            {
                var position = transform.position;
                var hoverForce = springForce.GetOutput(groundedController.RayHit, rigidBody, rideHeight);
                hoverController.Hover(rigidBody, hoverForce);
                Debug.DrawLine(position, position + (Vector3.down * hoverForce), Color.yellow);
            }
            else
            {
                var position = transform.position;
                var hoverForce = pidForce.GetOutput(Time.deltaTime, rideHeight, groundedController.GroundDistance);
                hoverController.Hover(rigidBody, hoverForce);
                Debug.DrawLine(position, position + (Vector3.down * hoverForce), Color.red);
            }

        }

        private void UpdateDownForce()
        {
            // Choosing push type and pushing down rigid bodies if hovering on them.
            if (pushDownForceType == PushDownForceType.SpringForce)
            {
                var hoverForce = springForce.GetOutput(groundedController.RayHit, rigidBody, rideHeight);
                hoverController.PushDown(groundedController.RayHit, hoverForce);
            }
            else
            {
                var hoverForce = pidForce.GetOutput(Time.deltaTime, rideHeight, groundedController.GroundDistance);
                hoverController.PushDown(groundedController.RayHit, hoverForce);
            }
        }


        // Method used for ride height check.
        private void RideHeightThreshold()
        {
            var startThreshold = groundedController.RayHit.distance - rideHeight;
            if (-startThreshold >= 0)
            {
                rideThreshold = true;

                // Check if we want to turn of gravity on hover, if so we turn it off.
            }

            if (groundedController.IsGrounded == false)
            {
                // Setting air locomotion, useful when player just rides off cliffs and drops.
                GravityOnJump();
                moveForceController.LocomotionAir();
                rideThreshold = false;
            }
        }

        // Method for turning hovering and ground locomotion if jump failed and hit something
        // before exiting ground check Ray.
        private void PlayerHitTop()
        {
            if(isHovering == false && groundedController.IsGrounded && rigidBody.velocity.y <= 0)
            {
                isHovering = true;
            }
        }

        // Gravity off on hover if the designer wants to do so.
        private void GravityOffOnHover()
        {
            if (disableGravityOnHover)
            {
                rigidBody.useGravity = false;
            }
        }

        // If we choose to disable gravity on hover, we need to turn it back on when we jump.
        private void GravityOnJump()
        {
            if (disableGravityOnHover)
            {
                rigidBody.useGravity = true;
            }
        }

        //Gizmos
        private void OnDrawGizmos()
        {
            var position = transform.position;
            //Camera look directly from camera transform.
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position + new Vector3(0, 0.45f, 0), cameraTransform.forward.normalized * gizmoLenght
            );

            //Camera look flattened to the Y coordinate.
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(position, flattenedLookDirection.normalized * gizmoLenght
            );

            // Relative Move direction of the WASD absolute vectors when the rotation of the camera forward vector is applied.
            Gizmos.color = Color.red;
            Gizmos.DrawRay(position, relativeMoveDirection.normalized * gizmoLenght);

            // Ride height sphere.
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(position - new Vector3(0, rideHeight, 0), 0.3f);
        }
    }
}
