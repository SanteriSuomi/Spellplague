using Spellplague.Items;
using Spellplague.Utility;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Spellplague.Inventory
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, 
        IBeginDragHandler, IDragHandler, IPointerUpHandler
    {
        public Item SlotItem { get; set; }
        [SerializeField]
        private TextMeshProUGUI stackText = default;
        [SerializeField]
        private RectTransform backgroundRectTransform = default;
        [SerializeField]
        PointerEventData.InputButton useItemButton = default;
        private RectTransform localRectTransform;
        private Transform otherItemSlotCollision;
        private Transform localSlotTransform;
        private Vector2 originalPositionBeforeDrag;

        [SerializeField]
        private string itemSlotTagString = "ItemSlot";
        [SerializeField]
        private float itemDragConfineMultiplier = 3;
        [SerializeField]
        private float itemSlotMoveSmooth = 1250;
        [SerializeField]
        private float itemSlotMoveUpdateRate = 8.33f;

        private int stack;
        public int Stack
        {
            get => stack;
            set
            {
                stack = value;
                stackText.text = $"{stack}";
            }
        }

        private bool isDraggingItemSlot;
        private bool isHoldingItemLot;
        private bool isTouchingOtherItemSlot;
        private bool isLerpingItemSlot;

        private void Awake()
        {
            if (SlotItem.Prefab != null)
            {
                SlotItem.InstantiatedPrefab = Instantiate(SlotItem.Prefab, transform.up * -1000, Quaternion.identity);
            }

            localSlotTransform = transform;
            localRectTransform = GetComponent<RectTransform>();
            originalPositionBeforeDrag = localSlotTransform.position;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == useItemButton && SlotItem.ExecuteItemEvent())
            {
                InventoryManager.Instance.RemoveItems(SlotItem.ItemType);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (isLerpingItemSlot) return;
            originalPositionBeforeDrag = localSlotTransform.position;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (isLerpingItemSlot) return;
            isDraggingItemSlot = true;
            isHoldingItemLot = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (isLerpingItemSlot) return;
            Vector2 pointerPositionRelativeToBackground
                = backgroundRectTransform.InverseTransformPoint(eventData.position);
            if (IsInBounds(pointerPositionRelativeToBackground))
            {
                localSlotTransform.position = eventData.position;
            }
        }

        private bool IsInBounds(Vector2 pointerPosRelativeToBackground)
        {
            return !(pointerPosRelativeToBackground.x > (backgroundRectTransform.sizeDelta.x / 2)
                   - (localRectTransform.sizeDelta.x / itemDragConfineMultiplier)
                  || pointerPosRelativeToBackground.x < -(backgroundRectTransform.sizeDelta.x / 2)
                   + (localRectTransform.sizeDelta.x / itemDragConfineMultiplier)
                  || pointerPosRelativeToBackground.y > (backgroundRectTransform.sizeDelta.y / 2)
                   - (localRectTransform.sizeDelta.y / itemDragConfineMultiplier)
                  || pointerPosRelativeToBackground.y < -(backgroundRectTransform.sizeDelta.y / 2)
                   + (localRectTransform.sizeDelta.y / itemDragConfineMultiplier));
        }

        public async void OnPointerUp(PointerEventData eventData)
        {
            await MoveSlot();
            isDraggingItemSlot = false;
            isHoldingItemLot = false;
            isTouchingOtherItemSlot = false;
        }

        private async Task MoveSlot()
        {
            if (isDraggingItemSlot && isTouchingOtherItemSlot)
            {
                Task localSlotTask = SlotLerp(localSlotTransform, otherItemSlotCollision.position);
                Task otherSlotTask = SlotLerp(otherItemSlotCollision, originalPositionBeforeDrag);
                await Task.WhenAll(localSlotTask, otherSlotTask);
            }
            else if (isDraggingItemSlot)
            {
                await SlotLerp(localSlotTransform, originalPositionBeforeDrag);
            }
        }

        private async Task SlotLerp(Transform itemSlot, Vector2 toPosition)
        {
            isLerpingItemSlot = true;
            while (itemSlot != null &&
                SPUtility.CheckPosition(itemSlot.position, toPosition))
            {
                itemSlot.position = Vector2.MoveTowards(itemSlot.position, toPosition, itemSlotMoveSmooth * Time.deltaTime);
                await Task.Delay(TimeSpan.FromMilliseconds(itemSlotMoveUpdateRate));
            }

            isLerpingItemSlot = false;
        }

        private void OnTriggerEnter2D(Collider2D collision) 
            => ToggleTouchBool(collision, isTouching: true);

        private void OnTriggerExit2D(Collider2D collision) 
            => ToggleTouchBool(collision, isTouching: false);

        private void ToggleTouchBool(Collider2D collision, bool isTouching)
        {
            if (isHoldingItemLot && collision.gameObject.CompareTag(itemSlotTagString))
            {
                isTouchingOtherItemSlot = isTouching;
                if (isTouching)
                {
                    otherItemSlotCollision = collision.transform;
                }
            }
        }

        private void OnDestroy()
        {
            if (SlotItem.InstantiatedPrefab is null) { return; }
            Destroy(SlotItem.InstantiatedPrefab);
        }
    }
}