using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Router
{
    public static class SceneRouter
    {
        public enum SceneType
        {
            MainMenu = 1,
            Game = 2
        }

        private static AsyncOperation _loadSceneAsyncOperation;

        public static event Action MainMenuSceneLoadEvent;
        public static event Action GameSceneLoadEvent;
        
        [RuntimeInitializeOnLoadMethod]
        private static void OnLoad()
        {
            SwitchScene(SceneType.MainMenu);
        }

        public static void SwitchScene(SceneType type)
        {
            switch (type)
            {
                case SceneType.MainMenu:
                    LoadAdditiveScene(SceneType.MainMenu);
                    UnloadAdditiveScene(SceneType.Game);
                    break;
                case SceneType.Game:
                    LoadAdditiveScene(SceneType.Game);
                    UnloadAdditiveScene(SceneType.MainMenu);
                    break;
            }
        }
        
        private static void OnSceneLoaded(SceneType type)
        {
            switch (type)
            {
                case SceneType.MainMenu:
                    MainMenuSceneLoadEvent?.Invoke();
                    break;
                case SceneType.Game:
                    GameSceneLoadEvent?.Invoke();
                    break;
            }

            _loadSceneAsyncOperation = null;
        }
        
        private static void LoadAdditiveScene(SceneType type)
        {
            var sceneIndex = (int)type;

            if (SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded || (_loadSceneAsyncOperation != null && !_loadSceneAsyncOperation.isDone)) return;
            
            _loadSceneAsyncOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            _loadSceneAsyncOperation.completed += _ => OnSceneLoaded(type);
        }

        private static void UnloadAdditiveScene(SceneType type)
        {
            var sceneIndex = (int)type;

            if (!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded) return;
            
            SceneManager.UnloadSceneAsync(sceneIndex);
        }
    }
}