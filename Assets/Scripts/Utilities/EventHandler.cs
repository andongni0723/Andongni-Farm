using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;

    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
        // The "?" is check this event whether is null
        //
        // ▼▼▼ Down code is origin ▼▼▼
        // if(UpdateInventoryUI == null)
        //     UpdateInventoryUI.Invoke(location, list);

    }
}
