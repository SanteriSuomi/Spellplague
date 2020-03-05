using Spellplague.Utility;
using UnityEngine;

namespace Spellplague.Player
{
    public class PlayerHoldableController : MonoBehaviour
    {
        [SerializeField]
        private ControlTypeVariable controlType = default;
        [SerializeField]
        private Camera playerCamera = default;
        [SerializeField]
        private Transform playerTransform = default;
        [SerializeField]
        private Vector3 thirdPersonOffset = new Vector3(0, 0.75f, 0);
        private Vector3 originalPosition;
        [SerializeField]
        private float pointForwardsSpeed = 5;

        private void Awake() => originalPosition = transform.localPosition;

        private void OnEnable() => controlType.ValueChangedEvent += ControlTypeChanged;

        private void ControlTypeChanged(ControlType value)
        {
            if (value == ControlType.FirstPerson)
            {
                ChangeHoldableParent(playerCamera.transform, originalPosition);
            }
            else
            {
                ChangeHoldableParent(playerTransform, originalPosition + thirdPersonOffset);
            }
        }

        private void ChangeHoldableParent(Transform newParent, Vector3 newLocalPosition)
        {
            transform.SetParent(newParent);
            transform.localPosition = newLocalPosition;
        }

        private void Update() => PointForwards();

        private void PointForwards()
        {
            Quaternion lookRotation = Quaternion.LookRotation(playerCamera.transform.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, pointForwardsSpeed * Time.deltaTime);
        }

        private void OnDisable() => controlType.ValueChangedEvent -= ControlTypeChanged;
    }
}