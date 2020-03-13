using Spellplague.Items;
using Spellplague.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Spellplague.Inventory
{
	public class PlayerInventoryController : MonoBehaviour
	{
		[SerializeField]
		private InputSystemVariable inputSystem = default;
		[SerializeField]
		private PlayerStateVariable playerState = default;
		[SerializeField]
		private GameObject inventoryUI = default;
		[SerializeField]
		private Item[] startingItems = default;

		private void OnEnable()
		{
			inputSystem.Value.Player.Inventory.Enable();
			inputSystem.Value.Player.Inventory.performed += InventoryPerformed;
		}

		private void Start() => StartingItems();

		private void StartingItems()
		{
			if (startingItems.Length > 0)
			{
				InventoryManager.Instance.AddItems(startingItems);
			}
		}

		private void InventoryPerformed(InputAction.CallbackContext callback)
		{
			switch (playerState.CurrentInventoryState)
			{
				case InventoryState.Closed:
					ClosedInventoryState();
					break;

				case InventoryState.Open:
					OpenInventoryState();
					break;

				default:
					Debug.LogWarning("Should not be here.");
					break;
			}
		}

		private void ClosedInventoryState()
		{
			inputSystem.Value.Player.Looking.Disable();
			inventoryUI.SetActive(true);
			playerState.CurrentInventoryState = InventoryState.Open;
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}

		private void OpenInventoryState()
		{
			inputSystem.Value.Player.Looking.Enable();
			Cursor.lockState = CursorLockMode.Locked;
			inventoryUI.SetActive(false);
			playerState.CurrentInventoryState = InventoryState.Closed;
			Cursor.visible = false;
		}

		private void OnDisable()
		{
			inputSystem.Value.Player.Inventory.Disable();
			inputSystem.Value.Player.Inventory.performed -= InventoryPerformed;
		}
	}
}