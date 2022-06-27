using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnFarm.Map;

namespace AnFarm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;

        public int seedItemID;
        public int growthDays;

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();      
        }

        private void OnEnable() {
            EventHandler.GenerateCropEvent += OnGenerateCropEvent;
        }

        private void OnDisable() {
            EventHandler.GenerateCropEvent -= OnGenerateCropEvent;
        }

        private void OnGenerateCropEvent()
        {
            GenerateCrop();
        }

        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);

            if(seedItemID != 0)
            {
                TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);
            
                if(tile == null)
                {
                    tile = new TileDetails();
                }
            
                tile.daysSinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;
                
                GridMapManager.Instance.UpdateTileDetails(tile);
            }   
        }
    }
}