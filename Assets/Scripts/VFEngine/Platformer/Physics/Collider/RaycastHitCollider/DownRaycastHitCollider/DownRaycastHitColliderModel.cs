﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VFEngine.Platformer.Physics.Movement.PathMovement;
using VFEngine.Platformer.Physics.PhysicsMaterial;
using VFEngine.Tools;
using UniTaskExtensions = VFEngine.Tools.UniTaskExtensions;

namespace VFEngine.Platformer.Physics.Collider.RaycastHitCollider.DownRaycastHitCollider
{
    using static MathsExtensions;
    using static LayerMask;
    using static Vector3;
    using static UniTaskExtensions;

    [CreateAssetMenu(fileName = "DownRaycastHitColliderModel",
        menuName =
            "VFEngine/Platformer/Physics/Raycast Hit Collider/Down Raycast Hit Collider/Down Raycast Hit Collider Model",
        order = 0)]
    public class DownRaycastHitColliderModel : ScriptableObject, IModel
    {
        #region fields

        #region dependencies

        [LabelText("Down Raycast Hit Collider Data")] [SerializeField]
        private DownRaycastHitColliderData d;

        #endregion

        #region private methods

        private void Initialize()
        {
            InitializeModel();
        }

        private void InitializeModel()
        {
            d.physicsMaterialClosestToDownHit = d.StandingOnWithSmallestDistance.gameObject
                .GetComponentNoAllocation<PhysicsMaterialData>();
            d.HasPhysicsMaterialClosestToDownHit = d.physicsMaterialClosestToDownHit != null;
            d.pathMovementClosestToDownHit = d.StandingOnWithSmallestDistance.gameObject
                .GetComponentNoAllocation<PathMovementData>();
            d.HasPathMovementClosestToDownHit = d.pathMovementClosestToDownHit != null;
            d.HasStandingOnLastFrame = d.StandingOnLastFrame != null;
            d.HasMovingPlatform = d.MovingPlatform != null;
            d.DownHitsStorageLength = d.DownHitsStorage.Length;
        }

        private void SetCurrentDownHitsStorage()
        {
            d.DownHitsStorage[d.CurrentDownHitsStorageIndex] = d.CurrentDownRaycast;
        }

        private void InitializeFriction()
        {
            d.friction = 0;
        }

        private void InitializeDownHitsStorage()
        {
            d.DownHitsStorage = new RaycastHit2D[d.NumberOfVerticalRaysPerSide];
        }

        private void InitializeDownHitsStorageSmallestDistanceIndex()
        {
            d.DownHitsStorageSmallestDistanceIndex = 0;
        }

        private void InitializeDownHitConnected()
        {
            d.DownHitConnected = false;
        }

        private void InitializeDownHitsStorageIndex()
        {
            d.CurrentDownHitsStorageIndex = 0;
        }

        private void AddDownHitsStorageIndex()
        {
            d.CurrentDownHitsStorageIndex++;
        }

        private void SetRaycastDownHitAt()
        {
            d.RaycastDownHitAt = d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex];
        }

        private void SetDownHitConnected()
        {
            d.DownHitConnected = true;
        }

