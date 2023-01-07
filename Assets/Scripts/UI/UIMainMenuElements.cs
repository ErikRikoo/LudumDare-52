using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
	[RequireComponent(typeof(UIDocument))]
	public class UIMainMenuElements : MonoBehaviour
	{
		[SerializeField] private UIDocument uiDocument;

		private VisualElement _mainContainer;
		
		private Button _playButton;
		private Button _settingsButton;
		private Button _aboutButton;
		private Button _quitButton;

		public VisualElement MainContainer => _mainContainer;
		
		public Button PlayButton => _playButton;
		public Button SettingsButton => _settingsButton;
		public Button AboutButton => _aboutButton;
		public Button QuitButton => _quitButton;

		private void OnEnable()
		{
			var root = uiDocument.rootVisualElement;

			_mainContainer = root.Q<VisualElement>("Menu-Container");
			
			_playButton = root.Q<Button>("Menu-Play-Button");
			_settingsButton = root.Q<Button>("Menu-Settings-Button");
			_aboutButton = root.Q<Button>("Menu-About-Button");
			_quitButton = root.Q<Button>("Menu-Quit-Button");
		}

		private void Reset()
		{
			uiDocument = GetComponent<UIDocument>();
		}
	}
}
