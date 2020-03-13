using Spellplague.Utility;
using UnityEngine;

namespace Spellplague.DialogSystem

{

	public class DialogTrigger : MonoBehaviour
	{
		[SerializeField]
		private InputSystemVariable inputSystem = default;

		public Dialog dialog;

		public GameObject dialogBase;
		public GameObject dialogHint;

		private DialogController dialogController;

		void Start()
		{
			dialogController = dialogBase.GetComponentInChildren<DialogController>();
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				inputSystem.Value.Player.ThirdPerson.Disable();
				inputSystem.Value.Player.ThirdPersonZoom.Disable();
				dialogHint.SetActive(true);
				dialogController._dialog = dialog;
				dialogBase.SetActive(true);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				inputSystem.Value.Player.ThirdPerson.Enable();
				inputSystem.Value.Player.ThirdPersonZoom.Enable();
				dialogBase.SetActive(false);
				dialogHint.SetActive(false);
			}
		}
	}
}