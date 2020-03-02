using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace Spellplague.DialogSystem

{

	[System.Serializable]

	public class DecisionEvent : UnityEvent<Decision> { }

	public class DialogController : MonoBehaviour
	{
		public Dialog dialog;
		public DecisionEvent decisionEvent;

		public GameObject speakerLeft;
		public GameObject speakerRight;
		public GameObject dialogPanel;

		private Dialog activeDialog;

		private SpeakerUI speakerUILeft;
		private SpeakerUI speakerUIRight;

		private Text activeLine;

		private int activeLineIndex;
		private bool dialogStarted = false;

		void Start()
		{
			speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
			speakerUIRight = speakerRight.GetComponent<SpeakerUI>();
			activeLine = dialogPanel.GetComponentInChildren<Text>();
			activeDialog = dialog;
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				AdvanceLine();
			}
		}

		//
		private void Initialize()
		{
			dialogStarted = true;
			activeLineIndex = 0;
			speakerUILeft.Speaker = activeDialog.speakerLeft;
			speakerUIRight.Speaker = activeDialog.speakerRight;
			dialogPanel.SetActive(true);
			Cursor.lockState = CursorLockMode.Confined;
			Cursor.visible = true;
		}

		void AdvanceLine()
		{
			if (activeDialog == null) return;
			if (!dialogStarted) Initialize();
			if (activeLineIndex < activeDialog.lines.Length)
				DisplayLine();
			else
				AdvanceDialog();
		}
		void DisplayLine()
		{
			Line line = activeDialog.lines[activeLineIndex];
			activeLine.text = line.text;
			DialogCharacter character = line.character;

			if (speakerUILeft.SpeakerIs(character))
			{
				speakerUILeft.Show();
				speakerUIRight.Hide();
			}

			else if (speakerUIRight.SpeakerIs(character))
			{
				speakerUIRight.Show();
				speakerUILeft.Hide();
			}

			else
			{
				speakerUILeft.Hide();
				speakerUIRight.Hide();
			}

			activeLineIndex += 1;
		}

		private void AdvanceDialog()
		{
			if (activeDialog.decision != null)
			{
				decisionEvent.Invoke(activeDialog.decision);
				activeLine.text = "";
			}
			else if (activeDialog.nextDialog != null)
				ChangeDialog(activeDialog.nextDialog);
			else
				EndDialog();
		}


		public void ChangeDialog(Dialog nextDialog)
		{
			dialogStarted = false;
			activeDialog = nextDialog;
			AdvanceLine();
		}

		private void EndDialog()
		{
			activeDialog = dialog;
			dialogStarted = false;
			dialogPanel.SetActive(false);
			speakerUILeft.Hide();
			speakerUIRight.Hide();
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

	}
}
