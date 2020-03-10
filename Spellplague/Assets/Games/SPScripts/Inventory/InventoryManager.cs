using Spellplague.Items;
using Spellplague.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Spellplague.Inventory
{
    #pragma warning disable S3168 // Disable task return void warnings, inventory methods do not need to be awaited from elsewhere.
    public class InventoryManager : Singleton<InventoryManager>
    {
        #region Fields and Initialization
        private List<Item> itemList;
        private List<ItemSlot> itemSlotList;

        [SerializeField]
        private RectTransform itemContainer = default;
        [SerializeField]
        private RectTransform itemSlot = default;
        [SerializeField]
        private float asyncTaskDelay = 0;
        [SerializeField]
        private int columnStartPosition = -210;
        [SerializeField]
        private int spaceBetweenColumns = 140;
        [SerializeField]
        private int numberOfColums = 4;
        [SerializeField]
        private int rowStartPosition = 105;
        [SerializeField]
        private int spaceBetweenRows = 105;
        [SerializeField]
        private int numberOfRows = 3;
        private int inventoryMaxCount;

        protected override void Awake()
        {
            base.Awake();
            itemList = new List<Item>();
            itemSlotList = new List<ItemSlot>();
            inventoryMaxCount = numberOfColums * numberOfRows;
        }
        #endregion

        #region Add
        /// <summary>
        /// Add items to the inventory by the Item class.
        /// </summary>
        /// <param name="items"></param>
        public async void AddItems(params Item[] items)
        {
            if (itemSlotList.Count >= inventoryMaxCount)
            {
                Debug.Log("Inventory full.");
                return;
            }

            await RefreshInventory(items);
        }

        private async Task RefreshInventory(params Item[] items)
        {
            int column = columnStartPosition;
            int columnMax = spaceBetweenColumns * (numberOfColums - 1) + columnStartPosition;
            int row = rowStartPosition;

            CalculateItemPosition(ref column, ref row, columnMax);
            await InitializeItems(items, column, row);
        }

        private void CalculateItemPosition(ref int column, ref int row, int columnMax)
        {
            foreach (Item item in itemList)
            {
                column += spaceBetweenColumns;
                if (column > columnMax)
                {
                    column = columnStartPosition;
                    row -= spaceBetweenRows;
                }
            }
        }

        private async Task InitializeItems(Item[] items, int column, int row)
        {
            foreach (Item item in items)
            {
                if (CheckForStack(item)) { continue; }

                itemList.Add(item);
                RectTransform newItemSlot = SlotPosition(column, row);
                SlotImage(item, newItemSlot);
                SlotScript(item, newItemSlot);
                newItemSlot.gameObject.SetActive(true);

                await TaskDelay();
            }
        }

        private bool CheckForStack(Item item)
        {
            ItemSlot stackedItemSlot = itemSlotList.FirstOrDefault(i => i.SlotItem.ItemType == item.ItemType);
            if (stackedItemSlot != null)
            {
                stackedItemSlot.Stack += 1;
                return true;
            }

            return false;
        }

        private RectTransform SlotPosition(int column, int row)
        {
            if (itemSlot == null) return null;
            RectTransform newItemSlot = Instantiate(itemSlot);
            newItemSlot.SetParent(itemContainer);
            newItemSlot.anchoredPosition = new Vector2(column, row);
            return newItemSlot;
        }

        private void SlotImage(Item item, RectTransform newItemSlot)
        {
            Image newItemImage = newItemSlot.GetComponent<Image>();
            newItemImage.sprite = item.Sprite;
        }

        private void SlotScript(Item item, RectTransform newItemSlot)
        {
            ItemSlot itemSlotScript = newItemSlot.GetComponent<ItemSlot>();
            itemSlotScript.SlotItem = item;
            itemSlotScript.Stack = 1;
            itemSlotList.Add(itemSlotScript);
        }
        #endregion

        #region Remove
        /// <summary>
        /// Remove items from the inventory, it is recommended to remove by ItemType enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        public async void RemoveItems<T>(params T[] items)
        {
            foreach (T item in items)
            {
                foreach (ItemSlot itemSlotInList in itemSlotList)
                {
                    if (ItemCheck(item, itemSlotInList)) { break; }
                    await TaskDelay();
                }
            }
        }

        private bool ItemCheck<T>(T item, ItemSlot itemSlotInList)
        {
            switch (item)
            {
                case ItemType itemType:
                    if (itemType != itemSlotInList.SlotItem.ItemType) { break; }
                    else if (itemSlotInList.Stack <= 1)
                    {
                        RemoveItems(itemList.FindAll(i => i.ItemType == itemType).ToArray());
                        RemoveItemSlot(itemSlotInList);
                    }
                    else
                    {
                        itemSlotInList.Stack -= 1;
                    }

                    return true;
                case Item itemClass:
                    if (itemClass != itemSlotInList.SlotItem) { break; }
                    else if (itemSlotInList.Stack <= 1)
                    {
                        RemoveItems(itemList.FindAll(i => i == itemClass).ToArray());
                        RemoveItemSlot(itemSlotInList);
                    }
                    else
                    {
                        itemSlotInList.Stack -= 1;
                    }

                    return true;
                default:
                    Debug.LogWarning($"Shouldn't be here! Perhaps removing items by invalid type? ItemType enum recommended.");
                    break;
            }

            return false;
        }

        private void RemoveItems(params Item[] itemsToBeRemoved)
        {
            foreach (Item item in itemsToBeRemoved) itemList.Remove(item);
        }

        private void RemoveItemSlot(ItemSlot itemSlot)
        {
            itemSlotList.Remove(itemSlot);
            Destroy(itemSlot.gameObject);
        }
        #endregion

        #region Iteration
        /// <summary>
        /// See if inventory contains an item of this type.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public bool Contains(ItemType itemType)
        {
            for (int i = 0; i < itemSlotList.Count; i++)
            {
                if (itemSlotList[i].SlotItem.ItemType == itemType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Find all items of this type and return them as a list.
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public List<Item> Find(ItemType itemType)
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < itemSlotList.Count; i++)
            {
                if (itemSlotList[i].SlotItem.ItemType == itemType)
                {
                    items.Add(itemSlotList[i].SlotItem);
                }
            }

            return items;
        }
        #endregion  

        private Task TaskDelay() 
            => Task.Delay(TimeSpan.FromMilliseconds(asyncTaskDelay));
    }
}