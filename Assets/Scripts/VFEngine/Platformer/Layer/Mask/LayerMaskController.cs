﻿using UnityEngine;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
namespace VFEngine.Platformer.Layer.Mask
{
    using static GameObject;
    using static ScriptableObject;

    public class LayerMaskController : MonoBehaviour
    {
        #region fields

        #region dependencies

        [SerializeField] private GameObject character;
        [SerializeField] private LayerMaskSettings settings;
        private LayerMaskModel _layerMask;

        #endregion

        #region internal

        #endregion

        #region private methods

        #region initialization

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!character) character = Find("Character");
            if (!settings) settings = CreateInstance<LayerMaskSettings>();
            _layerMask = new LayerMaskModel(character, settings);
        }

        #endregion

        #endregion

        #endregion

        #region properties

        public LayerMaskData Data => _layerMask.Data;

        #region public methods

        public void OnPlatformerInitializeFrame()
        {
            _layerMask.SetSavedLayer();
            _layerMask.SetCharacterToIgnoreRaycast();
        }

        #endregion

        #endregion
    }
}