using UnityEngine;

namespace Spellplague.Utility
{
    public enum ControlType
    {
        FirstPerson,
        ThirdPerson
    }

    [CreateAssetMenu(fileName = "Control Type Asset", menuName = "ScriptableObjects/Variables/New Control Type Asset", order = 1)]
    public class ControlTypeVariable : BaseScriptableObject<ControlType>
    {
    }
}