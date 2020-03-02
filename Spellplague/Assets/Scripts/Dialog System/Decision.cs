using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spellplague.DialogSystem

{

	[System.Serializable]

	public struct Choice
	{
		[TextArea(2, 5)]
		public string text;
		public Dialog dialog;
	}

	[CreateAssetMenu(fileName = "New Decision", menuName = "Dialog System/New Decision")]

	public class Decision : ScriptableObject
	{
		public Choice[] choices;
	}
}