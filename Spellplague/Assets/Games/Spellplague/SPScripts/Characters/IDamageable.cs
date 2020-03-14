using Spellplague.Utility;
using UnityEngine;

namespace Spellplague.Characters
{
    public interface IDamageable
    {
        FloatVariable Health { get; set; }
        string GetName();
        void TakeDamage(float damage);
        void DeathEvent();
        Transform GetTransform();
    }
}