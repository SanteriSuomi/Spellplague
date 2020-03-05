using UnityEngine;

namespace Spellplague.Utility
{
    [CreateAssetMenu(fileName = "Input System Asset", menuName = "ScriptableObjects/Variables/New Input System Asset", order = 3)]
    public class InputSystemVariable : BaseScriptableObject<InputSystem>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            value = new InputSystem();
        }
    }
}