using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spellplague.DialogSystem
{

	[CreateAssetMenu(fileName = "New Character", menuName = "Dialog System/New Character")]
	public class DialogCharacter : ScriptableObject
	{
		public string fullName;
		public Sprite portrait;
	}
}