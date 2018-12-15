﻿using System;
using UnityEngine;
using DogHouse.Core.UI;
using DogHouse.Core.Services;
using static UnityEngine.Mathf;
using static UnityEngine.Time;

namespace DogHouse.Services
{
    /// <summary>
    /// CameraTransition is an implementation of the
    /// ICameraTransition service. The Camera Transition
    /// is responsible for creating a fade in/out effect
    /// on the current main camera. This can be used
    /// for things such as transitioning between scenes
    /// or menus.
    /// </summary>
    public class CameraTransition : BaseService<ICameraTransition>, ICameraTransition
    {
        #region Public Variables
        public CameraTransitionState State => m_state;
        #endregion

        #region Private Variables
        [SerializeField]
        private GameObject m_fadeObject = null;

        private CameraTransitionState m_state 
                = CameraTransitionState.IDLE_OUT;

        private ImageColorController m_imageColorController;
        private float m_alpha = 0f;

        private bool CanFadeIn => m_state == CameraTransitionState.IDLE_OUT;
        private bool CanFadeOut => m_state == CameraTransitionState.IDLE_IN;

        private float m_totalTime = 0f;
        private float m_transitionTime = 0f;
        private Action m_callback = null;
        private bool m_fadingIn = false;
        private Color m_imageColor = default(Color);
        #endregion

        #region Main Methods
        void Start()
        {
            m_imageColorController = m_fadeObject
                .GetComponent<ImageColorController>();
        }

        void Update()
        {
            if (m_state != CameraTransitionState.TRANSITIONING) return;
            
            TransitionCamera();
        }

        public void FadeIn(float Time)
        {
            if (!CanFadeIn)
            {
                Debug.LogError("CANNOT TRANSITION");
                return;
            }
            m_transitionTime = Time;
            m_fadingIn = true;
            m_totalTime = 0f;
            m_state = CameraTransitionState.TRANSITIONING;
        }

        public void FadeIn(float Time, Action callback)
        {
            if (!CanFadeIn)
            {
                Debug.LogError("CANNOT TRANSITION");
                return;
            }

            m_transitionTime = Time;
            m_callback = callback;
            m_fadingIn = true;
            m_totalTime = 0f;
            m_state = CameraTransitionState.TRANSITIONING;
        }

        public void FadeOut(float Time)
        {
            if (!CanFadeOut)
            {
                Debug.LogError("CANNOT TRANSITION");
                return;
            }
            m_transitionTime = Time;
            m_fadingIn = false;
            m_totalTime = 0f;
            m_state = CameraTransitionState.TRANSITIONING;
        }

        public void FadeOut(float Time, Action callback)
        {
            if (!CanFadeOut)
            {
                Debug.LogError("CANNOT TRANSITION");
                return;
            }
            m_transitionTime = Time;
            m_callback = callback;
            m_fadingIn = false;
            m_totalTime = 0f;
            m_state = CameraTransitionState.TRANSITIONING;
        }
        #endregion

        #region Utility Methods
        private void TransitionCamera()
        {
            m_totalTime += deltaTime;
            m_alpha = (m_fadingIn) ? Lerp(0, 1, m_totalTime / m_transitionTime) :
                Lerp(1, 0, m_totalTime / m_transitionTime);
            SetBackgroundAlpha();

            if(m_totalTime / m_transitionTime >= 1f)
            {
                m_state = (m_fadingIn) ? CameraTransitionState.IDLE_IN 
                                       : CameraTransitionState.IDLE_OUT;

                m_callback?.Invoke();
            }
        }

        private void SetBackgroundAlpha()
        {
            if (m_imageColorController == null) return;

            m_imageColor = m_imageColorController.ImageColor;
            m_imageColor.a = m_alpha;
            m_imageColorController.SetColor(m_imageColor);
        }
        #endregion
    }
}
