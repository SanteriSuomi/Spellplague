using System.Collections.Generic;
using UnityEngine;

namespace Spellplague.DialogSystem
{
	public class DecisionController : MonoBehaviour
	{
		public Decision decision;
		public GameObject[] choiceButtons;

		private List<ChoiceController> choiceControllers = new List<ChoiceController>();

		public void Change(Decision activeDecision)
		{
			RemoveChoices();
			decision = activeDecision;
			gameObject.SetActive(true);
			Initialize();
		}

		public void Hide(Dialog dialog)
		{
			RemoveChoices();
			gameObject.SetActive(false);
		}

		private void RemoveChoices()
		{
			foreach (ChoiceController cc in choiceControllers)
				cc.gameObject.GetComponent<ChoiceController>().choice = default;

			choiceControllers.Clear();
		}

		private void Initialize()
		{
			for (int index = 0; index < decision.choices.Length; index++)
			{
				ChoiceController cc = ChoiceController.AddChoiceButton(choiceButtons[index], decision.choices[index]);
				choiceControllers.Add(cc);
			}
		}
	}
}
