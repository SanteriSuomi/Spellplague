using UnityEngine;

namespace Spellplague.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerGravity : MonoBehaviour
    {
        public delegate void IsGrounded(bool isGrounded);
        public event IsGrounded IsGroundedEvent;

        [SerializeField]
        private Transform groundedTransform = default;
        private CharacterController characterController;
        [SerializeField]
        private LayerMask groundedLayersToHit = default;
        private Vector3 gravity;

        [SerializeField]
        private float gravityMultiplier = 3.75f;
        [SerializeField]
        private float groundedCheckRadius = 0.125f;

        private void Awake() 
            => characterController = GetComponent<CharacterController>();

        private void Update() => Gravity();

        private void Gravity()
        {
            bool isGrounded = CheckGrounding();
            if (!isGrounded)
            {
                gravity += new Vector3(0, Mathf.Pow(Physics.gravity.y, 2) * gravityMultiplier * Time.deltaTime, 0);
                characterController.SimpleMove(gravity);
            }
            else
            {
                gravity = Vector3.zero;
            }
        }

        private bool CheckGrounding()
        {
            bool isGrounded = Physics.CheckSphere(groundedTransform.position, 
                groundedCheckRadius, groundedLayersToHit);
            IsGroundedEvent(isGrounded);
            return isGrounded;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos() => Gizmos.DrawWireSphere(groundedTransform.position, 
            groundedCheckRadius);
        #endif
    }
}