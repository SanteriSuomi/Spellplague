using UnityEngine;

namespace Spellplague.Items
{
    /// <summary>
    /// Game objects with this script can be inspected by the player.
    /// </summary>
    public class Inspectable : MonoBehaviour, IInspectable
    {
        [SerializeField]
        [Tooltip("Name that shows up in the inspecting mode.")]
        private string inspectorItemName = default;
        public string GetName()
        {
            return inspectorItemName;
        }

        public Transform GetInspectTransform()
        {
            return transform;
        }
    }
}