using UnityEngine;

namespace AnFarm.CropPlant
{
    public class CropManager : Singleton<CropManager>
    {
        public CropDataList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;
        
        private void OnEnable()
        {
            EventHandler.PlantSeedEvent += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
        }
        private void OnDisable()
        {
            EventHandler.PlantSeedEvent -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
        }

        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnPlantSeedEvent(int ID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(ID);

            if(currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1)
            {
                tileDetails.seedItemID = ID;
                tileDetails.growthDays = 0;

                // Display Crop
                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if(tileDetails.seedItemID != -1) // Refresh Map
            {
                // Display Crop
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }

        /// <summary>
        /// Display the Crop
        /// </summary>
        /// <param name="tileDetails">Tilemap details</param>
        /// <param name="cropDetails">Crop details</param>
        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            // Grown stages
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            // Cal the current growth stage in reverse order
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if(tileDetails.growthDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }

            // Get current stage Prefab
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);
            
            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
            
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
            cropInstance.GetComponent<Crop>().cropDetails = cropDetails;
            cropInstance.GetComponent<Crop>().tileDetails = tileDetails;
        }

        /// <summary>
        /// Use item ID to get the crop details
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>
        public CropDetails GetCropDetails(int ID)
        {
            return cropData.cropDetails.Find(c => c.seedItemID == ID);
        }

        /// <summary>
        /// Current season whether can be planted
        /// </summary>
        /// <param name="crop">Crop details</param>
        /// <returns></returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for (int i = 0; i < crop.seasons.Length; i++)
            {
                if(crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }
}
