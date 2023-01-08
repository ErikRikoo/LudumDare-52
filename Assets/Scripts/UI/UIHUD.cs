﻿using System;
using System.Collections.Generic;
using PlantHandling;
using PlantHandling.PlantType;
using Player.PlayerActions.Weapons;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
	public class UIHUD : MonoBehaviour
	{
		[SerializeField] private UIHUDElements elements;
		[SerializeField] private GameState gameState;
		[SerializeField] private PlantManager plantManager;

		private readonly Dictionary<PlantType, int> _seedsCountByPlantTypes = new ();
		private int _enemiesCount = 0;

		private void Awake()
		{
			Initialize();
		}

		private void Start()
		{
			UpdateEnemiesCount();
			UpdateHealth(gameState.defaultSiloHealth);
		}

		private void OnEnable()
		{
			GameEvents.OnSeedGained += OnSeedGained;
			GameEvents.OnSeedPlanted += OnSeedPlanted;
			GameEvents.OnWeaponChanged += OnWeaponChanged;
			GameEvents.OnEnemySpawned += OnEnemySpawned;
			GameEvents.OnEnemyKilled += OnEnemyKilled;
			GameEvents.OnSiloGotHit += OnSiloGotHit;
		}
		
		private void OnDisable()
		{
			GameEvents.OnSeedGained -= OnSeedGained;
			GameEvents.OnSeedPlanted -= OnSeedPlanted;
			GameEvents.OnWeaponChanged -= OnWeaponChanged;
			GameEvents.OnEnemySpawned -= OnEnemySpawned;
			GameEvents.OnEnemyKilled -= OnEnemyKilled;
			GameEvents.OnSiloGotHit -= OnSiloGotHit;
		}

		private void Initialize()
		{
			foreach (var plantType in plantManager.plantTypes)
			{
				_seedsCountByPlantTypes.Add(plantType, 0);
			}
		}
		
		private void OnSeedGained(PlantType plantType, bool _)
		{
			_seedsCountByPlantTypes[plantType]++;

			UpdateSeedSlots();
		}
		
		private void OnSeedPlanted(PlantType plantType, Vector3 _)
		{
			_seedsCountByPlantTypes[plantType]--;

			UpdateSeedSlots();
		}
		
		private void OnWeaponChanged(AWeapon weapon)
		{
			UpdateWeaponSlot(weapon);
		}
		
		private void OnEnemySpawned()
		{
			_enemiesCount++;
			
			UpdateEnemiesCount();
		}
		
		private void OnEnemyKilled()
		{
			_enemiesCount = math.max(0, _enemiesCount - 1);
			
			UpdateEnemiesCount();
		}
		
		private void OnSiloGotHit(float value)
		{
			UpdateHealth(value);
		}
		
		private void UpdateSeedSlots()
		{
			foreach (var seedsCountByPlantType in _seedsCountByPlantTypes)
			{
				var plantType = seedsCountByPlantType.Key;
				var seedCount = seedsCountByPlantType.Value;

				elements.SeedSlotLabels[plantType].text = $"x{seedCount}";
			}
		}

		private void UpdateWeaponSlot(AWeapon weapon)
		{
			elements.WeaponSlotIcon.style.backgroundImage = new StyleBackground(weapon.Icon);
			elements.WeaponSlotLabel.text = (weapon.HasAmmo) ? $"xTODO": "";
		}

		private void UpdateEnemiesCount()
		{
			elements.EnemiesLabel.text = $"{_enemiesCount}";
		}

		private void UpdateHealth(float value)
		{
			elements.HealthLabel.text = $"{value}";
		}
	}
}