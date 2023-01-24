using UnityEngine;
using UnityEngine.InputSystem;

namespace RokasDan.FistPump.Runtime
{
    internal sealed class JumpyBall : MonoBehaviour
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
            jumpInputAction.action.performed += OnJumpPerformed;

            moveInputAction.action.performed += OnMovePerformed;
            moveInputAction.action.canceled += OnMoveCanceled;
        }

        private void OnDisable()
        {
            jumpInputAction.action.performed -= OnJumpPerformed;

            moveInputAction.action.performed -= OnMovePerformed;
            moveInputAction.action.canceled -= OnMoveCanceled;

            absoluteMoveDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (absoluteMoveDirection != Vector3.zero)
            {
                Move();
            }
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Jump Performed!", this);
            Jump();
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Debug.Log("Move Performed!", this);
            var axis = context.ReadValue<Vector2>();
            absoluteMoveDirection = new Vector3(axis.x, 0f, axis.y);
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            Debug.Log("Move Canceled!", this);
            absoluteMoveDirection = Vector3.zero;
        }

        private void Move()
        {
            rigidBody.AddForce(absoluteMoveDirection * moveForce);
        }

        private void Jump()
        {
            var currentVelocity = rigidBody.velocity;
            currentVelocity.y += jumpSpeed;
            rigidBody.velocity = currentVelocity;
        }
    }
}
