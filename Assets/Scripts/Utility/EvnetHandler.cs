using System;
using System.Collections.Generic;

public static class EvnetHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> inventoryItems)
    {
        UpdateInventoryUI?.Invoke(location,inventoryItems);
    }
}
