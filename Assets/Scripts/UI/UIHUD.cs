using System;
using System.Collections.Generic;
using PlantHandling;
using PlantHandling.PlantType;
using UnityEngine;

namespace UI
{
	public class UIHUD : MonoBehaviour
	{
		[SerializeField] private UIHUDElements elements;
		[SerializeField] private PlantManager plantManager;

		private readonly Dictionary<PlantType, int> _seedsCountByPlantTypes = new ();

		private void Awake()
		{
			Initialize();
		}

		private void OnEnable()
		{
			GameEvents.OnSeedGained += OnSeedGained;
			GameEvents.OnSeedPlanted += OnSeedPlanted;
		}
		
		private void OnDisable()
		{
			GameEvents.OnSeedGained -= OnSeedGained;
			GameEvents.OnSeedPlanted -= OnSeedPlanted;
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

		private void UpdateSeedSlots()
		{
			foreach (var seedsCountByPlantType in _seedsCountByPlantTypes)
			{
				var plantType = seedsCountByPlantType.Key;
				var seedCount = seedsCountByPlantType.Value;

				elements.SeedSlotLabels[plantType].text = $"x{seedCount}";
			}
		}
	}
}