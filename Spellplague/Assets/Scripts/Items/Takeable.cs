using UnityEngine;

namespace Spellplague.Items
{
    /// <summary>
    /// Game objects with this script can be taken by the player and are stored in the inventory.
    /// </summary>
    public class Takeable : MonoBehaviour, ITakeable
    {
        [SerializeField]
        [Tooltip("Scriptable object inventory item. When this object gets picked up to the inventory, it's settings are in this scriptable object.")]
        private Item inventoryItem = default;

        public Item GetInventoryItem()
        {
            return inventoryItem;
        }
    }
}