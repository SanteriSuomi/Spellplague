using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spellplague.DialogSystem
{

	public class SpeakerUI : MonoBehaviour
	{
		//public Image portrait;
		public GameObject portrait;
		public GameObject namePanel;
		//public Text fullName;

		private DialogCharacter speaker;
		public DialogCharacter Speaker
		{
			get { return speaker; }
			set
			{
				speaker = value;
				//portrait.sprite = speaker.portrait;
				//fullName.text = speaker.fullName;
				portrait.GetComponent<Image>().sprite = speaker.portrait;
				namePanel.GetComponentInChildren<Text>().text = speaker.fullName;
			}
		}

		public bool HasSpeaker()
		{
			return speaker != null;
		}

		public bool SpeakerIs(DialogCharacter character)
		{
			return speaker == character;
		}

		public void Show()
		{
			//gameObject.SetActive(true);
			namePanel.SetActive(true);
			portrait.SetActive(true);
		}

		public void Hide()
		{
			//gameObject.SetActive(false);
			namePanel.SetActive(false);
			portrait.SetActive(false);
		}
	}
}