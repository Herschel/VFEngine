﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VFEngine.Platformer.Event.Raycast;
using VFEngine.Platformer.Event.Raycast.LeftRaycast;
using VFEngine.Tools;
using UniTaskExtensions = VFEngine.Tools.UniTaskExtensions;

namespace VFEngine.Platformer.Physics.Collider.RaycastHitCollider.LeftRaycastHitCollider
{
    using static RaycastHitColliderModel;
    using static UniTaskExtensions;
    using static MathsExtensions;
    using static ScriptableObjectExtensions;

    [CreateAssetMenu(fileName = "LeftRaycastHitColliderModel", menuName = PlatformerLeftRaycastHitColliderModelPath,
        order = 0)]
    [InlineEditor]
    public class LeftRaycastHitColliderModel : ScriptableObject, IModel
    {
        #region fields

        #region dependencies

        [LabelText("Left Raycast Hit Collider Data")] [SerializeField] private LeftRaycastHitColliderData l;
        [SerializeField] private GameObject character;
        [SerializeField] private PhysicsController physicsController;
        [SerializeField] private RaycastController raycastController;
        private PhysicsData physics;
        private RaycastData raycast;
        private LeftRaycastData leftRaycast;

        #endregion

        #region private methods

        private void InitializeData()
        {
            if (!l) l = CreateInstance<LeftRaycastHitColliderData>();
            if (!physicsController) physicsController = character.GetComponent<PhysicsController>();
            if (!raycastController) raycastController = character.GetComponent<RaycastController>();
            l.LeftHitsStorageLength = l.LeftHitsStorage.Length;
        }

        private void InitializeModel()
        {
            physics = physicsController.PhysicsModel.Data;
            raycast = raycastController.RaycastModel.Data;
            leftRaycast = raycastController.LeftRaycastModel.Data;
            InitializeLeftHitsStorage();
            InitializeCurrentLeftHitsStorageIndex();
            ResetState();
        }

        private void InitializeLeftHitsStorage()
        {
            l.LeftHitsStorage = new RaycastHit2D[raycast.NumberOfHorizontalRaysPerSide];
        }

        private void InitializeCurrentLeftHitsStorageIndex()
        {
            l.CurrentLeftHitsStorageIndex = 0;
        }

        private void SetCurrentLeftHitsStorage()
        {
            l.LeftHitsStorage[l.CurrentLeftHitsStorageIndex] = leftRaycast.CurrentLeftRaycastHit;
        }

        private void SetCurrentLeftHitDistance()
        {
            l.CurrentLeftHitDistance = l.LeftHitsStorage[l.CurrentLeftHitsStorageIndex].distance;
        }

        private void SetLeftRaycastHitConnected()
        {
            l.LeftHitConnected = true;
        }

        private void SetLeftRaycastHitMissed()
        {
            l.LeftHitConnected = false;
        }

        private void SetCurrentLeftHitAngle()
        {
            l.CurrentLeftHitAngle = OnSetRaycastHitAngle(l.LeftHitsStorage[l.CurrentLeftHitsStorageIndex].normal,
                physics.Transform);
        }

        private void SetCurrentLeftHitCollider()
        {
            l.CurrentLeftHitCollider = l.LeftHitsStorage[l.CurrentLeftHitsStorageIndex].collider;
        }

        private void SetCurrentLeftLateralSlopeAngle()
        {
            l.LeftLateralSlopeAngle = l.CurrentLeftHitAngle;
        }

        private void SetIsCollidingLeft()
        {
            l.IsCollidingLeft = true;
        }

        private void SetLeftDistanceToLeftCollider()
        {
            l.DistanceToLeftCollider = l.CurrentLeftHitAngle;
        }

        private void SetLeftCurrentWallCollider()
        {
            l.CurrentLeftWallCollider = l.CurrentLeftHitCollider.gameObject;
        }

        private void SetCurrentWallColliderNull()
        {
            l.CurrentLeftWallCollider = null;
        }

        private void SetLeftFailedSlopeAngle()
        {
            l.PassedLeftSlopeAngle = false;
        }

        private void SetCurrentDistanceBetweenLeftHitAndRaycastOrigin()
        {
            l.DistanceBetweenLeftHitAndRaycastOrigin = DistanceBetweenPointAndLine(
                l.LeftHitsStorage[l.CurrentLeftHitsStorageIndex].point, leftRaycast.LeftRaycastFromBottomOrigin,
                leftRaycast.LeftRaycastToTopOrigin);
        }

        private void AddToCurrentLeftHitsStorageIndex()
        {
            l.CurrentLeftHitsStorageIndex++;
        }

        private void ResetState()
        {
            l.LeftHitConnected = false;
            l.PassedLeftSlopeAngle = false;
            l.IsCollidingLeft = false;
            l.CurrentLeftHitCollider = null;
            l.CurrentLeftWallCollider = null;
            l.CurrentLeftHitAngle = 0f;
            l.LeftLateralSlopeAngle = 0f;
            l.DistanceToLeftCollider = -1f;
        }

        #endregion

        #endregion

        #region properties

        public LeftRaycastHitColliderData Data => l;

        #region public methods

        public async UniTaskVoid OnInitializeData()
        {
            InitializeData();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        public async UniTaskVoid OnInitializeModel()
        {
            InitializeModel();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        public void OnInitializeLeftHitsStorage()
        {
            InitializeLeftHitsStorage();
        }

        public void OnInitializeCurrentLeftHitsStorageIndex()
        {
            InitializeCurrentLeftHitsStorageIndex();
        }

        public void OnSetCurrentLeftHitsStorage()
        {
            SetCurrentLeftHitsStorage();
        }

        public void OnSetCurrentLeftHitAngle()
        {
            SetCurrentLeftHitAngle();
        }

        public void OnSetIsCollidingLeft()
        {
            SetIsCollidingLeft();
        }

        public void OnSetLeftDistanceToLeftCollider()
        {
            SetLeftDistanceToLeftCollider();
        }

        public void OnSetLeftCurrentWallCollider()
        {
            SetLeftCurrentWallCollider();
        }

        public void OnSetCurrentWallColliderNull()
        {
            SetCurrentWallColliderNull();
        }

        public void OnSetLeftFailedSlopeAngle()
        {
            SetLeftFailedSlopeAngle();
        }

        public void OnAddToCurrentLeftHitsStorageIndex()
        {
            AddToCurrentLeftHitsStorageIndex();
        }

        public void OnSetCurrentLeftHitDistance()
        {
            SetCurrentLeftHitDistance();
        }

        public void OnSetLeftRaycastHitConnected()
        {
            SetLeftRaycastHitConnected();
        }

        public void OnSetLeftRaycastHitMissed()
        {
            SetLeftRaycastHitMissed();
        }

        public void OnSetCurrentLeftHitCollider()
        {
            SetCurrentLeftHitCollider();
        }

        public void OnSetCurrentLeftLateralSlopeAngle()
        {
            SetCurrentLeftLateralSlopeAngle();
        }

        public void OnSetCurrentDistanceBetweenLeftHitAndRaycastOrigin()
        {
            SetCurrentDistanceBetweenLeftHitAndRaycastOrigin();
        }

        public void OnResetState()
        {
            ResetState();
        }

        #endregion

        #endregion
    }
}