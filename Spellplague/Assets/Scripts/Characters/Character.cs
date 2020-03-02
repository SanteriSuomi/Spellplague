using Spellplague.Utility;
using UnityEngine;

namespace Spellplague.Characters
{
    /// <summary>
    /// Base class for characters that have health and are killable, contains base class functionality such as health and death checking.
    /// </summary>
    public class Character : MonoBehaviour, IDamageable
    {
        [SerializeField]
        private FloatVariable health = default;
        public FloatVariable Health
        {
            get { return health; }
            set { health = value; }
        }

        public virtual void TakeDamage(float damage)
        {
            Health.Value -= damage;
            if (health.Value <= 0)
            {
                DeathEvent();
            }
        }

        public virtual void DeathEvent()
        {
            Destroy(gameObject);
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}