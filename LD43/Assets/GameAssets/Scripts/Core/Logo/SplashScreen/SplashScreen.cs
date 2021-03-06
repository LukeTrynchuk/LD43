﻿using UnityEngine;

namespace DogHouse.Core.Logo
{
    /// <summary>
    /// The Splash Screen Scriptable object is
    /// used to contain information about a splash
    /// image that should be played in the editor.
    /// </summary>
    [CreateAssetMenu(fileName = "NewSplashImage", menuName = "Core/SplashImage")]
    public class SplashScreen : ScriptableObject
    {
        public Sprite Image;
        public Color BackgroundColor;

        [Range(2f, 10f)]
        public float TimeOnScreen;

        public Vector2 m_imageSize;                                             //TODO : RENAME THIS!!!
    }
}
