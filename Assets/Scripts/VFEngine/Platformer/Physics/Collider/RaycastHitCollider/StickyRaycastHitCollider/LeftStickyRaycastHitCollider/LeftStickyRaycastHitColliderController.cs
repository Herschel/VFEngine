﻿using UnityEngine;
using VFEngine.Platformer.Event.Raycast.StickyRaycast.LeftStickyRaycast;
using VFEngine.Tools;

// ReSharper disable ConvertToAutoPropertyWithPrivateSetter
namespace VFEngine.Platformer.Physics.Collider.RaycastHitCollider.StickyRaycastHitCollider.LeftStickyRaycastHitCollider
{
    using static Vector3;

    public class LeftStickyRaycastHitColliderController : MonoBehaviour, IController
    {
        #region fields

        #region dependencies

        [SerializeField] private GameObject character;
        private PhysicsController physicsController;
        private LeftStickyRaycastController leftStickyRaycastController;
        private LeftStickyRaycastHitColliderData l;
        private PhysicsData physics;
        private LeftStickyRaycastData leftStickyRaycast;

        #endregion

        #region private methods

        private void Awake()
        {
            LoadCharacter();
            InitializeData();
            SetControllers();
        }

        private void LoadCharacter()
        {
            if (!character) character = transform.root.gameObject;
        }

        private void InitializeData()
        {
            l = new LeftStickyRaycastHitColliderData();
        }

        private void SetControllers()
        {
            physicsController = character.GetComponentNoAllocation<PhysicsController>();
            leftStickyRaycastController = character.GetComponentNoAllocation<LeftStickyRaycastController>();
        }

        private void Start()
        {
            SetDependencies();
            InitializeFrame();
        }

        private void SetDependencies()
        {
            physics = physicsController.Data;
            leftStickyRaycast = leftStickyRaycastController.Data;
        }

        private void InitializeFrame()
        {
            ResetState();
        }

        private void ResetState()
        {
            l.BelowSlopeAngleLeft = 0f;
            l.CrossBelowSlopeAngleLeft = zero;
        }

        private void SetBelowSlopeAngleLeft()
        {
            l.BelowSlopeAngleLeft = Vector2.Angle(leftStickyRaycast.LeftStickyRaycastHit.normal, physics.Transform.up);
        }

        private void SetCrossBelowSlopeAngleLeft()
        {
            l.CrossBelowSlopeAngleLeft = Cross(physics.Transform.up, leftStickyRaycast.LeftStickyRaycastHit.normal);
        }

        private void SetBelowSlopeAngleLeftToNegative()
        {
            l.BelowSlopeAngleLeft = -l.BelowSlopeAngleLeft;
        }

        #endregion

        #endregion

        #region properties

        public LeftStickyRaycastHitColliderData Data => l;

        #region public methods

        public void OnSetBelowSlopeAngleLeft()
        {
            SetBelowSlopeAngleLeft();
        }

        public void OnSetCrossBelowSlopeAngleLeft()
        {
            SetCrossBelowSlopeAngleLeft();
        }

        public void OnSetBelowSlopeAngleLeftToNegative()
        {
            SetBelowSlopeAngleLeftToNegative();
        }

        public void OnResetState()
        {
            ResetState();
        }

        #endregion

        #endregion
    }
}