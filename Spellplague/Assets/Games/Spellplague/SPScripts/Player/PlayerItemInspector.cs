using Spellplague.Inventory;
using Spellplague.Items;
using Spellplague.Utility;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellplague.Player
{
    public class PlayerItemInspector : MonoBehaviour
    {
        [SerializeField]
        private ControlTypeVariable controlType = default;
        [SerializeField]
        private InputSystemVariable inputSystem = default;
        [SerializeField]
        private BoolVariable clampLookRotation = default;
        [SerializeField]
        private PlayerStateVariable playerState = default;
        [SerializeField]
        private Light inspectorLight = default;
        [SerializeField]
        private Camera playerCamera = default;
        [SerializeField]
        private GameObject inspectUIElements = default;
        [SerializeField]
        private GameObject cantTakePopup = default;
        [SerializeField]
        private TextMeshProUGUI inspectText = default;
        private WaitForSeconds cantTakePoupWaitForSeconds;
        private Quaternion originalItemRotation;
        private Vector3 originalItemPosition;
        private Vector2 scrollValue;

        [SerializeField]
        private string inspectingItemTemporaryLayer = "Inspecting";
        private string inspectingItemName;
        [SerializeField]
        private float maxPickUpDistance = 2.25f;
        [SerializeField]
        private float pickUpItemPositionFromPlayer = 1.25f;
        [SerializeField]
        private float itemRotationSpeed = 9;
        [SerializeField]
        private float itemPositionResetSpeed = 15;
        [SerializeField]
        private float itemRotationResetSpeed = 350;
        [SerializeField]
        private float inspectZoomCameraFieldOfViewMultiplier = 0.6f;
        [SerializeField]
        private float cantTakePopupTime = 1;
        private float originalCameraFieldOfView;

        private int originalItemLayer;
        private bool isInspecting;
        private bool storedItemValues;
        private bool immediateItemReset;
        private bool isResettingItem;

        private void Awake()
        {
            cantTakePoupWaitForSeconds = new WaitForSeconds(cantTakePopupTime);
            originalCameraFieldOfView = playerCamera.fieldOfView;
        }

        private void OnEnable()
        {
            inputSystem.Value.Player.Inspect.Enable();
            inputSystem.Value.Player.Inspect.performed += InspectPerformedAsync;
            inputSystem.Value.Player.InspectRotate.Enable();
            inputSystem.Value.Player.InspectRotate.performed += InspectRotatePerformed;
            inputSystem.Value.Player.InspectZoom.Enable();
            inputSystem.Value.Player.InspectZoom.performed += InspectZoomPerformed;
            inputSystem.Value.Player.InspectZoom.canceled += InspectZoomCanceled;
            inputSystem.Value.Player.TakeInspectItem.Enable();
        }

        private void InspectRotatePerformed(InputAction.CallbackContext callback)
            => scrollValue = callback.ReadValue<Vector2>();

        private void InspectZoomPerformed(InputAction.CallbackContext callback)
        {
            if (isInspecting)
            {
                playerCamera.fieldOfView *= inspectZoomCameraFieldOfViewMultiplier;
            }
        }

        private void InspectZoomCanceled()
        {
            if (isInspecting)
            {
                playerCamera.fieldOfView = originalCameraFieldOfView;
            }
        }

        private void InspectZoomCanceled(InputAction.CallbackContext callback)
        {
            if (isInspecting)
            {
                playerCamera.fieldOfView = originalCameraFieldOfView;
            }
        }

        private async void InspectPerformedAsync(InputAction.CallbackContext callback)
        {
            if (isInspecting || isResettingItem || controlType.Value == ControlType.ThirdPerson)
            {
                isInspecting = false;
                return;
            }

            Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hitInfo, maxPickUpDistance);
            if (!isInspecting && hitInfo.collider != null
                && hitInfo.collider.TryGetComponent(out IInspectable inspectable))
            {
                isInspecting = true;
                inspectingItemName = inspectable.GetName();
                Transform inspectingItem = inspectable.GetInspectTransform();
                StoreItemInfo(inspectingItem);
                await ItemHoverAsync(inspectingItem);
            }
        }

        private void StoreItemInfo(Transform inspectingItem)
        {
            originalItemRotation = inspectingItem.rotation;
            originalItemPosition = inspectingItem.position;
            if (!storedItemValues)
            {
                storedItemValues = true;
                inspectorLight.enabled = true;
                originalItemLayer = inspectingItem.gameObject.layer;
                inspectingItem.gameObject.layer = LayerMask.NameToLayer(inspectingItemTemporaryLayer);
            }
        }

        private async Task ItemHoverAsync(Transform inspectingItem)
        {
            ITakeable takeable;
            ItemHoverStart();
            while (isInspecting && controlType.Value == ControlType.FirstPerson)
            {
                if (immediateItemReset)
                {
                    ResetItemTransform(inspectingItem);
                    break;
                }
                else if (inputSystem.Value.Player.TakeInspectItem.triggered)
                {
                    if (takeable != null)
                    {
                        AddItemToInventory();
                        break;
                    }
                    else StartCoroutine(CantTakePopup());
                }

                inspectingItem.position = playerCamera.transform.position + playerCamera.transform.forward * pickUpItemPositionFromPlayer;
                inspectingItem.rotation = Quaternion.Slerp(inspectingItem.rotation,
                               Quaternion.Euler(HoverRotate(inspectingItem)), itemRotationSpeed * Time.deltaTime);
                scrollValue = Vector2.zero;
                await TaskDelay(Time.deltaTime * 500);
            }

            ItemHoverEnd();
            if (inspectingItem != null) await ResetItemAsync(inspectingItem);

            #region ItemHover Local Functions
            void ItemHoverStart()
            {
                takeable = inspectingItem.GetComponent<ITakeable>();
                inspectUIElements.SetActive(true);
                inspectText.text = inspectingItemName;
                inspectText.gameObject.SetActive(true);
                inputSystem.Value.Player.Movement.Disable();
                inputSystem.Value.Player.Jump.Disable();
                clampLookRotation.Value = true;
                playerState.CurrentPlayerSpecialState = PlayerSpecialState.Inspecting;
                SetInitialRotation(inspectingItem);
            }

            void ItemHoverEnd()
            {
                if (inspectUIElements != null) inspectUIElements.SetActive(false);
                if (inspectText != null) inspectText.gameObject.SetActive(false);

                InspectZoomCanceled();
                playerState.CurrentPlayerSpecialState = PlayerSpecialState.None;
                clampLookRotation.Value = false;
                inputSystem.Value.Player.Movement.Enable();
                inputSystem.Value.Player.Jump.Enable();
                isInspecting = false;
            }

            void AddItemToInventory()
            {
                InventoryManager.Instance.AddItems(takeable.GetInventoryItem());
                Destroy(inspectingItem.gameObject);
            }
            #endregion
        }

        private IEnumerator CantTakePopup()
        {
            cantTakePopup.SetActive(true);
            yield return cantTakePoupWaitForSeconds;
            cantTakePopup.SetActive(false);
        }

        private void SetInitialRotation(Transform inspectingItem)
        {
            Vector3 lookDirection = (transform.position - inspectingItem.position).normalized * -1; // Invert look (temporary fix for letters being backwards as they're inspected)
            inspectingItem.rotation = Quaternion.LookRotation(lookDirection);
        }

        private Vector3 HoverRotate(Transform inspectingItem)
        {
            if (scrollValue.y > 0)
            {
                return new Vector3(inspectingItem.rotation.eulerAngles.x,
                                    inspectingItem.rotation.eulerAngles.y,
                                    inspectingItem.rotation.eulerAngles.z + scrollValue.y);
            }

            return new Vector3(inspectingItem.rotation.eulerAngles.x,
                                inspectingItem.rotation.eulerAngles.y + scrollValue.y,
                                inspectingItem.rotation.eulerAngles.z);
        }

        private async Task ResetItemAsync(Transform inspectingItem)
        {
            ResetItemStart();
            while (CheckPositionRotation())
            {
                if (immediateItemReset)
                {
                    ResetItemTransform(inspectingItem);
                    break;
                }

                inspectingItem.position = Vector3.MoveTowards(inspectingItem.position, originalItemPosition,
                    itemPositionResetSpeed * Time.deltaTime);
                inspectingItem.rotation = Quaternion.RotateTowards(inspectingItem.rotation, originalItemRotation,
                    itemRotationResetSpeed * Time.deltaTime);
                await TaskDelay(Time.deltaTime * SPUtility.CommonUpdateMultiplier);
            }

            isResettingItem = false;

            #region ResetItem Local Functions
            void ResetItemStart()
            {
                isResettingItem = true;
                storedItemValues = false;
                inspectingItem.gameObject.layer = originalItemLayer;
                inspectorLight.enabled = false;
            }

            bool CheckPositionRotation()
            {
                if (inspectingItem == null)
                {
                    return false;
                }

                return SPUtility.CheckPosition(inspectingItem.position, originalItemPosition)
                || SPUtility.CheckRotation(inspectingItem.rotation, originalItemRotation);
            }
            #endregion
        }

        private void ResetItemTransform(Transform inspectingItem)
        {
            inspectingItem.position = originalItemPosition;
            inspectingItem.rotation = originalItemRotation;
        }

        private void OnDisable()
        {
            immediateItemReset = true;
            inputSystem.Value.Player.Inspect.Disable();
            inputSystem.Value.Player.Inspect.performed -= InspectPerformedAsync;
            inputSystem.Value.Player.InspectRotate.Disable();
            inputSystem.Value.Player.InspectRotate.performed -= InspectRotatePerformed;
            inputSystem.Value.Player.InspectZoom.Disable();
            inputSystem.Value.Player.InspectZoom.performed -= InspectZoomPerformed;
            inputSystem.Value.Player.InspectZoom.canceled -= InspectZoomCanceled;
            inputSystem.Value.Player.TakeInspectItem.Disable();
        }

        private Task TaskDelay(float delay)
            => Task.Delay(TimeSpan.FromMilliseconds(delay));

        #if UNITY_EDITOR
        private void OnDrawGizmos() 
            => Gizmos.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * maxPickUpDistance);
        #endif
    }
}