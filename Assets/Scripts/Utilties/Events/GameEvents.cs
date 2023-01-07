using System;
using Inventory;
using PlantHandling.PlantType;
using Player.PlayerActions.Weapons;
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
    public static Action<InventoryItem, bool> OnSeedGained;
    public static Action<int> OnCurrentSeedChanged;
    public static Action<AWeapon> OnWeaponChanged;
    public static Action OnSiloGotHit;
    public static Action OnEnemyKilled;
    public static Action OnEnemySpawned;
}