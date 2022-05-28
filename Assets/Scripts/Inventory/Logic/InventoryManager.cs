using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        [Header("Item Data")]
        public ItemDataList_SO itemDataList_SO;

        [Header("Bag Data")]
        public InventoryBag_SO playerBag;


        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
        }
        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }



        private void Start()
        {
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnDropItemEvent(int ID, Vector3 arg2)
        {
            RemoveItem(ID, 1);
        }

        /// <summary> 
        /// Use Item ID return Item Details
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>     
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetails.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// Add item to player's bag
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isDestory">whether destory the item</param>
        public void AddItem(Item item, bool isDestory)
        {
            // Whether have same Item
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(" ID: " + GetItemDetails(item.itemID).itemID + " Name: " + GetItemDetails(item.itemID).itemName);

            if (isDestory)
            {
                Destroy(item.gameObject);
            }

            // Update UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// Check bag whether have spaces?
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0) // Have space
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Use item ID to find the location of item in bag
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns>-1: The bag doesn't have this item</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID) // Have space
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Add Item at specify index location in bag
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <param name="index">Index</param>
        /// <param name="amount">Amount</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if (index == -1 && CheckBagCapacity()) // The bag doesn't have this item And the bag has spaces
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };

                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0) // Have space
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }
                }
            }
            else // The bag has this item
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// Change item in player bag
        /// </summary>
        /// <param name="fromIndex">Start slot index</param>
        /// <param name="targetIndex">End slot index</param>
        public void SwapItem(int fromIndex, int targerIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[targerIndex];

            if (targetItem.itemAmount != 0) // Have Item
            {
                playerBag.itemList[fromIndex] = targetItem;
                playerBag.itemList[targerIndex] = currentItem;
            }
            else // Null
            {
                playerBag.itemList[fromIndex] = new InventoryItem();
                playerBag.itemList[targerIndex] = currentItem;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// Removes the specified number of bag items
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <param name="removeCount">Remove count</param>
        private void RemoveItem(int ID, int removeCount)
        {
            var index = GetItemIndexInBag(ID);

            if(playerBag.itemList[index].itemAmount > removeCount)
            {
                var amount = playerBag.itemList[index].itemAmount - removeCount;
                var item = new InventoryItem{itemID = ID, itemAmount = amount};
                playerBag.itemList[index] = item;
            }
            else if(playerBag.itemList[index].itemAmount == removeCount)
            {
                var item = new InventoryItem{};
                playerBag.itemList[index] = item;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}