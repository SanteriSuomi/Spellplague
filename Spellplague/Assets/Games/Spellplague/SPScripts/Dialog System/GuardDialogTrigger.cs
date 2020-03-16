using Spellplague.Utility;
using System.Collections;
using UnityEngine;

namespace Spellplague.DialogSystem
{
	public class GuardDialogTrigger : MonoBehaviour
	{
		private enum AnimBlendDirection
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

		private bool hasSpokenOnce;

		private void Start()
		{
			dialogController = dialogBase.GetComponentInChildren<DialogController>();
			guardTransform = transform.parent;
		}

		private void OnTriggerEnter(Collider collision)
		{
			DialogEnterEvent(collision);
		}

		private void DialogEnterEvent(Collider collision)
		{
			if (collision.CompareTag("Player"))
			{
				StartCoroutine(BlendAnimation(AnimBlendDirection.Up));
				DisablePlayerInput();
				dialogController._dialog = dialog;
				dialogBase.SetActive(true);
			}
		}

		private void DisablePlayerInput()
		{
			if (!hasSpokenOnce)
			{
				hasSpokenOnce = true;
				inputSystem.Value.Player.Movement.Disable(); // Disable movement. Gets re-enabled in DialogController.
			}

			inputSystem.Value.Player.ThirdPerson.Disable();
			inputSystem.Value.Player.ThirdPersonZoom.Disable();
		}

		private void OnTriggerExit(Collider collision)
		{
			DialogExitEvent(collision);
		}

		private void DialogExitEvent(Collider collision)
		{
			if (collision.CompareTag("Player"))
			{
				StartCoroutine(BlendAnimation(AnimBlendDirection.Down));
				inputSystem.Value.Player.ThirdPerson.Enable();
				inputSystem.Value.Player.ThirdPersonZoom.Enable();
				dialogBase.SetActive(false);
			}
		}

		private void OnTriggerStay(Collider collision)
		{
			if (collision.CompareTag("Player"))
			{
				LookTowardsPlayer(collision);
			}
		}

		private void LookTowardsPlayer(Collider collision)
		{
			Vector3 lookDirection = (collision.transform.position - guardTransform.position).normalized;
			lookDirection.y = 0;
			Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
			guardTransform.rotation = Quaternion.Slerp(guardTransform.rotation, lookRotation, 10 * Time.deltaTime);
		}

		private IEnumerator BlendAnimation(AnimBlendDirection direction)
		{
			if (direction == AnimBlendDirection.Up)
			{
				while (guardAnimator.GetFloat(animBlendId) < 1)
				{
					guardAnimator.SetFloat(animBlendId, guardAnimator.GetFloat(animBlendId) 
						+ (Time.deltaTime * animBlendingMultiplier));
					yield return null;
				}
			}
			else if (direction == AnimBlendDirection.Down)
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