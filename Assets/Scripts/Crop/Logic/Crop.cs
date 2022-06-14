using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private TileDetails tileDetails;
    private int harvestActionCount;

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;
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

            // Instiance the item of "amountToProduct" and Excuse
            for (int j = 0; j < amountToProduct; j++)
            {
                if(cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else // Instiance on the world
                {
                    
                }
            }
        }

        if(tileDetails != null)
        {
            tileDetails.daysSinceLastMarvest++;

            // Is crop can regrow?
            if(cropDetails.dayToRegrow > 0 && tileDetails.daysSinceLastMarvest < cropDetails.regrowTimes)
            {
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.dayToRegrow;
                
                // Update crop
                EventHandler.CallRefreshCurrentMap();
            }
            else // Can't regrow
            {
                tileDetails.daysSinceLastMarvest = -1;
                tileDetails.seedItemID = -1;      
            }

            Destroy(gameObject);
        }
    }
}
