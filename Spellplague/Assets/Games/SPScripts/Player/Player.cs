using Spellplague.Characters;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Spellplague.Player
{
    public class Player : Character
    {
        [SerializeField]
        private Slider healthbar = default;

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            healthbar.value = Health.Value;
        }

        public override void DeathEvent() 
            => SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
}