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
            // Bag whether have spaces
            // Whether have same Item
            IntentoryItem newItem = new IntentoryItem();
            newItem.itemID = item.itemID;
            newItem.itemAmount = 1;

            playerBag.itemList[0] = newItem;

            Debug.Log(" ID: " + GetItemDetails(item.itemID).itemID + " Name: " + GetItemDetails(item.itemID).itemName);

            if(isDestory)
            {
                Destroy(item.gameObject);
            }
        }
    }
}