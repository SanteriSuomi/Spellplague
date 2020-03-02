using UnityEngine;
using UnityEngine.UI;

namespace Spellplague.DialogSystem
{
	public class SpeakerUI : MonoBehaviour
	{
		public Image portrait;
		public Text fullName;

		private DialogCharacter speaker;
		public DialogCharacter Speaker
		{
			get { return speaker; }
			set
			{
				speaker = value;
				portrait.sprite = speaker.portrait;
				fullName.text = speaker.fullName;
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
			gameObject.SetActive(true);
		}

		public void Hide()
		{
			gameObject.SetActive(false);
		}
	}
}