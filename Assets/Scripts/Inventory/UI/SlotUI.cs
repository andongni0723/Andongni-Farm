using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace AnFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("Compontent")]
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amount_T;
        public Image slotHighlight;
        [SerializeField] private Button button;

        [Header("Slot Type")]
        public SlotType slotType;

        public bool isSelected;
        public int slotIndex;

        // Item Details
        public ItemDetails itemDetails;
        public int itemAmount;

        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;

            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// Update slot UI and details 
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">Amount</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = itemDetails.itemIcon;
            itemAmount = amount;
            amount_T.text = amount.ToString();
            button.interactable = true;
            slotImage.enabled = true;
        }

        /// <summary>
        /// Updata slot to empty
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;

                inventoryUI.UpdateSlotHighlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }

            itemDetails = null;
            slotImage.enabled = false;
            amount_T.text = "";
            button.interactable = false;
            itemAmount = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null) return;

            isSelected = !isSelected;
            inventoryUI.UpdateSlotHighlight(slotIndex);

            if(slotType == SlotType.Bag)
            {
                // Call the Details and State of Selecting Item
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (itemDetails == null) return;

            inventoryUI.dragitem_IMG.enabled = true;
            inventoryUI.dragitem_IMG.sprite = slotImage.sprite;
            inventoryUI.dragitem_IMG.SetNativeSize();
            isSelected = true;
            inventoryUI.UpdateSlotHighlight(slotIndex);
        }

        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragitem_IMG.transform.position = Input.mousePosition;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragitem_IMG.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null) return;

                var targerSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targerIndex = targerSlot.slotIndex;

                // Change Item in Player Bag
                if (slotType == SlotType.Bag && targerSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targerIndex);
                }

                inventoryUI.UpdateSlotHighlight(-1); // Close the index highlight
            }
            // else // Test Drop item on the ground
            // {
            //     if (itemDetails.canDropped)
            //     {
            //         // Mouse Position to World Position
            //         var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            //         EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //     }
            // }
        }
    }
}