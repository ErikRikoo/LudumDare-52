﻿using System;
using Inventory;
using PlantHandling.PlantType;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class GameEvents
{
    static GameEvents()
    {
        OnPlayerGainedItem = null;
    }

    // TODO use a named tuple
    public static Action<InventoryItem, int> OnPlayerGainedItem;
    public static Action OnPlayerGainedAllItems;
    public static Action<APlantType, Vector3> OnSeedPlanted;
}