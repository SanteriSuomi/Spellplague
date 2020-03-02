using Spellplague.Items;
using UnityEngine;

namespace Spellplague.ItemEvents
{
    /// <summary>
    /// Inherit from this class for new inventory item events (when item gets used in the inventory). If Execute returns true, item gets removed, else it won't.
    /// </summary>
    public class ItemEvent : MonoBehaviour, IExecutableItem
    {
        public virtual bool Execute()
        {
            Debug.Log("Don't forget to override this.");
            return false;
        }
    }
}