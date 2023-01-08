using System.Collections.Generic;
using PlantHandling;
using PlantHandling.PlantType;
using Player;
using Player.PlayerActions.Weapons;
using Player.PlayerActions.Weapons.Implementation.Shooting;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
	public class UIHUD : MonoBehaviour
	{
		[SerializeField] private UIHUDElements elements;
		[SerializeField] private GameState gameState;
		[SerializeField] private PlantManager plantManager;
		[SerializeField] private PlayerInventory playerInventory;
		
		private void Start()
		{
			UpdateEnemiesCount();
			UpdateHealthPoint(gameState.defaultSiloHealth);
		}

		private void OnEnable()
		{
			GameEvents.OnSeedGained += OnSeedGained;
			GameEvents.OnSeedPlanted += OnSeedPlanted;
			GameEvents.OnWeaponChanged += OnWeaponChanged;
			GameEvents.OnEnemySpawned += OnEnemySpawned;
			GameEvents.OnEnemyKilled += OnEnemyKilled;
			GameEvents.OnSiloGotHit += OnSiloGotHit;
			GameEvents.OnAmmoChanged += OnAmmoChanged;
		}
		
		private void OnDisable()
		{
			GameEvents.OnSeedGained -= OnSeedGained;
			GameEvents.OnSeedPlanted -= OnSeedPlanted;
			GameEvents.OnWeaponChanged -= OnWeaponChanged;
			GameEvents.OnEnemySpawned -= OnEnemySpawned;
			GameEvents.OnEnemyKilled -= OnEnemyKilled;
			GameEvents.OnSiloGotHit -= OnSiloGotHit;
			GameEvents.OnAmmoChanged -= OnAmmoChanged;
		}
		
		private void OnSeedGained(PlantType plantType, bool _)
		{
			UpdateSeedSlots();
		}
		
		private void OnSeedPlanted(PlantType plantType, Vector3 _)
		{
			UpdateSeedSlots();
		}
		
		private void OnWeaponChanged(AWeapon weapon)
		{
			UpdateWeaponSlot(weapon);
		}
		
		private void OnAmmoChanged(int value)
		{
			UpdateWeaponAmmoCount(value);
		}
		
		private void OnEnemySpawned()
		{
			UpdateEnemiesCount();
		}
		
		private void OnEnemyKilled()
		{
			UpdateEnemiesCount();
		}
		
		private void OnSiloGotHit(float value)
		{
			UpdateHealthPoint(value);
		}

		private void UpdateSeedSlots()
		{

			foreach (var plantType in plantManager.plantTypes)
			{
				var seedCount = playerInventory.m_Seeds.GetItem(plantType).Count;
				
				elements.SeedSlotLabels[plantType].text = $"x{seedCount}";
			}
		}

		private void UpdateWeaponSlot(AWeapon weapon)
		{
			var ammoCount = "";
			
			elements.WeaponSlotIcon.style.backgroundImage = new StyleBackground(weapon.Icon);

			if (weapon.HasAmmo)
			{
				var shootingWeapon = (AShootingWeapon)weapon;
				ammoCount = $"x{shootingWeapon.m_CurrentAmmo}";
			}
			
			elements.WeaponSlotLabel.text = ammoCount;
		}

		private void UpdateWeaponAmmoCount(int value)
		{
			elements.WeaponSlotLabel.text = $"x{value}";
		}
		

		private void UpdateEnemiesCount()
		{
			elements.EnemiesLabel.text = $"{gameState.numberOfEnemiesAlive}";
		}

		private void UpdateHealthPoint(float value)
		{
			elements.HealthLabel.text = $"{value}";
		}
	}
}