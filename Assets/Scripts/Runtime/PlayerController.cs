// Code from Unity Documentation
// Link - https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private float playerSpeed = 2.0f;
        [SerializeField]
        private float jumpHeight = 1.0f;
        [SerializeField]
        private float gravityValue = -9.81f;

        private CharacterController controller;
        private Vector3 playerVelocity;
        private bool groundedPlayer;
        private InputManager inputManager;
        private Transform cameraTransfor;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            inputManager = InputManager.Instance;
            cameraTransfor = Camera.main.transform;
        }

        void Update()
        {
            // We are checking if the player is on the ground.
            groundedPlayer = controller.isGrounded;
            if (groundedPlayer && playerVelocity.y < 0)
            {
                playerVelocity.y = 0f;
            }

            // Horizontal movement.
            Vector2 movement = inputManager.GetPlayerMovement();
            Vector3 move = new Vector3(movement.x, 0f, movement.y);

            // Tracking the camera transform to sync it with the player transform.
            move = cameraTransfor.forward * move.z + cameraTransfor.right * move.x;
            move.y = 0f;
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Jumping.
            // if (move != Vector3.zero)
            // {
            //     gameObject.transform.forward = move;
            // }

            // Changes the height position of the player..
            if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
            {
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
}
