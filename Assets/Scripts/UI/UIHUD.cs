using System;
using Inventory;
using UnityEngine;

namespace UI
{
	public class UIHUD : MonoBehaviour
	{
		[SerializeField] private UIHUDElements hudElements;

		private void OnEnable()
		{
			GameEvents.OnPlayerGainedItem += OnPlayerGainedItem;
		}
		
		private void OnDisable()
		{
			GameEvents.OnPlayerGainedItem -= OnPlayerGainedItem;
		}

		private void OnPlayerGainedItem(InventoryItem inventoryItem, int count)
		{
			throw new NotImplementedException();
		}
	}
}