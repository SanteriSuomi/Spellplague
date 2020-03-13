using Spellplague.Utility;
using System.Collections;
using UnityEngine;

namespace Spellplague.DialogSystem

{

	public class DialogTrigger : MonoBehaviour
	{
		private enum BlendDirection
		{
			Up,
			Down
		}

		[SerializeField]
		private InputSystemVariable inputSystem = default;
		[SerializeField]
		private Animator guardAnimator = default;
		[SerializeField]
		private string blendId = "Blend";
		[SerializeField]
		private float blendingMultiplier = 1.5f;

		public Dialog dialog;
		public GameObject dialogBase;
		private DialogController dialogController;

		void Start()
		{
			dialogController = dialogBase.GetComponentInChildren<DialogController>();
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(BlendAnim(BlendDirection.Up));
				inputSystem.Value.Player.ThirdPerson.Disable();
				inputSystem.Value.Player.ThirdPersonZoom.Disable();
				dialogController._dialog = dialog;
				dialogBase.SetActive(true);
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(BlendAnim(BlendDirection.Down));
				inputSystem.Value.Player.ThirdPerson.Enable();
				inputSystem.Value.Player.ThirdPersonZoom.Enable();
				dialogBase.SetActive(false);
			}
		}

		private IEnumerator BlendAnim(BlendDirection direction)
		{
			if (direction == BlendDirection.Up)
			{
				while (guardAnimator.GetFloat(blendId) < 1)
				{
					guardAnimator.SetFloat(blendId, guardAnimator.GetFloat(blendId) 
						+ (Time.deltaTime * blendingMultiplier));
					yield return null;
				}
			}
			else
			{
				while (guardAnimator.GetFloat(blendId) > 0)
				{
					guardAnimator.SetFloat(blendId, guardAnimator.GetFloat(blendId) 
						- (Time.deltaTime * blendingMultiplier));
					yield return null;
				}
			}
		}
	}
}