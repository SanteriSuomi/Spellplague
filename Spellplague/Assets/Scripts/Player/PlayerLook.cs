using Spellplague.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellplague.Player
{
    #pragma warning disable S3168 // Async method is a fire and forget method, so returning task is useless
    [RequireComponent(typeof(PlayerPerspectiveChanger))]
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField]
        private InputSystemVariable inputSystem = default;
        [SerializeField]
        private ControlTypeVariable controlType = default;
        [SerializeField]
        private Vector3Variable thirdPersonCameraPosition = default;
        [SerializeField]
        private BoolVariable clampRotation = default;
        [SerializeField]
        private PlayerStateVariable playerState = default;
        private CharacterController characterController;
        private PlayerPerspectiveChanger perspectiveChanger;
        private Transform playerCamera;
        private Transform playerTransform;
        [SerializeField]
        private Vector3 thirdPersonCameraRotationCenterOffset = new Vector3(0, 1.25f, 0);
        [SerializeField]
        private Vector2 thirdPersonZoomValueRangeClamp = new Vector2(-0.75f, 2.25f);
        [SerializeField]
        private Vector2 verticalThirdPersonClampRange = new Vector2(-6.5f, 21.5f);
        private Vector3 currentVelocity;
        private Vector2 lookingReadValue;
        private Vector2 lookValue;
        [SerializeField]
        private LayerMask cameraCollisionMask = default;
        [SerializeField]
        private float cameraCollisionSphereRadius = 0.85f;
        [SerializeField]
        private float overAllLookSpeed = 15;
        [SerializeField]
        private float firstPersonCameraSpeed = 100;
        [SerializeField]
        private float verticalFirstPersonClampRange = 80;
        [SerializeField]
        private float thirdPersonRotationSpeed = 250;
        [SerializeField]
        private float thirdPersonCameraPositionSmooth = 1;
        [SerializeField]
        private float thirdPersonCameraPositionMax = 250;
        [SerializeField]
        private float thirdPersonCameraPositionYSpeedMultiplier = 0.1f;
        [SerializeField]
        private float scrollZoomValueMultiplier = 0.0045f;
        [SerializeField]
        private float inspectingHorizontalClampRange = 45;
        [SerializeField]
        private float cameraCollisionCameraZoomMultiplier = 5f;
        [SerializeField]
        private float cameraCollisionCameraZoomMultiplierLineCast = 2.5f;
        [SerializeField]
        private float cameraCollisionUpdateInMilliseconds = 0.5f;
        private float thirdPersonZoomValue;
        private float originalHorizontalRotation;
        private bool isZooming;
        private bool runCameraCollision;
        private bool clampRotationBool;

        private void Awake()
        {
            InitializeCursor();
            playerCamera = GetComponentInChildren<Camera>().transform;
            perspectiveChanger = GetComponent<PlayerPerspectiveChanger>();
            characterController = GetComponent<CharacterController>();
            playerTransform = transform;
        }

        private static void InitializeCursor() => Cursor.lockState = CursorLockMode.Locked;

        private void OnEnable()
        {
            runCameraCollision = true;
            inputSystem.Value.Player.Looking.Enable();
            inputSystem.Value.Player.Looking.performed += LookingPerformed;
            inputSystem.Value.Player.Looking.canceled += LookingCanceled;
            inputSystem.Value.Player.ThirdPersonZoom.Enable();
            inputSystem.Value.Player.ThirdPersonZoom.performed += ThirdPersonZoomPerformed;
            inputSystem.Value.Player.ThirdPersonZoom.canceled += ThirdPersonZoomCanceled;
            clampRotation.ValueChangedEvent += ClampRotationChanged;
            CameraCollision();
        }

        private void ThirdPersonZoomCanceled(InputAction.CallbackContext callback)
            => isZooming = false;

        private void LookingPerformed(InputAction.CallbackContext callback)
            => lookingReadValue = callback.ReadValue<Vector2>();

        private void LookingCanceled(InputAction.CallbackContext callback)
            => lookingReadValue = Vector2.zero;

        private void ClampRotationChanged(bool value) => clampRotationBool = value;

        private async void CameraCollision()
        {
            while (runCameraCollision)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(cameraCollisionUpdateInMilliseconds));

                if (isZooming || lookingReadValue.sqrMagnitude != 0 || characterController.velocity.sqrMagnitude != 0
                    && controlType.Value == ControlType.ThirdPerson)
                {
                    bool lineHit = Physics.Linecast(playerTransform.position, playerCamera.position, cameraCollisionMask,
                        QueryTriggerInteraction.Ignore);
                    if (lineHit)
                    {
                        ModifyCameraZoom(cameraCollisionCameraZoomMultiplier * cameraCollisionCameraZoomMultiplierLineCast 
                            * Time.deltaTime);
                        continue;
                    }

                    bool sphereHit = Physics.CheckSphere(playerCamera.position, cameraCollisionSphereRadius, 
                        cameraCollisionMask, QueryTriggerInteraction.Ignore);
                    if (sphereHit && controlType.Value == ControlType.ThirdPerson)
                    {
                        ModifyCameraZoom(cameraCollisionCameraZoomMultiplier * Time.deltaTime);
                    }
                }
            }
        }

        private void ThirdPersonZoomPerformed(InputAction.CallbackContext callback)
        {
            if (playerState.CurrentPlayerSpecialState == PlayerSpecialState.Inspecting) { return; }

            float zoomValue = callback.ReadValue<Vector2>().y;
            if (controlType.Value == ControlType.FirstPerson && zoomValue <= 0)
            {
                controlType.Value = ControlType.ThirdPerson;
            }
            else if (controlType.Value == ControlType.ThirdPerson)
            {
                isZooming = true;
                ModifyCameraZoom(zoomValue * scrollZoomValueMultiplier);
            }
        }

        private void ModifyCameraZoom(float byValue)
        {
            thirdPersonZoomValue += byValue;
            if (thirdPersonZoomValue >= thirdPersonZoomValueRangeClamp.y)
            {
                perspectiveChanger.SwitchPerspective();
            }

            thirdPersonZoomValue = Mathf.Clamp(thirdPersonZoomValue, thirdPersonZoomValueRangeClamp.x,
                thirdPersonZoomValueRangeClamp.y);
        }

        private void Update()
        {
            if (isZooming || lookingReadValue.sqrMagnitude != 0)
            {
                lookValue.x += lookingReadValue.x * overAllLookSpeed * Time.deltaTime;
                lookValue.y += lookingReadValue.y * overAllLookSpeed * Time.deltaTime;
                HorizontalClamp();
                switch (controlType.Value)
                {
                    case ControlType.FirstPerson:
                        FPPlayerRotation();
                        FPCameraRotation();
                        break;
                    case ControlType.ThirdPerson:
                        TPPlayerRotation();
                        TPCameraPosition();
                        TPCameraRotation();
                        break;
                    default:
                        Debug.LogError("Shouldn't be here.");
                        break;
                }
            }
        }

        private void HorizontalClamp()
        {
            if (clampRotationBool)
            {
                lookValue.x = Mathf.Clamp(lookValue.x, originalHorizontalRotation - inspectingHorizontalClampRange,
                            originalHorizontalRotation + inspectingHorizontalClampRange);
            }
            else {  originalHorizontalRotation = lookValue.x; }
        }

        private void FPPlayerRotation()
        {
            playerTransform.localRotation = Quaternion.Slerp(playerTransform.localRotation,
                            Quaternion.Euler(0, lookValue.x, 0), firstPersonCameraSpeed * Time.deltaTime);
        }

        private void FPCameraRotation()
        {
            lookValue.y = Mathf.Clamp(lookValue.y, -verticalFirstPersonClampRange, verticalFirstPersonClampRange);
            playerCamera.localRotation = Quaternion.Slerp(playerCamera.localRotation,
                            Quaternion.Euler(-lookValue.y, 0, 0), firstPersonCameraSpeed * Time.deltaTime);
        }

        private void TPPlayerRotation()
        {
            playerTransform.localRotation = Quaternion.Slerp(playerTransform.localRotation, Quaternion.Euler(0, lookValue.x, 0),
                            thirdPersonRotationSpeed * Time.deltaTime);
        }

        private void TPCameraPosition()
        {
            lookValue.y = Mathf.Clamp(lookValue.y, verticalThirdPersonClampRange.x, verticalThirdPersonClampRange.y);
            Vector3 targetPosition = playerTransform.TransformPoint(thirdPersonCameraPosition.Value.x,
                            thirdPersonCameraPosition.Value.y + -lookValue.y * thirdPersonCameraPositionYSpeedMultiplier,
                            thirdPersonCameraPosition.Value.z + thirdPersonZoomValue);
            playerCamera.position = Vector3.SmoothDamp(playerCamera.position, targetPosition, ref currentVelocity,
                            thirdPersonCameraPositionSmooth * Time.deltaTime, thirdPersonCameraPositionMax);
        }

        private void TPCameraRotation()
        {
            Quaternion targetRotation = Quaternion.LookRotation(playerTransform.position - playerCamera.position
                            + thirdPersonCameraRotationCenterOffset, Vector3.up);
            playerCamera.rotation = Quaternion.Slerp(playerCamera.rotation, targetRotation, thirdPersonRotationSpeed * Time.deltaTime);
        }

        private void OnDisable()
        {
            runCameraCollision = false;
            inputSystem.Value.Player.Looking.Disable();
            inputSystem.Value.Player.Looking.performed -= LookingPerformed;
            inputSystem.Value.Player.Looking.canceled -= LookingCanceled;
            inputSystem.Value.Player.ThirdPersonZoom.Disable();
            inputSystem.Value.Player.ThirdPersonZoom.performed -= ThirdPersonZoomPerformed;
            inputSystem.Value.Player.ThirdPersonZoom.canceled -= ThirdPersonZoomCanceled;
        }
    }
}