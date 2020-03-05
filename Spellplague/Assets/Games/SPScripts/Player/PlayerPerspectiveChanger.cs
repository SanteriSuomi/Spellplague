using Spellplague.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellplague.Player
{
    public class PlayerPerspectiveChanger : MonoBehaviour
    {
        [SerializeField]
        private ControlTypeVariable controlType = default;
        [SerializeField]
        private Vector3Variable thirdPersonCameraPosition = default;
        [SerializeField]
        private InputSystemVariable inputSystem = default;
        private Transform playerTransform;
        private Transform playerCamera;
        [SerializeField]
        private Vector3 firstPersonCameraPosition = new Vector3(0, 0.75f, 0);

        private void Awake()
        {
            playerCamera = GetComponentInChildren<Camera>().transform;
            playerTransform = transform;
        }

        private void OnEnable()
        {
            inputSystem.Value.Player.ThirdPerson.Enable();
            inputSystem.Value.Player.ThirdPerson.performed += SwitchPerspective;
        }

        public void SwitchPerspective() => Switch();
        
        private void SwitchPerspective(InputAction.CallbackContext callback) 
            => Switch();

        private void Switch()
        {
            switch (controlType.Value)
            {
                case ControlType.FirstPerson:
                    controlType.Value = ControlType.ThirdPerson;
                    EnableThirdPerson();
                    break;
                case ControlType.ThirdPerson:
                    controlType.Value = ControlType.FirstPerson;
                    EnableFirstPerson();
                    break;
                default:
                    Debug.LogError("Should not reach here.");
                    break;
            }
        }

        private void EnableFirstPerson() 
            => playerCamera.position = playerTransform.position + firstPersonCameraPosition;

        private void EnableThirdPerson() 
            => playerCamera.position = playerTransform.TransformPoint(thirdPersonCameraPosition.Value);

        private void OnDisable()
        {
            inputSystem.Value.Player.ThirdPerson.Disable();
            inputSystem.Value.Player.ThirdPerson.performed -= SwitchPerspective;
        }
    }
}