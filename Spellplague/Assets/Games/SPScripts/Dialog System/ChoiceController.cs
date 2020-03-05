using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Spellplague.DialogSystem
{
	[System.Serializable]
	public class DialogChangeEvent : UnityEvent<Dialog> { }

	public class ChoiceController : MonoBehaviour
	{
		public Choice choice;
		public DialogChangeEvent dialogChangeEvent;

		public static ChoiceController AddChoiceButton(GameObject choiceButton, Choice _choice)
		{
			choiceButton.SetActive(true);
			ChoiceController choiceController = choiceButton.GetComponent<ChoiceController>();
			choiceController.choice = _choice;
			return choiceController;
		}

		private void Start()
		{
			if (dialogChangeEvent == null)
				dialogChangeEvent = new DialogChangeEvent();

			GetComponent<Button>().GetComponentInChildren<Text>().text = choice.text;
		}

		public void MakeChoice()
		{
			dialogChangeEvent.Invoke(choice.dialog);
		}
	}
}
