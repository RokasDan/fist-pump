using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class InputManager : MonoBehaviour
    {
        // Singleton
        private static InputManager _instance;

        public static InputManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private InputActions playerControls;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
            playerControls = new InputActions();
            // Removes cursor while playing.
            Cursor.visible = false;
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        // Helper functions for movement.

        public Vector2 GetPlayerMovement()
        {
            return playerControls.Player.Move.ReadValue<Vector2>();
        }

        public Vector2 GetMouseDelta()
        {
            return playerControls.Player.Look.ReadValue<Vector2>();
        }

        public bool PlayerJumpedThisFrame()
        {
            return playerControls.Player.Jump.triggered;
        }

    }
}
