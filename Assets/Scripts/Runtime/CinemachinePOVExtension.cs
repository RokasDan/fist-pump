using System;
using Cinemachine;
using UnityEngine;

namespace RokasDan.FistPump.Runtime
{
    public class CinemachinePOVExtension : CinemachineExtension
    {
        [SerializeField]
        private float horizontalSpeed = 10f;
        [SerializeField]
        private float verticalSpeed = 10f;
        [SerializeField]
        private float clampAngle = 90f;

        private InputManager inputManager;
        private Vector3 startingRotation;

        protected override void Awake()
        {
            inputManager = InputManager.Instance;
            base.Awake();
        }

        protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
        {
            if (vcam.Follow)
            {
                if (stage == CinemachineCore.Stage.Aim)
                {
                    //If mouse is not present the camera looks same way as it looked before.
                    if (startingRotation == null) startingRotation = transform.localRotation.eulerAngles;
                    //Getting mouse input.
                    Vector2 deltaInput = inputManager.GetMouseDelta();
                    //Allowing mouse to move the camera.
                    startingRotation.x += deltaInput.x * verticalSpeed * Time.deltaTime;
                    startingRotation.y += deltaInput.y * horizontalSpeed * Time.deltaTime;
                    //Clamping look angle.
                    startingRotation.y = Math.Clamp(startingRotation.y, -clampAngle, clampAngle);
                    state.RawOrientation = Quaternion.Euler(-startingRotation.y, startingRotation.x, 0f);

                }
            }
        }
    }
}
