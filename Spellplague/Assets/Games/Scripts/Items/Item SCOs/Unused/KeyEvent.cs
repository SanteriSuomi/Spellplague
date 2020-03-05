using Spellplague.Interacting;
using UnityEngine;

namespace Spellplague.ItemEvents
{
    public class KeyEvent : ItemEvent
    {
        private Transform player;
        [SerializeField]
        private float keySphereCheckRadius = 1.5f;
        [SerializeField]
        private string openTag = "Door";

        private void Awake() => player = FindObjectOfType<Player.Player>().transform;

        public override bool Execute()
        {
            Collider[] hits = Physics.OverlapSphere(player.position, keySphereCheckRadius);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Collider hit = hits[i];
                    if (hit.CompareTag(openTag) && hit.TryGetComponent(out IOpenable openable))
                    {
                        openable.Open();
                        return true;
                    }
                }
            }

            return false;
        }
    }
}