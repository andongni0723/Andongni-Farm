using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemTooltip itemTooltip;

        [Header("Drag Image")]
        public Image dragitem_IMG;

        [Header("Player Bag UI")]
        [SerializeField] private GameObject bagUI;
        private bool isbagOpened;
        [SerializeField] private SlotUI[] playerSlots;

        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        }

        private void Start()
        {
            // Give a index to all slots
            for (int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }

            isbagOpened = bagUI.activeInHierarchy;
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                OpenBag();
            }
        }

        private void OnBeforeSceneUnloadEvent()
        {
            UpdateSlotHighlight(-1);
        }

        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch (location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }

        ///<summary>
        /// Open and close bag UI (Button Action)
        ///</summary>
        public void OpenBag()
        {
            isbagOpened = !isbagOpened;
            bagUI.SetActive(isbagOpened);
        }
        
        ///<summary>
        /// Update slot highlight display
        ///</summary>
        ///<param name="index">Bag Index</param>
        public void UpdateSlotHighlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if(slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
    }
}

