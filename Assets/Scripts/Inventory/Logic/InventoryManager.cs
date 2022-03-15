using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnFarm.Inventory
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public ItemDataList_SO itemDataList_SO;


        /// <summary> 
        /// Use Item ID return Item Details
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>     
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetails.Find(i => i.itemID == ID);
        }
    }
}