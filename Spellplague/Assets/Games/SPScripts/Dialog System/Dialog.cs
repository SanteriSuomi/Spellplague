using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spellplague.DialogSystem
{

	[System.Serializable]
	public struct Line

	{
		//public bool leftCharacter;
		//public bool rightCharacter;
		public DialogCharacter character;

		[TextArea(2, 5)]
		public string text;
	}


	[CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog System/New Dialog")]

	public class Dialog : ScriptableObject

	{
		public DialogCharacter speakerLeft;
		public DialogCharacter speakerRight;
		public Decision decision;
		public Dialog nextDialog;
		public Line[] lines;
	}
}