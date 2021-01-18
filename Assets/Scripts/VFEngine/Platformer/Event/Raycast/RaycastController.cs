﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VFEngine.Platformer.Event.Raycast.ScriptableObjects;
using VFEngine.Platformer.Layer.Mask;
using VFEngine.Platformer.Layer.Mask.ScriptableObjects;
using VFEngine.Platformer.Physics;
using VFEngine.Platformer.Physics.ScriptableObjects;

namespace VFEngine.Platformer.Event.Raycast
{
    using static ScriptableObject;
    using static UniTask;

    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastController : SerializedMonoBehaviour
    {
        #region events

        #endregion

        #region properties

        [OdinSerialize] public RaycastData Data { get; private set; }

        #endregion

        #region fields

        [OdinSerialize] private GameObject character;
        [OdinSerialize] private BoxCollider2D boxCollider;
        [OdinSerialize] private RaycastSettings settings;
        [OdinSerialize] private PlatformerController platformerController;
        [OdinSerialize] private LayerMaskController layerMaskController;
        [OdinSerialize] private PhysicsController physicsController;
        private LayerMaskData layerMaskData;
        private PhysicsData physicsData;

        #endregion

        #region initialization

        private void Initialize()
        {
            if (!character) character = GameObject.Find("Character");
            if (!boxCollider) boxCollider = GetComponent<BoxCollider2D>();
            if (!settings) settings = CreateInstance<RaycastSettings>();
            if (!platformerController) platformerController = GetComponent<PlatformerController>();
            if (!layerMaskController) layerMaskController = GetComponent<LayerMaskController>();
            if (!physicsController) physicsController = GetComponent<PhysicsController>();
            if (!Data) Data = CreateInstance<RaycastData>();
            Data.OnInitialize(boxCollider, settings);
        }

        #endregion

        #region unity events

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
            SetDependencies();
        }

        private void SetDependencies()
        {
            layerMaskData = layerMaskController.Data;
            physicsData = physicsController.Data;
        }

        #endregion

        #region public methods

        #endregion

        #region private methods

        private async UniTask InitializeFrame()
        {
            Data.OnInitializeFrame(boxCollider);
            await Yield();
        }

        private int VerticalRays => Data.VerticalRays;
        private int DeltaMoveXDirectionAxis => physicsData.DeltaMoveXDirectionAxis;
        private LayerMask Collision => layerMaskData.Collision;
        private LayerMask OneWayPlatform => layerMaskData.OneWayPlatform;
        private RaycastHit2D Hit => Data.Hit;
        private float IgnorePlatformsTime => Data.IgnorePlatformsTime;
        private bool CastGroundCollisionRaycastForOneWayPlatform => !Hit && IgnorePlatformsTime <= 0;
        private bool RaycastHitMissed => Hit.distance <= 0;

        private async UniTask GroundCollisionRaycast()
        {
            for (var i = 0; i < VerticalRays; i++)
            {
                SetGroundCollisionRaycast(i);
                if (CastGroundCollisionRaycastForOneWayPlatform)
                {
                    SetGroundCollisionRaycastHit(OneWayPlatform);
                    if (RaycastHitMissed) continue;
                }

                if (!Hit) continue;
                OnGroundCollisionRaycastHit();
                break;
            }

            await Yield();
        }

        private void OnGroundCollisionRaycastHit()
        {
            Data.OnGroundCollisionRaycastHit();
        }

        private void SetGroundCollisionRaycast(int index)
        {
            Data.OnSetGroundCollisionRaycast(DeltaMoveXDirectionAxis, index, Collision);
        }

        private void SetGroundCollisionRaycastHit(LayerMask layer)
        {
            Data.OnSetGroundCollisionRaycastHit(layer);
        }

        private async UniTask SlopeBehavior()
        {
            Data.OnSlopeBehavior();
            await Yield();
        }

