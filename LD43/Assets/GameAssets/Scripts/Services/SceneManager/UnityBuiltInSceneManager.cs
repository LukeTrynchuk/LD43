﻿using DogHouse.Core.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.SceneManagement.SceneManager;

namespace DogHouse.Services
{
    /// <summary>
    /// UnityBuiltInSceneManager will use the built
    /// in Unity Scene Manager to load new scenes.
    /// </summary>
    public class UnityBuiltInSceneManager : BaseService<ISceneManager>, ISceneManager
    {
        #region Public Variables
        public event System.Action OnAboutToLoadNewScene;
        #endregion

        #region Private Variables
        [SerializeField]
        private float m_fadeTime = 0f;

        private ServiceReference<ICameraTransition> m_cameraTransition
            = new ServiceReference<ICameraTransition>();

        private ServiceReference<IAudioMixerService> m_audioMixerService
            = new ServiceReference<IAudioMixerService>();

        private ServiceReference<IAnalyticsService> m_analytcsService
            = new ServiceReference<IAnalyticsService>();

        private ServiceReference<ILoadingScreenService> m_loadingScreenService
            = new ServiceReference<ILoadingScreenService>();

        private const string LOGO_SCENE = "LogoSlideShow";
        private const string MAIN_MENU = "MainMenu";
        private const string GAME_SCENE = "Game";
        private const string EMPTY_BUFFER = "_EmptySwitchBuffer";
        private const float FADE_TIME_SCALAR = 0.75f;

        private string m_currentScene = "";

        private float m_audioMixTime => m_fadeTime * FADE_TIME_SCALAR;
        #endregion

        #region Main Methods
        public override void OnEnable() 
        {
            base.OnEnable();
            sceneLoaded -= HandleSceneLoaded;
            sceneLoaded += HandleSceneLoaded;
        }

        public override void OnDisable() 
        {
            base.OnDisable();
            sceneLoaded -= HandleSceneLoaded;
        }

        public void LoadSlideShowScene() => Load(LOGO_SCENE);
        public void LoadMainMenuScene() => Load(MAIN_MENU);
        public void LoadGameScene() => Load(GAME_SCENE);
        #endregion

        #region Utility Methods
        private void Load(string sceneName)
        {
            m_currentScene = sceneName;
            m_audioMixerService.Reference?.TransitionToTransitionMix(m_audioMixTime);
            m_cameraTransition.Reference?.FadeIn(m_fadeTime, LoadIntoEmptyBuffer);
        }

        private void ExecuteLoad()
        {
            OnAboutToLoadNewScene?.Invoke();
            LoadSceneAsync(m_currentScene);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.name.Equals(EMPTY_BUFFER))
            {
                ExecuteLoad();
                return;
            }

            m_cameraTransition.Reference?.FadeOut(m_fadeTime);
            m_audioMixerService.Reference?.TransitionToGameMix(m_audioMixTime);
            m_analytcsService.Reference?.SendSceneLoadedEvent(scene.name);
        }

        private void LoadIntoEmptyBuffer()
        {
            LoadSceneAsync(EMPTY_BUFFER);
        }
        #endregion
    }
}
