using Spellplague.Utility;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellplague.Player
{
    [RequireComponent(typeof(PlayerGravity))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateVariable playerState = default;
        [SerializeField]
        private InputSystemVariable inputSystem = default;
        [SerializeField]
        private Camera playerCamera = default;
        private CharacterController characterController;
        private PlayerGravity playerGravity;
        private Transform playerTransform;
        private Coroutine playerCameraFovCoroutine;
        private Vector2 moveReadValue;
        private Vector3 moveValue;
        [SerializeField]
        private float movementSpeed = 3.3f;
        [SerializeField]
        private float sprintingSpeedMultiplier = 1.5f;
        [SerializeField]
        private float crouchSpeedMultiplier = 0.5f;
        [SerializeField]
        private float jumpHeight = 1.25f;
        [SerializeField]
        private float jumpTime = 22.5f;
        [SerializeField]
        private float jumpMultiplierValue = 3.5f;
        [SerializeField]
        private float maxJumpTime = 0.8f;
        [SerializeField]
        private float playerCameraMaxRunFov = 110;
        [SerializeField]
        private float playerCameraRunFovChangeSpeed = 2.5f;
        private float playerCameraMinFov;
        private float maxVerticalHeight;
        private float timeBeforeJump;
        private bool isJumping;
        private bool isGrounded;
        private bool isSprinting;
        private bool isCrouching;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerGravity = GetComponent<PlayerGravity>();
            playerTransform = transform;
            playerCameraMinFov = playerCamera.fieldOfView;
        }

        private void OnEnable()
        {
            inputSystem.Value.Player.Movement.Enable();
            inputSystem.Value.Player.Movement.performed += MovementPerformed;
            inputSystem.Value.Player.Movement.canceled += MovementCanceled;
            inputSystem.Value.Player.Sprint.Enable();
            inputSystem.Value.Player.Sprint.performed += SprintPerformed;
            inputSystem.Value.Player.Sprint.canceled += SprintCanceled;
            playerGravity.IsGroundedEvent += IsGrounded;
            inputSystem.Value.Player.Jump.Enable();
            inputSystem.Value.Player.Jump.performed += JumpPerformedAsync;
            inputSystem.Value.Player.Crouch.Enable();
            inputSystem.Value.Player.Crouch.performed += CrouchPerformed;
            inputSystem.Value.Player.Crouch.canceled += CrouchCanceled;
        }

        #region Movement Events
        private void MovementPerformed(InputAction.CallbackContext callback)
        {
            moveReadValue = callback.ReadValue<Vector2>().normalized;
            if (!isSprinting)
            {
                playerState.CurrentPlayerMoveState = PlayerMove.Walk;
            }
        }

        private void MovementCanceled(InputAction.CallbackContext callback)
        {
            moveReadValue = Vector2.zero;
            playerState.CurrentPlayerMoveState = PlayerMove.Still;
        }
        #endregion

        #region Sprint Events
        private void SprintPerformed(InputAction.CallbackContext callback)
        {
            isSprinting = true;
            if (playerState.CurrentPlayerSpecialState != PlayerSpecialState.Inspecting)
            {
                if (playerCameraFovCoroutine != null) StopCoroutine(playerCameraFovCoroutine);
                playerCameraFovCoroutine = StartCoroutine(ChangeCameraFov(playerCameraMaxRunFov));
            }

            if (moveReadValue.sqrMagnitude != 0)
            {
                playerState.CurrentPlayerMoveState = PlayerMove.Sprint;
            }
        }

        private void SprintCanceled(InputAction.CallbackContext callback)
        {
            isSprinting = false;
            if (playerState.CurrentPlayerSpecialState != PlayerSpecialState.Inspecting)
            {
                if (playerCameraFovCoroutine != null) StopCoroutine(playerCameraFovCoroutine);
                playerCameraFovCoroutine = StartCoroutine(ChangeCameraFov(playerCameraMinFov));
            }

            playerState.CurrentPlayerMoveState = PlayerMove.Still;
        }

        private IEnumerator ChangeCameraFov(float fov)
        {
            while (!Mathf.Approximately(playerCamera.fieldOfView, fov))
            {
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov,
                    playerCameraRunFovChangeSpeed * Time.deltaTime);
                Mathf.Clamp(playerCamera.fieldOfView, playerCameraMinFov, playerCameraMaxRunFov);
                yield return null;
            }
        }
        #endregion

        #region Grounded Events
        private void IsGrounded(bool isGrounded) => this.isGrounded = isGrounded;
        #endregion

        #region Jump Events
        private async void JumpPerformedAsync(InputAction.CallbackContext callback)
        {
            if (isJumping || isCrouching || !isGrounded) { return; }
            isJumping = true;
            maxVerticalHeight = playerTransform.position.y + jumpHeight;
            timeBeforeJump = Time.realtimeSinceStartup + maxJumpTime;
            isJumping = await JumpAsync();
        }

        private async Task<bool> JumpAsync()
        {
            playerState.CurrentPlayerStance = PlayerStance.Jump;
            float jumpMultiplier = 1;
            while (playerTransform.position.y < maxVerticalHeight && Time.realtimeSinceStartup < timeBeforeJump)
            {
                playerTransform.position = SPUtility.SmoothStep(playerTransform.position, playerTransform.position
                            + Vector3.up, jumpTime * jumpMultiplier * Time.deltaTime);
                jumpMultiplier -= jumpMultiplierValue * Time.deltaTime;
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * 1000));
            }

            playerState.CurrentPlayerStance = PlayerStance.Upright;
            return false;
        }
        #endregion

        #region Crouch Events
        private void CrouchPerformed(InputAction.CallbackContext callback)
        {
            isCrouching = true;
            playerState.CurrentPlayerStance = PlayerStance.Crouch;
        }

        private void CrouchCanceled(InputAction.CallbackContext callback)
        {
            isCrouching = false;
            playerState.CurrentPlayerStance = PlayerStance.Upright;
        }
        #endregion

        private void Update()
        {
            if (moveReadValue.sqrMagnitude != 0)
            {
                moveValue.x = moveReadValue.x * movementSpeed * Time.deltaTime;
                moveValue.z = moveReadValue.y * movementSpeed * Time.deltaTime;
                Crouch();
                Sprint();
                Move();
            }
        }

        private void Crouch()
        {
            if (isCrouching)
            {
                moveValue *= crouchSpeedMultiplier;
            }
        }

        private void Sprint()
        {
            if (isSprinting && !isCrouching)
            {
                moveValue *= sprintingSpeedMultiplier;
            }
        }

        private void Move()
        {
            Vector3 movement = playerTransform.right * moveValue.x + playerTransform.forward * moveValue.z;
            characterController.Move(movement);
        }

        private void OnDisable()
        {
            inputSystem.Value.Player.Movement.Disable();
            inputSystem.Value.Player.Movement.performed -= MovementPerformed;
            inputSystem.Value.Player.Movement.canceled -= MovementCanceled;
            inputSystem.Value.Player.Sprint.Disable();
            inputSystem.Value.Player.Sprint.performed -= SprintPerformed;
            inputSystem.Value.Player.Sprint.canceled -= SprintCanceled;
            playerGravity.IsGroundedEvent -= IsGrounded;
            inputSystem.Value.Player.Jump.Disable();
            inputSystem.Value.Player.Jump.performed -= JumpPerformedAsync;
            inputSystem.Value.Player.Crouch.Disable();
            inputSystem.Value.Player.Crouch.performed -= CrouchPerformed;
            inputSystem.Value.Player.Crouch.canceled -= CrouchCanceled;
        }
    }
}