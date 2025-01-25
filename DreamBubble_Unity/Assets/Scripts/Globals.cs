using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public static class Globals
{
    private static List<DreamItemData> s_DreamItems = new List<DreamItemData>();
    public static List<DreamItemData> dreamItems => s_DreamItems;

    public static Action OnDreamItemsChanged;

    public static void ClearDreamItems()
    {
        s_DreamItems.Clear();
    }

    public static void AddDreamItem(DreamItemData item)
    {
        if (dreamItems.Contains(item))
           return;
        
        Debug.Log("Dream Item Added to List: " + item.name);
        s_DreamItems.Add(item);
        OnDreamItemsChanged?.Invoke();
    }
}
