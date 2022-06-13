using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private int harvestActionCount;

    public void ProcessToolAction(ItemDetails tool)
    {
        // Tool use count
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if(requireActionCount == -1) return;

        // has anim? (tree)

        // Click timer
        if(harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            // Play vfx
            // Play se
        }

        if(harvestActionCount >= requireActionCount)
        {
            if(cropDetails.generateAtPlayerPosition)
            {
                // Instiance item
                SpawnHarvestItems();
            }
        }
    }

    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduct;
            if(cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                // Min count
                amountToProduct = cropDetails.producedMinAmount[i];
            }
            else
            {
                // Random count
                amountToProduct = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
            }

            // Excute the item of "amountToProduct"
            for (int j = 0; j < amountToProduct; j++)
            {
                if(cropDetails.generateAtPlayerPosition)
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
            }
        }
    }
}