        private void SetBelowSlopeAngleAt()
        {
            d.belowSlopeAngle = Vector2.Angle(d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex].normal,
                d.Transform.up);
        }

        private void SetCrossBelowSlopeAngleAt()
        {
            d.CrossBelowSlopeAngle = Cross(d.Transform.up,
                d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex].normal);
        }

        private void SetSmallestDistanceIndexAt()
        {
            d.DownHitsStorageSmallestDistanceIndex = d.CurrentDownHitsStorageIndex;
        }

        private void SetNegativeBelowSlopeAngle()
        {
            d.CrossBelowSlopeAngle = -d.CrossBelowSlopeAngle;
        }

        private void SetDownHitWithSmallestDistance()
        {
            d.DownHitWithSmallestDistance = d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex];
        }

        private void SetFrictionToDownHitWithSmallestDistancesFriction()
        {
            if (d.physicsMaterialClosestToDownHit is null) return;
            d.friction = d.physicsMaterialClosestToDownHit.Friction;
        }

        private void SetIsCollidingBelow()
        {
            d.IsCollidingBelow = true;
        }

        private void SetIsNotCollidingBelow()
        {
            d.IsCollidingBelow = false;
        }

        private void SetMovingPlatformToDownHitWithSmallestDistancesPathMovement()
        {
            if (d.pathMovementClosestToDownHit is null) return;
            d.MovingPlatform = d.pathMovementClosestToDownHit;
        }

        private void SetMovingPlatformToNull()
        {
            d.MovingPlatform = null;
        }

        private void StopMovingPlatformCurrentGravity()
        {
            d.movingPlatformCurrentGravity = 0;
        }

        private void SetCurrentDownHitSmallestDistance()
        {
            d.CurrentDownHitSmallestDistance = DistanceBetweenPointAndLine(
                d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex].point, d.DownRaycastFromLeft,
                d.DownRaycastToRight);
        }

        private void SetGroundedEvent()
        {
            d.groundedEvent = true;
        }

        private void SetStandingOnLastFrameLayerToPlatform()
        {
            d.StandingOnLastFrame.layer = NameToLayer("Platform");
        }

        private void SetStandingOnLastFrameLayerToSavedBelowLayer()
        {
            d.StandingOnLastFrame.layer = d.SavedBelowLayer;
        }

        private void SetStandingOn()
        {
            d.standingOn = d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex].collider.gameObject;
        }

        private void SetStandingOnCollider()
        {
            d.StandingOnCollider = d.DownHitsStorage[d.DownHitsStorageSmallestDistanceIndex].collider;
        }

        private void ResetState()
        {
            // foobar
        }

        private void SetWasGroundedLastFrame()
        {
            d.WasGroundedLastFrame = d.IsCollidingBelow;
        }

        private void SetStandingOnLastFrame()
        {
            d.StandingOnLastFrame = d.standingOn;
        }

        private void SetOnMovingPlatform()
        {
            d.OnMovingPlatform = true;
        }

        private void SetNotOnMovingPlatform()
        {
            d.OnMovingPlatform = false;
        }

        private void SetMovingPlatformCurrentGravity()
        {
            d.movingPlatformCurrentGravity = d.movingPlatformGravity;
        }

        #endregion

        #endregion

        #region properties

        #region public methods

        public void OnSetWasGroundedLastFrame()
        {
            SetWasGroundedLastFrame();
        }

        public void OnSetStandingOnLastFrame()
        {
            SetStandingOnLastFrame();
        }

        public void OnSetCurrentDownHitsStorage()
        {
            SetCurrentDownHitsStorage();
        }

        public void OnInitializeFriction()
        {
            InitializeFriction();
        }

        public void OnInitializeDownHitsStorage()
        {
            InitializeDownHitsStorage();
        }

        public void OnInitializeDownHitsStorageSmallestDistanceIndex()
        {
            InitializeDownHitsStorageSmallestDistanceIndex();
        }

        public void OnInitializeDownHitConnected()
        {
            InitializeDownHitConnected();
        }

        public void OnInitializeDownHitsStorageIndex()
        {
            InitializeDownHitsStorageIndex();
        }

        public void OnAddDownHitsStorageIndex()
        {
            AddDownHitsStorageIndex();
        }

        public void OnSetRaycastDownHitAt()
        {
            SetRaycastDownHitAt();
        }

        public void OnSetDownHitConnected()
        {
            SetDownHitConnected();
        }

        public void OnSetBelowSlopeAngleAt()
        {
            SetBelowSlopeAngleAt();
        }

        public void OnSetCrossBelowSlopeAngleAt()
        {
            SetCrossBelowSlopeAngleAt();
        }

        public void OnSetSmallestDistanceIndexAt()
        {
            SetSmallestDistanceIndexAt();
        }

        public void OnSetNegativeBelowSlopeAngle()
        {
            SetNegativeBelowSlopeAngle();
        }

        public void OnSetDownHitWithSmallestDistance()
        {
            SetDownHitWithSmallestDistance();
        }

        public void OnSetIsCollidingBelow()
        {
            SetIsCollidingBelow();
        }

        public void OnSetIsNotCollidingBelow()
        {
            SetIsNotCollidingBelow();
        }

        public void OnSetFrictionToDownHitWithSmallestDistancesFriction()
        {
            SetFrictionToDownHitWithSmallestDistancesFriction();
        }

        public void OnSetMovingPlatformToDownHitWithSmallestDistancesPathMovement()
        {
            SetMovingPlatformToDownHitWithSmallestDistancesPathMovement();
        }

        public void OnSetMovingPlatformToNull()
        {
            SetMovingPlatformToNull();
        }

        public void OnStopMovingPlatformCurrentGravity()
        {
            StopMovingPlatformCurrentGravity();
        }

        public void OnSetCurrentDownHitSmallestDistance()
        {
            SetCurrentDownHitSmallestDistance();
        }

        public void OnSetGroundedEvent()
        {
            SetGroundedEvent();
        }

        public void OnSetStandingOnLastFrameLayerToPlatform()
        {
            SetStandingOnLastFrameLayerToPlatform();
        }

        public void OnSetStandingOnLastFrameLayerToSavedBelowLayer()
        {
            SetStandingOnLastFrameLayerToSavedBelowLayer();
        }

        public void OnSetStandingOn()
        {
            SetStandingOn();
        }

        public void OnSetStandingOnCollider()
        {
            SetStandingOnCollider();
        }

        public void OnSetNotOnMovingPlatform()
        {
            SetNotOnMovingPlatform();
        }

        public void OnResetState()
        {
            ResetState();
        }

        public async UniTaskVoid OnInitialize()
        {
            Initialize();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        #endregion

        #endregion
    }
}