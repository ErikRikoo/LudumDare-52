using System;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace General
{
    public class EndOfGameHandler : MonoBehaviour
    {
        [SerializeField] private UIHUD m_HUD;

        [SerializeField] private string m_LooseText = "You lose, close popup to restart.";
        [SerializeField] private string m_LooseTitle = "You lose!";
        [SerializeField] private Sprite m_LooseIcon;
        
        [SerializeField] private string m_WinText = "You win, close popup to restart.";
        [SerializeField] private string m_WinTitle = "Congrats!";
        [SerializeField] private Sprite m_WinIcon;

        
        private void OnEnable()
        {
            GameEvents.OnGameWin += OnGameWin;
            GameEvents.OnGameLose += OnGameLose;
        }
        
        private void OnDisable()
        {
            GameEvents.OnGameWin -= OnGameWin;
            GameEvents.OnGameLose -= OnGameLose;
        }

        private void OnGameLose()
        {
            DisplayPopUp(m_LooseTitle, m_LooseText, m_LooseIcon);
        }

        private void OnGameWin()
        {
            DisplayPopUp(m_WinTitle, m_WinText, m_WinIcon);
        }
        
        private void DisplayPopUp(string _title, string _text, Sprite _sprite)
        {
            m_HUD.ShowPopup(_title, _sprite, _text, true, RestartScene);
        }

        private void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}