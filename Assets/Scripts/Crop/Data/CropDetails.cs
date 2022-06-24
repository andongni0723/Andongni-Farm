using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int seedItemID;

    [Header("Days for different stages of growth")]
    public int[] growthDays;

    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach (var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("Item prefab of different stages")]
    public GameObject[] growthPrefabs;

    [Header("Sprites of different stages")]
    public Sprite[] growthSprites;

    [Header("Seasons can plant")]
    public Season[] seasons;


    [Space]
    [Header("Harvest tool")]
    public int[] harvestToolItemID;

    [Header("Tools require action count")]
    public int[] requireActionCount;

    [Header("Transfer item ID")]
    public int transferItemID;

    
    [Space]
    [Header("Produced item details")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;

    [Header("Regrow need days")]
    public int dayToRegrow;
    public int regrowTimes;

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;

    //TODO: VFX, SE
    public ParticaleEffectType effectType;


    /// <summary>
    /// Check the tools is available
    /// </summary>
    /// <param name="toolID">Tool ID</param>
    /// <returns>Can available?</returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if(tool == toolID) return true;
        }
        return false;
    }

    public int GetTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if(harvestToolItemID[i] == toolID) return requireActionCount[i];           
        }
        return -1;
    }
}
