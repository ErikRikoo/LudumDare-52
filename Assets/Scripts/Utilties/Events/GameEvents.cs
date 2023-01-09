using System;
using General.TutorialData;
using Inventory;
using PlantHandling.PlantType;
using Player.PlayerActions.Weapons;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
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
    [NonSerialized] public static Action OnWaveStart;
    [NonSerialized] public static Action OnWaveEnd;
    [NonSerialized] public static Action OnGameWin;
    [NonSerialized] public static Action OnGameLose;
    [NonSerialized] public static Action OnPopupOpened;
    [NonSerialized] public static Action OnPopupClosed;
    [NonSerialized] public static Action<TutorialEvent> OnTutoAsked;
    [NonSerialized] public static Action<PlantType> OnSuccessfulPlantedSeed;
    [NonSerialized] public static Action<PlantType> OnSuccessfulHarvest;
}