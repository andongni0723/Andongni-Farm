using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;
    public bool canHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;

    private Animator anim;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;
        // Tool use count
        int requireActionCount = cropDetails.GetTotalRequireCount(tool.itemID);
        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        // Click timer
        if (harvestActionCount < requireActionCount)
        {
            harvestActionCount++;

            // has anim? (tree)
            if (anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("rotateRight");
                else
                    anim.SetTrigger("rotateLeft");
            }

            // Play vfx
            if(cropDetails.hasParticalEffect)
                EventHandler.CallParticaleEffectEvent(cropDetails.effectType, transform.position + cropDetails.effectPos);
            // Play se
        }

        if (harvestActionCount >= requireActionCount)
        {
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)
            {
                // Instiance item
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("fallingRight");
                else
                    anim.SetTrigger("fallingLeft");

                StartCoroutine(HarvestAfterAnimator());
            }
        }
    }

    private IEnumerator HarvestAfterAnimator()
    {
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName("End"))
        {
            yield return null;
        }

        SpawnHarvestItems();

        // Transfer new items
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferChop();
        }
    }

    private void CreateTransferChop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daysSinceLastMarvest = -1;
        tileDetails.growthDays = 0;

        EventHandler.CallRefreshCurrentMap();
    }

    /// <summary>
    /// Spawn the harvest items
    /// </summary>
    public void SpawnHarvestItems()
    {
        // Spawn harvest products
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduct;
            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
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
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else // Instiance on the world
                {
                    // The items spawn direction
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;

                    // The random position in range 
                    var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                    transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 0);

                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                }
            }
        }

        if (tileDetails != null)
        {
            tileDetails.daysSinceLastMarvest++;

            // Is crop can regrow?
            if (cropDetails.dayToRegrow > 0 && tileDetails.daysSinceLastMarvest < cropDetails.regrowTimes)
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
