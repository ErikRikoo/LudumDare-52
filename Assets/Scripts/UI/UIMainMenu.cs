using Router;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private UIMainMenuElements elements;

        private Button[] _buttons;

        private void Start()
        {
            Initialize();
        }

        private void OnEnable()
        {
            BindButtons();
        }

        private void OnDisable()
        {
            UnbindButtons();
        }

        private void BindButtons()
        {
            elements.PlayButton.clicked += OnPlayButtonPressed; 
            elements.SettingsButton.clicked += OnSettingsButtonPressed;
            elements.AboutButton.clicked += OnAboutButtonPressed; 
            elements.QuitButton.clicked += OnQuitButtonPressed;
            elements.AboutSectionBackButton.clicked += OnAboutSectionBackButtonPressed;
        }
        
        private void UnbindButtons()
        {
            elements.PlayButton.clicked -= OnPlayButtonPressed; 
            elements.SettingsButton.clicked -= OnSettingsButtonPressed;
            elements.AboutButton.clicked -= OnAboutButtonPressed; 
            elements.QuitButton.clicked -= OnQuitButtonPressed; 
            elements.AboutSectionBackButton.clicked -= OnAboutSectionBackButtonPressed; 
        }

        private void Initialize()
        {
            _buttons = new[]
            {
                elements.PlayButton,
                elements.SettingsButton,
                elements.AboutButton,
                elements.QuitButton
            };
        }
        
        private void OnPlayButtonPressed()
        {
            SceneRouter.SwitchScene(SceneRouter.SceneType.Game);
        }
        
        private void OnSettingsButtonPressed()
        {
            //TODO show settings section
        }
        
        private void OnAboutButtonPressed()
        {
            HideButtons();
            ShowAboutSection();
        }
        
        private void OnQuitButtonPressed()
        {
            Application.Quit();
        }

        private void OnAboutSectionBackButtonPressed()
        {
            ShowButtons();
            HideAboutSection();
        }

        private void ShowButtons()
        {
            foreach (var button in _buttons)
            {
                UIAnimationUtils.FadeIn(button);
            }
        }

        private void HideButtons()
        {
            foreach (var button in _buttons)
            {
                UIAnimationUtils.FadeOut(button);
            }
        }
        
        private void ShowAboutSection()
        {
            UIAnimationUtils.FadeIn(elements.AboutSectionContainer);
        }

        private void HideAboutSection()
        {
            UIAnimationUtils.FadeOut(elements.AboutSectionContainer);
        }
    }
}