        private int HorizontalRays => Data.HorizontalRays;
        private float HitAngle => Data.HitAngle;
        private int Index => Data.Index;
        private bool OnSlope => Data.OnSlope;
        private float MinimumWallAngle => physicsData.MinimumWallAngle;
        private bool FirstHitNotOnSlope => Index == 0 && !OnSlope;
        private bool HitMissedWall => HitAngle < MinimumWallAngle;
        private bool FirstHitClimbingSlope => FirstHitNotOnSlope && HitMissedWall;
        private float MaximumSlopeAngle => physicsData.MaximumSlopeAngle;
        private bool HitMaximumSlopeAngle => !FirstHitNotOnSlope && HitAngle > MaximumSlopeAngle;
        private float GroundAngle => Data.GroundAngle;
        private bool HitSlopedGroundAngle => OnSlope && GroundAngle < MinimumWallAngle;
        private float DeltaMoveDistanceX => physicsData.DeltaMoveDistanceX;

        private async UniTask HorizontalCollision()
        {
            for (var i = 0; i < HorizontalRays; i++)
            {
                SetHorizontalCollisionRaycast(i);
                if (!Hit) continue;
                if (FirstHitClimbingSlope)
                {
                    SetCollisionOnHorizontalCollisionRaycastHitClimbingSlope();
                    await SetPhysicsOnHorizontalCollisionRaycastHitClimbingSlope();
                    SetLengthOnHitHorizontalCollisionRaycastHitClimbingSlope();
                }
                else if (HitMaximumSlopeAngle)
                {
                    if (HitMissedWall) continue;
                    await SetDeltaMoveXOnHorizontalCollisionRaycastHitMaximumSlope();
                    SetLengthOnHitHorizontalCollisionRaycastHitClimbingSlope();
                    if (HitSlopedGroundAngle) await SetPhysicsOnHorizontalCollisionRaycastHitSlopedGroundAngle();
                    SetCollisionOnHorizontalCollisionRaycastHitMaximumSlope();
                    await SetPhysicsOnHorizontalCollisionRaycastHitMaximumSlope();
                }
            }

            await Yield();
        }

        private void SetHorizontalCollisionRaycast(int index)
        {
            Data.OnSetHorizontalCollisionRaycast(index, DeltaMoveXDirectionAxis, DeltaMoveDistanceX, Collision);
        }

        private void SetCollisionOnHorizontalCollisionRaycastHitClimbingSlope()
        {
            Data.OnHorizontalCollisionRaycastHitClimbingSlope();
        }

        private async UniTask SetPhysicsOnHorizontalCollisionRaycastHitClimbingSlope()
        {
            await physicsController.OnRaycastHorizontalCollisionRaycastHitClimbingSlope();
        }

        private void SetLengthOnHitHorizontalCollisionRaycastHitClimbingSlope()
        {
            Data.OnHorizontalCollisionRaycastHitClimbingSlope(DeltaMoveDistanceX);
        }

        private async UniTask SetDeltaMoveXOnHorizontalCollisionRaycastHitMaximumSlope()
        {
            await physicsController.OnRaycastHorizontalCollisionRaycastHitMaximumSlopeSetDeltaMoveX();
        }

        private async UniTask SetPhysicsOnHorizontalCollisionRaycastHitSlopedGroundAngle()
        {
            await physicsController.OnHorizontalCollisionRaycastHitSlopedGroundAngle();
        }

        private void SetCollisionOnHorizontalCollisionRaycastHitMaximumSlope()
        {
            Data.OnHorizontalCollisionRaycastHitMaximumSlope(DeltaMoveXDirectionAxis);
        }

        private async UniTask SetPhysicsOnHorizontalCollisionRaycastHitMaximumSlope()
        {
            await physicsController.OnRaycastHorizontalCollisionRaycastHitMaximumSlope();
        }

        private async UniTask StopHorizontalSpeed()
        {
            Data.OnStopHorizontalSpeed(Collision);
            await Yield();
        }

        private bool RaycastHitMissedWhileFalling => !Hit && DeltaMoveYDirectionAxis < 0;
        private bool RaycastHitClimbingSlope => OnSlope && DeltaMoveYDirectionAxis == 1;

        private async UniTask VerticalCollision()
        {
            for (var i = 0; i < VerticalRays; i++)
            {
                SetVerticalCollisionRaycast(i);
                if (RaycastHitMissedWhileFalling) SetVerticalCollisionRaycastHit(OneWayPlatform);
                if (!Hit) continue;
                await SetDeltaMoveYOnVerticalCollisionRaycastHit();
                SetVerticalCollisionRaycastLengthOnHit();
                if (RaycastHitClimbingSlope) await SetPhysicsOnVerticalCollisionRaycastHitClimbingSlope();
                SetCollisionOnVerticalCollisionRaycastHit();
            }

            await Yield();
        }

