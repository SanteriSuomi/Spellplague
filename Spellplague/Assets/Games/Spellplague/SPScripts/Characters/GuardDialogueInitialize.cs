using Spellplague.Utility;
using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

namespace Spellplague.DialogSystem
{
	public class GuardDialogueInitialize : MonoBehaviour
	{
		[SerializeField]
		private InputSystemVariable controls = default;

		private Action<CallbackContext> ctxStartConversation;
		private Action<CallbackContext> ctxAdvanceLine;
		private Action<CallbackContext> ctxEndDialog;

		private void Awake() 
			=> InitializeDialogControllerInputs();

		private void InitializeDialogControllerInputs()
		{
			DialogController dialogController = GetComponentInChildren<DialogController>();
			ctxStartConversation = new Action<CallbackContext>(dialogController.StartConversation);
			controls.Value.Player.Inspect.performed += ctxStartConversation;
			ctxAdvanceLine = new Action<CallbackContext>(dialogController.AdvanceLine);
			controls.Value.Player.Inspect.performed += ctxAdvanceLine;
			ctxEndDialog = new Action<CallbackContext>(dialogController.EndDialog);
			controls.Value.UI.Cancel.performed += ctxEndDialog;
			dialogController.gameObject.SetActive(false);
		}

		private void OnDisable() 
			=> UnsubscribeDialogControllerInputs();

		private void UnsubscribeDialogControllerInputs()
		{
			controls.Value.Player.Inspect.performed -= ctxStartConversation;
			controls.Value.Player.Inspect.performed -= ctxAdvanceLine;
			controls.Value.UI.Cancel.performed -= ctxEndDialog;
		}
	}
}