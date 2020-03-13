using UnityEngine;

namespace Spellplague.DialogSystem
{
    public class TextHintLookAt : MonoBehaviour
    {
        private Transform player;

        private void Awake() => player = FindObjectOfType<Player.Player>().transform;

        private void Update()
        {
            Vector3 lookDirection = -(player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }
    }
}