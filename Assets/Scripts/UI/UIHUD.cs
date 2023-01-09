using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
		[SerializeField] private float bannerDisplayDuration = 2;

		private void Start()
		{
			UpdateEnemiesCount();
			UpdateHealthPoint(gameState.defaultSiloHealth);
			SetSeedSlotActive(0);
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
			GameEvents.OnCurrentSeedChanged += OnCurrentSeedChanged;
			GameEvents.OnWaveStart += OnWaveStart;

			BindButtons();
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
			GameEvents.OnCurrentSeedChanged -= OnCurrentSeedChanged;
			GameEvents.OnWaveStart -= OnWaveStart;

			UnbindButtons();
		}

		private void LateUpdate()
		{
			UpdateTimerValue();
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
		
		private void OnCurrentSeedChanged(int value)
		{
			SetSeedSlotActive(value);
		}
		
		private void OnWaveStart()
		{
			StartCoroutine(StartBannerFadeInOutAnimation());
		}

		private void BindButtons()
		{
			elements.PopupCloseButton.clicked += HidePopup;
		}

		private void UnbindButtons()
		{
			elements.PopupCloseButton.clicked -= HidePopup;
		}

		public void ShowPopup(string title, Sprite image, string text, bool displayCloseButton = true, string genericButtonLabel = "", Action genericButtonAction = null)
		{
			elements.PopupTitle.style.display = DisplayStyle.Flex;
			elements.PopupImage.style.display = DisplayStyle.Flex;
			elements.PopupText.style.display = DisplayStyle.Flex;
			elements.PopupCloseButton.style.display =  displayCloseButton 
				? DisplayStyle.Flex : DisplayStyle.None;
			elements.PopupGenericButton.style.display = (genericButtonAction == null) 
				? DisplayStyle.None 
				: DisplayStyle.Flex;
			
			elements.PopupTitle.text = title;
			elements.PopupImage.style.backgroundImage = new StyleBackground(image);
			elements.PopupText.text = text;
			elements.PopupGenericButton.text = genericButtonLabel;

			if (genericButtonAction != null)
			{
				elements.PopupGenericButton.clickable = null;
				elements.PopupGenericButton.clicked += genericButtonAction;
			}

			UIAnimationUtils.FadeIn(elements.PopupContainer);

			GameEvents.OnPopupOpened?.Invoke();
		}
		
		public void ShowPopup(string title, string text, bool displayCloseButton = true, string genericButtonLabel = "", Action genericButtonAction = null)
		{
			elements.PopupTitle.style.display = DisplayStyle.Flex;
			elements.PopupImage.style.display = DisplayStyle.None;
			elements.PopupText.style.display = DisplayStyle.Flex;
			elements.PopupCloseButton.style.display =  displayCloseButton 
				? DisplayStyle.Flex : DisplayStyle.None;
			elements.PopupGenericButton.style.display = (genericButtonAction == null) 
				? DisplayStyle.None 
				: DisplayStyle.Flex;
			
			elements.PopupTitle.text = title;
			elements.PopupText.text = text;
			elements.PopupGenericButton.text = genericButtonLabel;

			if (genericButtonAction != null)
			{
				elements.PopupGenericButton.clickable = null;
				elements.PopupGenericButton.clicked += genericButtonAction;
			}

			UIAnimationUtils.FadeIn(elements.PopupContainer);

			GameEvents.OnPopupOpened?.Invoke();
		}
		
		public void ShowPopup(Sprite image, string text, bool displayCloseButton = true, string genericButtonLabel = "", Action genericButtonAction = null)
		{
			elements.PopupTitle.style.display = DisplayStyle.None;
			elements.PopupImage.style.display = DisplayStyle.Flex;
			elements.PopupText.style.display = DisplayStyle.Flex;
			elements.PopupCloseButton.style.display =  displayCloseButton 
				? DisplayStyle.Flex : DisplayStyle.None;
			elements.PopupGenericButton.style.display = (genericButtonAction == null) 
				? DisplayStyle.None 
				: DisplayStyle.Flex;
			
			elements.PopupImage.style.backgroundImage = new StyleBackground(image);
			elements.PopupText.text = text;
			elements.PopupGenericButton.text = genericButtonLabel;
			
			if (genericButtonAction != null)
			{
				elements.PopupGenericButton.clickable = null;
				elements.PopupGenericButton.clicked += genericButtonAction;
			}
			
			UIAnimationUtils.FadeIn(elements.PopupContainer);
			
			GameEvents.OnPopupOpened?.Invoke();
		}

		private IEnumerator StartBannerFadeInOutAnimation()
		{
			elements.BannerLabel.text = $"Wave {gameState.currentWave}";
			
			UIAnimationUtils.FadeIn(elements.BannerContainer);

			yield return new WaitForSeconds(bannerDisplayDuration);
			
			UIAnimationUtils.FadeOut(elements.BannerContainer);
		}

		public void HidePopup()
		{
			UIAnimationUtils.FadeOut(elements.PopupContainer);
			
			GameEvents.OnPopupClosed?.Invoke();
		}

		private void UpdateSeedSlots()
		{

			foreach (var plantType in plantManager.plants.plantTypes)
			{
				var item = playerInventory.m_Seeds.GetItem(plantType);
				if (item != null)
				{
					var seedCount = playerInventory.m_Seeds.GetItem(plantType).Count;
				
					elements.SeedSlotLabels[plantType].text = $"x{seedCount}";
				}
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

		private void UpdateTimerValue()
		{
			if (elements.TimerLabel == null)
			{
				return;
			}
			elements.TimerLabel.text = $"{gameState.timeToNextWave}";
		}

		private void SetSeedSlotActive(int seedIndex)
		{
			for (var index = 0; index < elements.SeedSlotButtons.Values.Count; index++)
			{
				var seedSlotButton = elements.SeedSlotButtons.Values.ElementAt(index);
				
				seedSlotButton.EnableInClassList("active", index == seedIndex);
			}
		}
	}
}