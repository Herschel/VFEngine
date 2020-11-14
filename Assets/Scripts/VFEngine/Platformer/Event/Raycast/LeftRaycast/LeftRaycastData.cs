﻿using ScriptableObjects.Atoms.Raycast.References;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using VFEngine.Tools;

// ReSharper disable RedundantDefaultMemberInitializer

// ReSharper disable RedundantAssignment
namespace VFEngine.Platformer.Event.Raycast.LeftRaycast
{
    using static RaycastData;
    using static ScriptableObjectExtensions;

    public class LeftRaycastData : MonoBehaviour
    {
        #region fields

        #region dependencies

        [SerializeField] private BoolReference drawRaycastGizmosControl = new BoolReference();
        [SerializeField] private IntReference numberOfHorizontalRaysPerSide = new IntReference();
        [SerializeField] private IntReference currentLeftHitsStorageIndex = new IntReference();
        [SerializeField] private FloatReference rayOffset = new FloatReference();
        [SerializeField] private FloatReference obstacleHeightTolerance = new FloatReference();
        [SerializeField] private FloatReference boundsWidth = new FloatReference();
        [SerializeField] private Vector2Reference boundsBottomLeftCorner = new Vector2Reference();
        [SerializeField] private Vector2Reference boundsBottomRightCorner = new Vector2Reference();
        [SerializeField] private Vector2Reference boundsTopLeftCorner = new Vector2Reference();
        [SerializeField] private Vector2Reference boundsTopRightCorner = new Vector2Reference();
        [SerializeField] private Vector2Reference speed = new Vector2Reference();
        [SerializeField] private new Transform transform = null;
        [SerializeField] private LayerMask platformMask = new LayerMask();
        [SerializeField] private LayerMask oneWayPlatformMask = new LayerMask();
        [SerializeField] private LayerMask movingOneWayPlatformMask = new LayerMask();

        #endregion

        [SerializeField] private FloatReference leftRayLength = new FloatReference();
        [SerializeField] private Vector2Reference leftRaycastFromBottomOrigin = new Vector2Reference();
        [SerializeField] private Vector2Reference leftRaycastToTopOrigin = new Vector2Reference();
        [SerializeField] private RaycastReference currentLeftRaycast = new RaycastReference();
        private static readonly string LeftRaycastPath = $"{RaycastPath}RightRaycast/";
        private static readonly string ModelAssetPath = $"{LeftRaycastPath}LeftRaycastModel.asset";

        #endregion

        #region properties

        #region dependencies

        public bool DrawRaycastGizmosControl => drawRaycastGizmosControl.Value;
        public int NumberOfHorizontalRaysPerSide => numberOfHorizontalRaysPerSide.Value;
        public int CurrentLeftHitsStorageIndex => currentLeftHitsStorageIndex.Value;
        public float RayOffset => rayOffset.Value;
        public float ObstacleHeightTolerance => obstacleHeightTolerance.Value;
        public float BoundsWidth => boundsWidth.Value;
        public Vector2 BoundsBottomLeftCorner => boundsBottomLeftCorner.Value;
        public Vector2 BoundsBottomRightCorner => boundsBottomRightCorner.Value;
        public Vector2 BoundsTopLeftCorner => boundsTopLeftCorner.Value;
        public Vector2 BoundsTopRightCorner => boundsTopRightCorner.Value;
        public Vector2 Speed => speed.Value;
        public Transform Transform => transform;
        public LayerMask PlatformMask => platformMask;
        public LayerMask OneWayPlatformMask => oneWayPlatformMask;
        public LayerMask MovingOneWayPlatformMask => movingOneWayPlatformMask;

        #endregion

        public float LeftRayLength
        {
            get => leftRayLength.Value;
            set => value = leftRayLength.Value;
        }

        public Vector2 LeftRaycastFromBottomOrigin
        {
            get => leftRaycastFromBottomOrigin.Value;
            set => value = leftRaycastFromBottomOrigin.Value;
        }

        public Vector2 LeftRaycastToTopOrigin
        {
            get => leftRaycastToTopOrigin.Value;
            set => value = leftRaycastToTopOrigin.Value;
        }

        [HideInInspector] public Vector2 currentLeftRaycastOrigin;

        public ScriptableObjects.Atoms.Raycast.Raycast CurrentLeftRaycast
        {
            set => value = currentLeftRaycast.Value;
        }

        public static readonly string LeftRaycastModelPath = $"{PlatformerScriptableObjectsPath}{ModelAssetPath}";

        #endregion
    }
}