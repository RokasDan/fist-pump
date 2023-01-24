using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RokasDan.FistPump.Runtime
{
    internal sealed class InputSystem : MonoBehaviour
    {
        [SerializeField]
        private List<InputActionReference> inputActions;

        private void OnEnable()
        {
            SetInputActionsEnabled(true);
        }

        private void OnDisable()
        {
            SetInputActionsEnabled(false);
        }

        private void SetInputActionsEnabled(bool isEnabled)
        {
            foreach (var inputAction in inputActions)
            {
                if (isEnabled)
                {
                    inputAction.action.Enable();
                }
                else
                {
                    inputAction.action.Disable();
                }
            }
        }
    }
}
