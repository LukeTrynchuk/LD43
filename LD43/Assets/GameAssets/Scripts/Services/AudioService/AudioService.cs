﻿using System.Collections.Generic;
using DogHouse.Core.Services;
using UnityEngine;
using DogHouse.Core.Audio;
using System.Linq;
using UnityEngine.Audio;

namespace DogHouse.Services
{
    /// <summary>
    /// AudioService is an implementation of the 
    /// Audio Service interface. This concrete
    /// implementation uses several children objects
    /// each with an audio source to play multiple
    /// audio files at once.
    /// </summary>
    public class AudioService : BaseService<IAudioService>, IAudioService
    {
        #region Private Variables
        [SerializeField]
        private  int m_numberOfChannels = 0;

        [SerializeField]
        private AudioAsset[] m_audioAssets = null;

        [SerializeField]
        private AudioMixerGroup m_musicGroup = null;

        [SerializeField]
        private AudioMixerGroup m_sfxGroup = null;

        private List<AudioAsset> m_stopOnSceneLoadAssets
            = new List<AudioAsset>();

        private List<AudioSource> m_sources 
            = new List<AudioSource>();

        private ServiceReference<ISceneManager> m_sceneManager 
            = new ServiceReference<ISceneManager>();

        private GameObject m_audioClone = default(GameObject);
        #endregion

        #region Main Methods
        public override void OnEnable()
        {
            SetupClone();
            GenerateAudioChannels();
            InitializeStopOnLoadAudioAssets();
            m_sceneManager.AddRegistrationHandle(HandleSceneManagerRegistered);
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();

            if(m_sceneManager.CheckServiceRegistered())
            {
                m_sceneManager.Reference.OnAboutToLoadNewScene 
                              -= HandleAboutToLoadScene;
            }
        }

        public void Play(string AssetID)
        {
            AudioSource source = FetchAvailableAudioChannel();
            AudioAsset asset = FetchAudioAsset(AssetID);

            if (source == null || asset == null) return;
            Play(source, asset);
        }

        public AudioSource FetchAvailableAudioSource()
        {
            return FetchAvailableAudioChannel();
        }
        #endregion

        #region Utility Methods
        private void GenerateAudioChannels()
        {
            if (m_sources.Count != 0) return;
            for (int i = 0; i < m_numberOfChannels; i++)
            {
                CreateAudioChannel();
            }
        }

        private void CreateAudioChannel()
        {
            GameObject channel = Instantiate(m_audioClone, transform);
            m_sources.Add(channel.GetComponent<AudioSource>());
        }

        private AudioSource FetchAvailableAudioChannel() => 
            m_sources.FirstOrDefault(x => x.isPlaying == false);

        private AudioAsset FetchAudioAsset(string id) => 
            m_audioAssets.FirstOrDefault(x => x.AssetID.Equals(id));

        private void InitializeStopOnLoadAudioAssets()
        {
            if (m_stopOnSceneLoadAssets.Count != 0) return;

            m_stopOnSceneLoadAssets = m_audioAssets
                                .Where(x => x.StopOnSceneLoad == true)
                                .ToList();
        }

        private void Play(AudioSource source, AudioAsset asset)
        {
            source.clip = asset.Clip;
            source.priority = (int)asset.Priority;
            source.loop = asset.Loop;

            source.outputAudioMixerGroup = (asset.AudioType == AudioChannel.MUSIC)
                ? m_musicGroup
                : m_sfxGroup;

            source.Play();
        }

        private void HandleSceneManagerRegistered()
        {
            m_sceneManager.Reference.OnAboutToLoadNewScene -= HandleAboutToLoadScene;
            m_sceneManager.Reference.OnAboutToLoadNewScene += HandleAboutToLoadScene;
        }

        private void HandleAboutToLoadScene()
        {
            foreach(AudioAsset asset in m_stopOnSceneLoadAssets)
            {
                AudioSource[] sources = FetchSourcesPlayingAsset(asset);
                StopSourcesPlaying(sources);
            }
        }

        private AudioSource[] FetchSourcesPlayingAsset(AudioAsset asset)
        {
            return m_sources
                .Where(x => x.isPlaying)
                .Where(x => x.clip == asset.Clip)
                .ToArray();
        }

        private void StopSourcesPlaying(AudioSource[] sources)
        {
            foreach(AudioSource source in sources)
            {
                source.Stop();
                source.loop = false;
                source.clip = null;
            }
        }

        private void SetupClone()
        {
            m_audioClone = new GameObject();
            AudioSource source = m_audioClone.AddComponent<AudioSource>();
            source.playOnAwake = false;
            m_audioClone.transform.parent = transform;
        }
        #endregion
    }
}
