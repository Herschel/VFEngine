﻿using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VFEngine.Tools;

namespace VFEngine.Platformer
{
    using static ScriptableObjectExtensions;

    [CreateAssetMenu(fileName = "PlatformerSettings", menuName = PlatformerSettingsPath, order = 0)]
    [InlineEditor]
    public class PlatformerSettings : ScriptableObject
    {
        #region properties

        [SerializeField] public bool displayWarnings = true;
        [SerializeField] public float oneWayPlatformDelay = 0.1f;
        [SerializeField] public float ladderClimbThreshold = 0.3f;
        [SerializeField] public float ladderDelay = 0.3f;

        #endregion
    }
}