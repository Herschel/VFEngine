﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFEngine.Platformer.Layer.Mask;
using VFEngine.Platformer.Physics;
using VFEngine.Platformer.Physics.Collider.RaycastHitCollider.DownRaycastHitCollider;
using VFEngine.Platformer.Physics.Collider.RaycastHitCollider.UpRaycastHitCollider;
using VFEngine.Tools;
using UniTaskExtensions = VFEngine.Tools.UniTaskExtensions;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
namespace VFEngine.Platformer.Event.Raycast.UpRaycast
{
    using static Single;
    using static Raycast;
    using static DebugExtensions;
    using static Color;
    using static UniTaskExtensions;
    using static Vector2;

    public class UpRaycastController : MonoBehaviour, IController
    {
        #region fields

        #region dependencies

        private PhysicsController physicsController;
        private RaycastController raycastController;
        private UpRaycastHitColliderController upRaycastHitColliderController;
        private DownRaycastHitColliderController downRaycastHitColliderController;
        private LayerMaskController layerMaskController;
        private UpRaycastData u;
        private PhysicsData physics;
        private RaycastData raycast;
        private UpRaycastHitColliderData upRaycastHitCollider;
        private DownRaycastHitColliderData downRaycastHitCollider;
        private LayerMaskData layerMask;

        #endregion

        #region private methods

        private void Awake()
        {
            SetControllers();
            InitializeData();
        }

        private void SetControllers()
        {
            physicsController = GetComponent<PhysicsController>();
            raycastController = GetComponent<RaycastController>();
            upRaycastHitColliderController = GetComponent<UpRaycastHitColliderController>();
            downRaycastHitColliderController = GetComponent<DownRaycastHitColliderController>();
            layerMaskController = GetComponent<LayerMaskController>();
        }
        
        private void InitializeData()
        {
            u = new UpRaycastData
            {
                CurrentUpRaycastOrigin = zero,
                UpRaycastStart = zero,
                UpRaycastEnd = zero
            };
        }

        private void Start()
        {
            SetDependencies();
        }

        private void SetDependencies()
        {
            physics = physicsController.Data;
            raycast = raycastController.Data;
            upRaycastHitCollider = upRaycastHitColliderController.Data;
            downRaycastHitCollider = downRaycastHitColliderController.Data;
            layerMask = layerMaskController.Data;
        }

        private void InitializeUpRaycastLength()
        {
            u.UpRayLength = downRaycastHitCollider.GroundedEvent ? raycast.RayOffset : physics.NewPosition.y;
        }

        /*private void InitializeUpRaycastStart()
        {
            u.UpRaycastStart = SetPoint(raycast.BoundsBottomLeftCorner, raycast.BoundsTopLeftCorner, physics.Transform,
                physics.NewPosition.x);
        }

        private void InitializeUpRaycastEnd()
        {
            u.UpRaycastEnd = SetPoint(raycast.BoundsBottomRightCorner, raycast.BoundsTopRightCorner, physics.Transform,
                physics.NewPosition.y);
        }*/

        /*private static Vector2 SetPoint(Vector2 bounds1, Vector2 bounds2, Transform t, float axis)
        {
            return OnSetBounds(bounds1, bounds2) * 2 + (Vector2) t.right * axis;
        }*/

        private void InitializeUpRaycastSmallestDistance()
        {
            u.UpRaycastSmallestDistance = MaxValue;
        }

        private void SetCurrentUpRaycastOrigin()
        {
            u.CurrentUpRaycastOrigin = OnSetCurrentRaycastOrigin(u.UpRaycastStart, u.UpRaycastEnd,
                upRaycastHitCollider.CurrentUpHitsStorageIndex, raycast.NumberOfVerticalRaysPerSide);
        }

        private void SetCurrentUpRaycast()
        {
            /*u.CurrentUpRaycastHit = Raycast(u.CurrentUpRaycastOrigin, physics.Transform.up, u.UpRayLength,
                layerMask.PlatformMask & ~ layerMask.OneWayPlatformMask & ~ layerMask.MovingOneWayPlatformMask, cyan,
                raycast.DrawRaycastGizmosControl);*/
        }

        private void SetUpRaycastSmallestDistanceToRaycastUpHitAt()
        {
            u.UpRaycastSmallestDistance = upRaycastHitCollider.RaycastUpHitAt.distance;
        }

        #endregion

        #endregion

        #region properties

        public UpRaycastData Data => u;

        #region public methods

        public async UniTaskVoid OnInitializeUpRaycastLength()
        {
            InitializeUpRaycastLength();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        /*public async UniTaskVoid OnInitializeUpRaycastStart()
        {
            InitializeUpRaycastStart();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        public async UniTaskVoid OnInitializeUpRaycastEnd()
        {
            InitializeUpRaycastEnd();
            await SetYieldOrSwitchToThreadPoolAsync();
        }*/

        public async UniTaskVoid OnInitializeUpRaycastSmallestDistance()
        {
            InitializeUpRaycastSmallestDistance();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        public void OnSetCurrentUpRaycastOrigin()
        {
            SetCurrentUpRaycastOrigin();
        }

        public void OnSetCurrentUpRaycast()
        {
            SetCurrentUpRaycast();
        }

        public void OnSetUpRaycastSmallestDistanceToRaycastUpHitAt()
        {
            SetUpRaycastSmallestDistanceToRaycastUpHitAt();
        }

        #endregion

        #endregion
    }
}