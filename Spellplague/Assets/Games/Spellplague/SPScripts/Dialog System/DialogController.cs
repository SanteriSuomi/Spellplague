﻿using Spellplague.Utility;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace Spellplague.DialogSystem
{
	[Serializable]
	public class DecisionEvent : UnityEvent<Decision> { }

	public class DialogController : MonoBehaviour
	{
		[SerializeField]
		private InputSystemVariable controls = default;
		private WaitForSeconds dialogTextTypeWFS;

		[HideInInspector]
		public Dialog _dialog;
		public DecisionEvent decisionEvent;

		public GameObject speakerLeft;
		public GameObject speakerRight;
		public GameObject dialogPanel;

		private Dialog activeDialog;
		private SpeakerUI speakerUILeft;
		private SpeakerUI speakerUIRight;

		private Text panelText;
		private string activeLine;

		private int activeLineIndex;
		private bool dialogActive = false;
		private bool dialogStarted = false;
		private bool isTyping = false;
		private bool cancelTyping = false;
		public float typeSpeed;

		private void Awake() 
			=> dialogTextTypeWFS = new WaitForSeconds(typeSpeed);

		private void Start()
		{
			speakerUILeft = speakerLeft.GetComponent<SpeakerUI>();
			speakerUIRight = speakerRight.GetComponent<SpeakerUI>();
			panelText = dialogPanel.GetComponentInChildren<Text>();
			activeDialog = _dialog;
			StartConversation(default);
		}

		public void StartConversation(CallbackContext context)
		{
			if (activeDialog == null) return;
			if (!dialogActive)
			{
				dialogActive = true;
				Initialize();
			}
		}

		private void Initialize()
		{
			dialogStarted = true;
			activeLineIndex = 0;
			speakerUILeft.Speaker = activeDialog.speakerLeft;
			speakerUIRight.Speaker = activeDialog.speakerRight;
			dialogPanel.SetActive(true);
			DisplayLine();
		}

		public void AdvanceLine(CallbackContext context)
		{
			if (dialogActive)
			{
				if (!dialogStarted)
					Initialize();
				else if (isTyping)
					cancelTyping = true;
				else if (activeLineIndex < activeDialog.lines.Length)
					DisplayLine();
				else
					AdvanceDialog();
			}
		}

		void DisplayLine()
		{
			Line line = activeDialog.lines[activeLineIndex];
			activeLine = activeDialog.lines[activeLineIndex].text;
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

			if (!isTyping)
				StartCoroutine(TextScroll(activeLine));

		}
		private IEnumerator TextScroll(string line)
		{
			int letter = 0;
			panelText.text = "";
			isTyping = true;
			cancelTyping = false;
			StringBuilder dialogStringBuilder = new StringBuilder();
			while (isTyping && !cancelTyping && (letter < line.Length - 1))
			{
				dialogStringBuilder.Append(line[letter]);
				panelText.text = dialogStringBuilder.ToString();
				letter += 1;
				yield return dialogTextTypeWFS;
			}

			panelText.text = line;
			isTyping = false;
			cancelTyping = false;
			activeLineIndex += 1;
		}
		private void AdvanceDialog()
		{
			if (activeDialog.decision != null)
			{
				decisionEvent.Invoke(activeDialog.decision);
				panelText.text = "";
			}
			else if (activeDialog.nextDialog != null)
				ChangeDialog(activeDialog.nextDialog);
			else
				EndDialog(default);
		}

		public void ChangeDialog(Dialog nextDialog)
		{
			dialogStarted = false;
			activeDialog = nextDialog;
			AdvanceLine(default);
		}

		public void EndDialog(CallbackContext context)
		{
			activeDialog = _dialog;
			dialogActive = false;
			dialogStarted = false;
			cancelTyping = true;
			dialogPanel.SetActive(false);
			speakerUILeft.Hide();
			speakerUIRight.Hide();
			controls.Value.Player.Movement.Enable();
		}
	}
}