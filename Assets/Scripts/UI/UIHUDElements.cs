using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace UI
{
	[RequireComponent(typeof(UIDocument))]
	public class UIHUDElements : MonoBehaviour
	{
		[SerializeField] private UIDocument uiDocument;

		private const int SeedSlotsCount = 5;
		
		private VisualElement _mainContainer;
		private Button _menuButton;
		
		private VisualElement _weaponSlotIcon;
		private Label _weaponSlotLabel;
		
		private Button[] _seedSlotButtons = new Button[SeedSlotsCount];
		private VisualElement[] _seedSlotIcons = new VisualElement[SeedSlotsCount];
		private Label[] _seedSlotLabels = new Label[SeedSlotsCount];
		private VisualElement[] _seedSlotTooltips = new VisualElement[SeedSlotsCount];

		private Label _healthLabel;

		public VisualElement MainContainer => _mainContainer;
		public Button MenuButton => _menuButton;
		
		public VisualElement WeaponSlotIcon => _weaponSlotIcon;
		public Label WeaponSlotLabel => _weaponSlotLabel;
		
		public Button[] SeedSlotButtons => _seedSlotButtons;
		public VisualElement[] SeedSlotIcons => _seedSlotIcons;
		public Label[] SeedSlotLabels => _seedSlotLabels;
		public VisualElement[] SeedSlotTooltips => _seedSlotTooltips;
		
		public Label HealthLabel => _healthLabel;

		private void OnEnable()
		{
			var root = uiDocument.rootVisualElement;

			_mainContainer = root.Q<VisualElement>("HUD-Container");
			_menuButton = root.Q<Button>("HUD-Menu-Button");

			_weaponSlotIcon = root.Q<Button>("HUD-Weapon-Slot-Icon");
			_weaponSlotLabel = root.Q<Label>("HUD-Weapon-Slot-Label");
			
			for (var index = 0; index < SeedSlotsCount; index++)
			{
				_seedSlotButtons[index] = root.Q<Button>($"HUD-Seed-Slot-{index + 1}");
				_seedSlotIcons[index] = _seedSlotButtons[index].Q<VisualElement>("HUD-Seed-Slot-Icon");
				_seedSlotLabels[index] = _seedSlotButtons[index].Q<Label>("HUD-Seed-Slot-Label");
				_seedSlotTooltips[index] = root.Q<VisualElement>($"HUD-Tooltip-Seed-Slot-{index + 1}");
				
				_seedSlotButtons[index].AddManipulator(new SeedSlotButtonManipulator(_seedSlotTooltips[index]));
			}
			
			_healthLabel = root.Q<Label>("HUD-Health-Label");
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
				_tooltipElement.experimental.animation.Start(0, 1, 300, (_, opacity) =>
				{
					_tooltipElement.style.opacity = opacity;
				}).Ease(Easing.OutCubic);
			}

			private void OnPointerLeaved(PointerLeaveEvent evt)
			{
				_tooltipElement.experimental.animation.Start(1, 0, 300, (_, opacity) =>
				{
					_tooltipElement.style.opacity = opacity;
				}).Ease(Easing.OutCubic);
			}
		}
	}
}
