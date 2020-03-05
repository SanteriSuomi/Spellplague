using UnityEngine;

namespace Spellplague.Items
{
    public enum ItemType
    {
        Key
    }

    [CreateAssetMenu(fileName = "Item", menuName = "New Item", order = 1)]
    public class Item : ScriptableObject
    {
        [SerializeField]
        private ItemType itemType;
        public ItemType ItemType
        {
            get => itemType;
            private set => itemType = value;
        }

        [SerializeField]
        private GameObject prefab;
        public GameObject Prefab
        {
            get => prefab;
            set => prefab = value;
        }
        public GameObject InstantiatedPrefab { get; set; }

        /// <summary>
        /// Execute this item prefab's event. Only one event per item. To create an event, make a new script, 
        /// attach it to the object, and inherit from the IExecutableItem interface, then override the Execute method.
        /// </summary>
        /// <returns></returns>
        public bool ExecuteItemEvent()
        {
            if (InstantiatedPrefab.TryGetComponent(out IExecutableItem item))
            {
                return item.Execute();
            }

            return false;
        }

        [SerializeField]
        private Sprite sprite;
        public Sprite Sprite
        {
            get => sprite;
            private set => sprite = value;
        }

        [SerializeField]
        private string description;
        public string Description
        {
            get => description;
            private set => description = value;
        }

        /// <summary>
        /// Create a new scriptable object item instance for use in inventory on runtime.
        /// </summary>
        /// <param name="itemType"></param>
        /// <param name="prefab"></param>
        /// <param name="sprite"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Item Create(ItemType itemType, GameObject prefab, Sprite sprite, string description)
        {
            Item item = CreateInstance<Item>();
            item.ItemType = itemType;
            item.Prefab = prefab;
            item.Sprite = sprite;
            item.Description = description;
            return item;
        }
    }
}