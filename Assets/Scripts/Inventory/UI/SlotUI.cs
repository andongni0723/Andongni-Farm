using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotUI : MonoBehaviour
{
    [Header("Compontent")]
    [SerializeField] private Image slotImage;
    [SerializeField] private TextMeshProUGUI amount_T;
    [SerializeField] private Image slotHighlight;
    [SerializeField] private Button button;

    [Header("Slot Type")]
    public SlotType slotType;

    public bool isSelected;
    // Item Details
    public ItemDetails itemDetails;
    public int itemAmount;

    private void Start()
    {
        isSelected = false;

        if(itemDetails.itemID == 0)
        {
            UpdateEmptySlot();
        }
    }

    /// <summary>
    /// Update slot UI and details 
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">Amount</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        itemDetails = item;
        slotImage.sprite = itemDetails.itemIcon;
        itemAmount = amount;
        amount_T.text = amount.ToString();
        button.interactable = true;
    }

    /// <summary>
    /// Updata slot to empty
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
        {
            isSelected = false;
        }

        slotImage.enabled = false;
        amount_T.text = "";
        button.interactable = false;
        itemAmount = 0;
    }
}
