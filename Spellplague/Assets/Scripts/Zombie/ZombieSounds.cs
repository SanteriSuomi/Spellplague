using UnityEngine;

namespace Spellplague.AI
{
    public class ZombieSounds : MonoBehaviour
    {
        public AudioSource zombieSound;
        public AudioClip[] zombieWalking;
        public AudioClip zombieDying;
        public AudioClip zombieAttacking;

        void Awake()
        {
            zombieSound = GetComponent<AudioSource>();
            isWalking();
        }

        public void isWalking()
        {
            CallWalkAudio();
        }

        void isAttacking()
        {
            CallAttackAudio();
        }

        void isDying()
        {
            zombieSound.clip = zombieDying;
            zombieSound.Play();
        }

        void ZombieAttackSound()
        {
            zombieSound.clip = zombieAttacking;
            zombieSound.Play();
            CallAttackAudio();
        }

        void RandomZombieSound()
        {
            zombieSound.clip = zombieWalking[Random.Range(0, zombieWalking.Length)];
            zombieSound.Play();
            CallWalkAudio();
        }

        void CallAttackAudio()
        {
            Invoke("ZombieAttackSound", 5);
        }

        void CallWalkAudio()
        {
            Invoke("RandomZombieSound", 10);
        }
    }
}
