using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Spellplague.DialogSystem

{

	public class DialogTrigger : MonoBehaviour
	{
		private GameObject dialogCanvas;
		public GameObject dialogHint;

		private void Start()
		{
			dialogCanvas = transform.GetChild(0).gameObject;
		}
		void OnTriggerEnter(Collider other)
		{
			if (other.tag == "Player")
				dialogCanvas.SetActive(true);
				dialogHint.SetActive(true);
		}

		void OnTriggerExit(Collider other)
		{
			if (other.tag == "Player")
				dialogCanvas.SetActive(false);
				dialogHint.SetActive(false);
		}
	}
}
