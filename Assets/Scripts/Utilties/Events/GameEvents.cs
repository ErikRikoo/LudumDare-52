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
    public static Action<PlantType, Vector3> OnSeedPlanted;
    public static Action<PlantType, bool> OnSeedGained;
    public static Action<int> OnCurrentSeedChanged;
    public static Action<AWeapon> OnWeaponChanged;
    public static Action OnWeaponAmmoEntirelyConsumed;
    public static Action<int> OnAmmoChanged;
    public static Action<float> OnSiloGotHit;
    public static Action OnEnemyKilled;
    public static Action OnEnemySpawned;
    public static Action OnWaveStart;
    public static Action OnWaveEnd;
    public static Action OnGameWin;
    public static Action OnGameLose;
    public static Action OnPopupOpened;
    public static Action OnPopupClosed;
}