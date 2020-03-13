using UnityEngine;

namespace Spellplague.DialogSystem

{

	public class DialogTrigger : MonoBehaviour
	{
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
			if (other.tag == "Player")
			{
				dialogHint.SetActive(true);
				dialogController._dialog = dialog;
				dialogBase.SetActive(true);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
			{
				//dialogController.EndDialog();
				dialogBase.SetActive(false);
				dialogHint.SetActive(false);
			}
		}
	}
}