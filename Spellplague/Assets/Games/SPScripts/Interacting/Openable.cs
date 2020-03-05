using Spellplague.Inventory;
using Spellplague.Items;
using UnityEngine;

namespace Spellplague.Interacting
{
    public enum OpenType
    {
        Open,
        Item
    }

    /// <summary>
    /// Openable objects such as doors and chests objects.
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class Openable : MonoBehaviour, IOpenable, IHasInteractorEvent
    {
        private Animator animator;
        [SerializeField]
        [Tooltip("Specify this openables open type. Open == Can be opened without special requirements. " +
            "Others must be specified.")]
        private OpenType openType = OpenType.Open;
        public OpenType GetOpenType()
        {
            return openType;
        }

        [SerializeField]
        [Tooltip("Item this door openable can be opened with. Only used with the enum Item.")]
        private Item openableItem = default;

        private void Awake() => animator = GetComponent<Animator>();

        private const string openParameterName = "Open";
        private const bool openParameterValue = true;
        public void Open() => animator.SetBool(openParameterName, openParameterValue);

        public void Execute()
        {
            if (openType == OpenType.Open)
            {
                Open();
            }
            else if (InventoryManager.Instance.Contains(openableItem.ItemType))
            {
                Open();
                InventoryManager.Instance.RemoveItems(openableItem.ItemType);
            }
        }
    }
}