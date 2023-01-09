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
    [NonSerialized] public static Action<InventoryItem, int> OnPlayerGainedItem;
    [NonSerialized] public static Action OnPlayerGainedAllItems;
    [NonSerialized] public static Action<PlantType, Vector3> OnSeedPlanted;
    [NonSerialized] public static Action<PlantType, bool> OnSeedGained;
    [NonSerialized] public static Action<int> OnCurrentSeedChanged;
    [NonSerialized] public static Action<AWeapon> OnWeaponChanged;
    [NonSerialized] public static Action OnWeaponAmmoEntirelyConsumed;
    [NonSerialized] public static Action<int> OnAmmoChanged;
    [NonSerialized] public static Action<float> OnSiloGotHit;
    [NonSerialized] public static Action OnEnemyKilled;
    [NonSerialized] public static Action OnEnemySpawned;
}