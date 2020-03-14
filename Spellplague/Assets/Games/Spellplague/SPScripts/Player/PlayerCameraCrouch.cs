using Spellplague.Utility;
using System.Collections;
using UnityEngine;

namespace Spellplague.Player
{
    public class PlayerCameraCrouch : MonoBehaviour
    {
        [SerializeField]
        private PlayerStateVariable playerState = default;
        [SerializeField]
        private Vector3Variable thirdPersonPosition = default;
        private Coroutine moveCameraCoroutine;
        [SerializeField]
        private Vector3 crouchPositionOffset = new Vector3(0, 0.4f, 0);
        private Vector3 originalPosition;
        [SerializeField]
        private float crouchSmooth = 5;

        private void Awake() => originalPosition = transform.localPosition;

        private void OnEnable() => playerState.PlayerStateChangedEvent += PlayerStateChanged;

        private void PlayerStateChanged(PlayerStance value)
        {
            if (value == PlayerStance.Crouch)
            {
                originalPosition = transform.localPosition;
                thirdPersonPosition.Value -= crouchPositionOffset;
                Vector3 crouchPosition = transform.localPosition - crouchPositionOffset;
                NewMoveCameraCoroutine(crouchPosition);
            }
            else
            {
                thirdPersonPosition.Value = thirdPersonPosition.OriginalValue;
                NewMoveCameraCoroutine(originalPosition);
            }
        }

        private void NewMoveCameraCoroutine(Vector3 position)
        {
            if (moveCameraCoroutine != null)
            {
                StopCoroutine(moveCameraCoroutine);
            }

            moveCameraCoroutine = StartCoroutine(MoveCrouchCameraTowards(position));
        }

        private IEnumerator MoveCrouchCameraTowards(Vector3 goal)
        {
            while (transform.localPosition != goal)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition,
                    goal, crouchSmooth * Time.deltaTime);
                yield return null;
            }
        }

        private void OnDisable() => playerState.PlayerStateChangedEvent -= PlayerStateChanged;
    }
}