        private int DeltaMoveYDirectionAxis => physicsData.DeltaMoveYDirectionAxis;
        private float DeltaMoveDistanceY => physicsData.DeltaMoveDistanceY;
        private Vector2 DeltaMove => physicsData.DeltaMove;

        private void SetVerticalCollisionRaycast(int index)
        {
            Data.OnSetVerticalCollisionRaycast(index, DeltaMoveYDirectionAxis, DeltaMoveDistanceY, DeltaMove.x,
                Collision);
        }

        private void SetVerticalCollisionRaycastHit(LayerMask layer)
        {
            Data.OnSetVerticalCollisionRaycastHit(DeltaMoveYDirectionAxis, layer);
        }

        private async UniTask SetDeltaMoveYOnVerticalCollisionRaycastHit()
        {
            await physicsController.OnRaycastVerticalCollisionRaycastHit();
        }

        private void SetVerticalCollisionRaycastLengthOnHit()
        {
            Data.OnSetVerticalCollisionRaycastLengthOnHit();
        }

        private async UniTask SetPhysicsOnVerticalCollisionRaycastHitClimbingSlope()
        {
            await physicsController.OnRaycastVerticalCollisionRaycastHitClimbingSlope();
        }

        private void SetCollisionOnVerticalCollisionRaycastHit()
        {
            Data.OnVerticalCollisionRaycastHit(DeltaMoveYDirectionAxis);
        }

        private async UniTask ClimbSteepSlope()
        {
            Data.OnClimbSteepSlope(DeltaMoveXDirectionAxis, DeltaMoveDistanceX, DeltaMove.y, Collision);
            await Yield();
        }

        private async UniTask OnClimbSteepSlopeHit()
        {
            Data.OnClimbSteepSlopeHit();
            await Yield();
        }

        private async UniTask ClimbMildSlope()
        {
            Data.OnClimbMildSlope(DeltaMoveXDirectionAxis, DeltaMove, Collision);
            await Yield();
        }

        private async UniTask DescendMildSlope()
        {
            Data.OnDescendMildSlope(DeltaMoveXDirectionAxis, DeltaMoveDistanceY, DeltaMove.x, Collision);
            await Yield();
        }

        private async UniTask OnDescendMildSlopeHit()
        {
            Data.OnDescendMildSlopeHit();
            await Yield();
        }

        private async UniTask DescendSteepSlope()
        {
            Data.OnDescendSteepSlope(DeltaMoveXDirectionAxis, DeltaMove, Collision);
            await Yield();
        }

        private async UniTask Move()
        {
            Data.OnMove();
            await Yield();
        }

        #endregion

        #region event handlers

        public async UniTask OnPlatformerInitializeFrame()
        {
            await InitializeFrame();
        }

        public async UniTask OnPlatformerGroundCollisionRaycast()
        {
            await GroundCollisionRaycast();
        }

        public async UniTask OnPlatformerSlopeBehavior()
        {
            await SlopeBehavior();
        }

        public async UniTask OnPlatformerHorizontalCollision()
        {
            await HorizontalCollision();
        }

        public async UniTask OnPlatformerStopHorizontalSpeed()
        {
            await StopHorizontalSpeed();
        }

        public async UniTask OnPlatformerVerticalCollision()
        {
            await VerticalCollision();
        }

        public async UniTask OnPlatformerClimbSteepSlope()
        {
            await ClimbSteepSlope();
        }

        public async UniTask OnPlatformerClimbSteepSlopeHit()
        {
            await OnClimbSteepSlopeHit();
        }

        public async UniTask OnPlatformerClimbMildSlope()
        {
            await ClimbMildSlope();
        }

        public async UniTask OnPlatformerDescendMildSlope()
        {
            await DescendMildSlope();
        }

        public async UniTask OnPlatformerDescendMildSlopeHit()
        {
            await OnDescendMildSlopeHit();
        }

        public async UniTask OnPlatformerDescendSteepSlope()
        {
            await DescendSteepSlope();
        }

        public async UniTask OnPlatformerMove()
        {
            await Move();
        }

        #endregion
    }
}