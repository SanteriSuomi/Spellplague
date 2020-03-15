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
		private Transform guardTransform = default;

		[SerializeField]
		private string animBlendId = "Blend";
		[SerializeField]
		private float animBlendingMultiplier = 1.5f;

		public Dialog dialog;
		public GameObject dialogBase;
		private DialogController dialogController;

		private void Start()
		{
			dialogController = dialogBase.GetComponentInChildren<DialogController>();
			guardTransform = transform.parent;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(BlendAnimation(BlendDirection.Up));
				inputSystem.Value.Player.ThirdPerson.Disable();
				inputSystem.Value.Player.ThirdPersonZoom.Disable();
				dialogController._dialog = dialog;
				dialogBase.SetActive(true);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				StartCoroutine(BlendAnimation(BlendDirection.Down));
				inputSystem.Value.Player.ThirdPerson.Enable();
				inputSystem.Value.Player.ThirdPersonZoom.Enable();
				dialogBase.SetActive(false);
			}
		}

		private void OnTriggerStay(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				LookTowardsPlayer(other);
			}
		}

		private void LookTowardsPlayer(Collider other)
		{
			Vector3 lookDirection = (other.transform.position - guardTransform.position).normalized;
			lookDirection.y = 0;
			Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
			guardTransform.rotation = Quaternion.Slerp(guardTransform.rotation, lookRotation, 10 * Time.deltaTime);
		}

		private IEnumerator BlendAnimation(BlendDirection direction)
		{
			if (direction == BlendDirection.Up)
			{
				while (guardAnimator.GetFloat(animBlendId) < 1)
				{
					guardAnimator.SetFloat(animBlendId, guardAnimator.GetFloat(animBlendId) 
						+ (Time.deltaTime * animBlendingMultiplier));
					yield return null;
				}
			}
			else if (direction == BlendDirection.Down)
			{
				while (guardAnimator.GetFloat(animBlendId) > 0)
				{
					guardAnimator.SetFloat(animBlendId, guardAnimator.GetFloat(animBlendId) 
						- (Time.deltaTime * animBlendingMultiplier));
					yield return null;
				}
			}
		}
	}
}