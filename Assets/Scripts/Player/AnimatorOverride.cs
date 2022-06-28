using System.Collections;
using AnFarm.Inventory;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;
    public SpriteRenderer holdItem_Sprite;

    [Header("Animator List on Player Parts")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (var anim in animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition += OnHarvestAtPlayerPosition;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition -= OnHarvestAtPlayerPosition;
    }


    private void OnBeforeSceneUnloadEvent()
    {
        holdItem_Sprite.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;
        
        if(!holdItem_Sprite.enabled)
            StartCoroutine(ShowItem(itemSprite));
    }

    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem_Sprite.sprite = itemSprite;
        holdItem_Sprite.enabled = true;

        yield return new WaitForSeconds(1);
        holdItem_Sprite.enabled = false;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //WORKFLOW: Diffierent Item Return Diffierent
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            ItemType.HoeTool => PartType.Hoe,
            ItemType.WaterTool => PartType.Water,
            ItemType.CollectTool => PartType.Collect,
            ItemType.ChopTool => PartType.Chop,
            ItemType.BreakTool => PartType.Break,
            ItemType.ReapTool => PartType.Reap,
            _ => PartType.None
        };

        if (!isSelected)
        {
            currentType = PartType.None;           
            holdItem_Sprite.enabled = false;
        }
        else
        {
            if(currentType == PartType.Carry)
            {
                holdItem_Sprite.sprite = itemDetails.itemOnWorldSprite;
                holdItem_Sprite.enabled = true;
            }
            else
            {
                holdItem_Sprite.enabled = false;
            }
        }

        SwitchAnimator(currentType);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.animatorOverrideController;
            }
        }
    }
}
