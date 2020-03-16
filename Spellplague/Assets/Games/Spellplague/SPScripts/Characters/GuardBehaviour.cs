using System.Collections;
using UnityEngine;

namespace Spellplague.Characters
{
    public class GuardBehaviour : MonoBehaviour
    {
        private Transform player;
        private WaitForSeconds distanceCheckerUpdateWFS;
        [SerializeField]
        private float minSqrDistanceUntilDestroy = 100;
        [SerializeField]
        private float checkDistanceEverySec = 1;

        private void Awake()
        {
            player = FindObjectOfType<Player.Player>().transform;
            distanceCheckerUpdateWFS = new WaitForSeconds(checkDistanceEverySec);
            StartCoroutine(LeaveWhenPlayerGoesFar());
        }

        private IEnumerator LeaveWhenPlayerGoesFar()
        {
            while (enabled)
            {
                float sqrDistanceFromPlayer = (player.position - transform.position).sqrMagnitude;
                if (sqrDistanceFromPlayer >= minSqrDistanceUntilDestroy)
                {
                    gameObject.SetActive(false);
                }

                yield return distanceCheckerUpdateWFS;
            }
        }
    }
}