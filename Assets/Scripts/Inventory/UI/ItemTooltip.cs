using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemTooltip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name_T;
    [SerializeField] private TextMeshProUGUI type_T;
    [SerializeField] private TextMeshProUGUI description_T;
    [SerializeField] private Text value_T;
    [SerializeField] private GameObject bottomPart;

    public void SetupToolTip(ItemDetails itemDetails, SlotType slotType)
    {
        name_T.text = itemDetails.itemName;

        type_T.text = GetItemType(itemDetails.itemType);

        description_T.text = itemDetails.itemDescription;

        if (itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            bottomPart.SetActive(true);

            var price = itemDetails.itemPrice;

            if (slotType == SlotType.Bag)
            {
                price = (int)(price * itemDetails.sellPercentage);
            }

            value_T.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => "種子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.BreakTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.CollectTool => "工具",
            ItemType.HoeTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            _ => "無"
        };
    }
}
