using System.Collections.Generic;
using PlantHandling;
using PlantHandling.PlantType;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UI
{
	[RequireComponent(typeof(UIDocument))]
	public class UIHUDElements : MonoBehaviour
	{
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private PlantManager plantManager;
		
		private VisualElement _mainContainer;
		
		private TemplateContainer _seedSlotTemplate;
		private TemplateContainer _tooltipTemplate;
		
		private Button _menuButton;
		
		private VisualElement _inventory;
		private VisualElement _tooltips;
		
		private VisualElement _weaponSlotIcon;
		private Label _weaponSlotLabel;
		
		private Dictionary<PlantType, Button> _seedSlotButtons = new();
		private Dictionary<PlantType, VisualElement> _seedSlotIcons = new();
		private Dictionary<PlantType, Label> _seedSlotLabels = new();
		private Dictionary<PlantType, VisualElement> _seedSlotTooltips = new();

		private VisualElement _popupContainer;
		private Label _popupTitle;
		private VisualElement _popupImage;
		private Label _popupText;
		private Button _popupCloseButton;
		private Button _popupGenericButton;
		
		private Label _healthLabel;
		private Label _enemiesLabel;
		private Label _timerLabel;

		public VisualElement MainContainer => _mainContainer;
		
		public Button MenuButton => _menuButton;
		
		public VisualElement WeaponSlotIcon => _weaponSlotIcon;
		public Label WeaponSlotLabel => _weaponSlotLabel;
		
		public Dictionary<PlantType, Button> SeedSlotButtons => _seedSlotButtons;
		public Dictionary<PlantType, VisualElement> SeedSlotIcons => _seedSlotIcons;
		public Dictionary<PlantType, Label> SeedSlotLabels => _seedSlotLabels;
		public Dictionary<PlantType, VisualElement> SeedSlotTooltips => _seedSlotTooltips;

		public VisualElement PopupContainer => _popupContainer;
		public Label PopupTitle => _popupTitle;
		public VisualElement PopupImage => _popupImage;
		public Label PopupText => _popupText;
		public Button PopupCloseButton => _popupCloseButton;
		public Button PopupGenericButton => _popupGenericButton;

		public Label HealthLabel => _healthLabel;
		public Label EnemiesLabel => _enemiesLabel;
		public Label TimerLabel => _timerLabel;

		private void Awake()
		{
			FindReferences();
		}

		private void FindReferences()
		{
			var root = uiDocument.rootVisualElement;

			_mainContainer = root.Q<VisualElement>("HUD-Container");
			
			_seedSlotTemplate = root.Q<TemplateContainer>("HUD-Seed-Slot-Template");
			_tooltipTemplate = root.Q<TemplateContainer>("HUD-Tooltip-Template");
			
			_menuButton = root.Q<Button>("HUD-Menu-Button");
			
			_inventory = root.Q<VisualElement>("HUD-Inventory");
			_tooltips = root.Q<VisualElement>("HUD-Tooltips");

			_weaponSlotIcon = root.Q<VisualElement>("HUD-Weapon-Slot-Icon");
			_weaponSlotLabel = root.Q<Label>("HUD-Weapon-Slot-Label");
			
			InstantiateSeedSlots();
			
			_popupContainer = root.Q<VisualElement>("HUD-Popup-Container");
			_popupTitle = root.Q<Label>("HUD-Popup-Title");
			_popupImage = root.Q<VisualElement>("HUD-Popup-Image");
			_popupText = root.Q<Label>("HUD-Popup-Text");
			_popupCloseButton = root.Q<Button>("HUD-Popup-Close-Button");
			_popupGenericButton = root.Q<Button>("HUD-Popup-Generic-Button");
			
			_healthLabel = root.Q<Label>("HUD-Health-Label");
			_enemiesLabel = root.Q<Label>("HUD-Enemies-Label");
			_timerLabel = root.Q<Label>("HUD-Timer-Label");
		}

		private void InstantiateSeedSlots()
		{
			var index = 0;
			foreach (var plantType in plantManager.plants.plantTypes)
			{
				var seedSlotInstance = _seedSlotTemplate.templateSource.Instantiate();
				var tooltipInstance = _tooltipTemplate.templateSource.Instantiate();
					
				var seedButton = seedSlotInstance.Q<Button>($"HUD-Seed-Slot");
				var seedIcon = seedButton.Q<VisualElement>("HUD-Seed-Slot-Icon");
				var seedLabel = seedButton.Q<Label>("HUD-Seed-Slot-Label");
				var tooltip = tooltipInstance.Q<VisualElement>($"HUD-Tooltip");
				var tooltipLabel = tooltip.Q<Label>($"HUD-Tooltip-Label");
				
				seedButton.AddManipulator(new SeedSlotButtonManipulator(tooltip));

				seedIcon.style.backgroundImage = new StyleBackground(plantType.Icon);
				seedLabel.text = "x0";
				tooltipLabel.text = plantType.ToolTip;
				
				_inventory.Add(seedButton);
				_tooltips.Add(tooltip);

                var b_index = index;
                seedButton.clicked += () =>
				{
                    GameEvents.OnCurrentSeedChanged.Invoke(b_index);
                };

                _seedSlotButtons.Add(plantType, seedButton);
				_seedSlotIcons.Add(plantType, seedIcon);
				_seedSlotLabels.Add(plantType, seedLabel);
				_seedSlotTooltips.Add(plantType, tooltip);
				index++;
            }
		}
		
		private void Reset()
		{
			uiDocument = GetComponent<UIDocument>();
		}
		
		private class SeedSlotButtonManipulator : Manipulator
		{
			private VisualElement _tooltipElement;
			
			public SeedSlotButtonManipulator(VisualElement tooltipElement)
			{
				_tooltipElement = tooltipElement;
			}

			protected override void RegisterCallbacksOnTarget()
			{
				target.RegisterCallback<PointerEnterEvent>(OnPointerEntered);
				target.RegisterCallback<PointerLeaveEvent>(OnPointerLeaved);
			}
			
			protected override void UnregisterCallbacksFromTarget()
			{
				target.UnregisterCallback<PointerEnterEvent>(OnPointerEntered);
				target.UnregisterCallback<PointerLeaveEvent>(OnPointerLeaved);
			}
			
			private void OnPointerEntered(PointerEnterEvent evt)
			{
				UIAnimationUtils.FadeIn(_tooltipElement, false); 
			}

			private void OnPointerLeaved(PointerLeaveEvent evt)
			{
				UIAnimationUtils.FadeOut(_tooltipElement, false); 
			}
		}
	}
}
