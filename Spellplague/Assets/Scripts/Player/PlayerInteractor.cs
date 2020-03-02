using Spellplague.Interacting;
using Spellplague.Utility;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellplague.Player
{
    #pragma warning disable S3241, CS4014
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField]
        private InputSystemVariable inputSystem = default;
        [SerializeField]
        private PlayerStateVariable playerState = default;
        [SerializeField]
        private Camera playerCamera = default;
        [SerializeField]
        private TextMeshProUGUI itemNamerText = default;
        [SerializeField]
        private string interactNamerSuffix = "[E]";
        [SerializeField]
        private float interactorRayDistance = 2;
        [SerializeField]
        [Tooltip("Higher equals slower update rate")]
        private float interactorUpdateRate = 3;
        private bool runTasks;
        private bool interactPerformed;
        private bool isDisplaying;

        private void OnEnable()
        {
            runTasks = true;
            inputSystem.Value.Player.Inspect.performed += InteractPerformed;
            inputSystem.Value.Player.Inspect.canceled += InteractCanceled;
            InteractorRay();
        }

        private void InteractPerformed(InputAction.CallbackContext callback) => interactPerformed = true;

        private void InteractCanceled(InputAction.CallbackContext callback) => interactPerformed = false;

        private async Task InteractorRay()
        {
            while (runTasks)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(Time.deltaTime * (interactorUpdateRate * 1000)));
                if (playerState.CurrentPlayerSpecialState == PlayerSpecialState.Inspecting)
                {
                    EmptyText();
                    continue;
                }

                Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, 
                    interactorRayDistance);
                if (hitInfo.collider != null && hitInfo.collider.TryGetComponent(out IItemInteractor interactor))
                {
                    Interactor(interactor);
                }
                else
                {
                    EmptyText();
                }
            }
        }

        private void EmptyText()
        {
            if (itemNamerText.text.Length > 0)
            {
                isDisplaying = false;
                itemNamerText.text = string.Empty;
            }
        }

        private void Interactor(IItemInteractor interactor)
        {
            if (interactor.HasEvent() && interactPerformed)
            {
                interactor.GetEvent().Execute();
            }
            else if (interactor.ShowSuffix())
            {
                if (!isDisplaying)
                {
                    DisplayText($"{interactor.GetName()} {interactNamerSuffix}");
                }
            }
            else
            {
                if (!isDisplaying)
                {
                    DisplayText(interactor.GetName());
                }
            }
        }

        private void DisplayText(string text)
        {
            isDisplaying = true;
            itemNamerText.text = text;
        }

        private void OnDisable()
        {
            runTasks = false;
            inputSystem.Value.Player.Inspect.performed -= InteractPerformed;
            inputSystem.Value.Player.Inspect.canceled -= InteractCanceled;
        }
    }
}