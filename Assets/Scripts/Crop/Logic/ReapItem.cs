using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnFarm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        public void InitCropData(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
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
        }
    }

}
