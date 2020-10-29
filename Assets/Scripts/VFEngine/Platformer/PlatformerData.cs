﻿using ScriptableObjects.Atoms.LayerMask.References;
using ScriptableObjects.Atoms.RaycastHit2D.References;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Serialization;
using VFEngine.Platformer.Event.Boxcast.SafetyBoxcast;
using VFEngine.Platformer.Event.Raycast;
using VFEngine.Platformer.Event.Raycast.StickyRaycast;
using VFEngine.Platformer.Layer.Mask;
using VFEngine.Platformer.Physics;
using VFEngine.Platformer.Physics.Collider.RaycastHitCollider;
using VFEngine.Tools;

namespace VFEngine.Platformer
{
    using static ScriptableObjectExtensions;

    public class PlatformerData : MonoBehaviour
    {
        /* fields: dependencies */
        [SerializeField] private PlatformerSettings settings;
        [SerializeField] private PhysicsController physics;
        [SerializeField] private RaycastController raycast;
        [SerializeField] private RaycastHitColliderController raycastHitCollider;
        [SerializeField] private StickyRaycastController stickyRaycast;
        [SerializeField] private LayerMaskController layerMask;
        [SerializeField] private SafetyBoxcastController safetyBoxcast;
        [SerializeField] private Vector2Reference speed;
        [SerializeField] private BoolReference gravityActive;
        [SerializeField] private FloatReference fallSlowFactor;
        [SerializeField] private BoolReference isCollidingWithMovingPlatform;
        [SerializeField] private Vector2Reference movingPlatformCurrentSpeed;
        [SerializeField] private BoolReference wasTouchingCeilingLastFrame;
        [SerializeField] private FloatReference movementDirectionThreshold;
        [SerializeField] private Vector2Reference externalForce;
        [SerializeField] private BoolReference castRaysOnBothSides;
        [SerializeField] private IntReference horizontalMovementDirection;
        [SerializeField] private Vector2Reference horizontalRaycastFromBottom;
        [SerializeField] private Vector2Reference horizontalRaycastToTop;
        [SerializeField] private IntReference numberOfHorizontalRays;
        [SerializeField] private Vector2Reference currentRightRaycastOrigin;
        [SerializeField] private Vector2Reference currentLeftRaycastOrigin;
        [SerializeField] private BoolReference wasGroundedLastFrame;
        [SerializeField] private IntReference rightHitsStorageIndex;
        [SerializeField] private IntReference leftHitsStorageIndex;
        [SerializeField] private FloatReference horizontalRayLength;
        [SerializeField] private IntReference horizontalHitsStorageLength;
        [SerializeField] private IntReference numberOfHorizontalRaysPerSide;
        [SerializeField] private FloatReference currentRightHitDistance;
        [SerializeField] private FloatReference currentLeftHitDistance;
        [SerializeField] private FloatReference currentDownHitSmallestDistance;
        [SerializeField] private Collider2DReference currentRightHitCollider;
        [SerializeField] private Collider2DReference currentLeftHitCollider;
        [SerializeField] private Collider2DReference ignoredCollider;
        [SerializeField] private FloatReference currentRightHitAngle;
        [SerializeField] private FloatReference currentLeftHitAngle;
        [SerializeField] private FloatReference maximumSlopeAngle;
        [SerializeField] private BoolReference isGrounded;
        [SerializeField] private Vector2Reference newPosition;
        [SerializeField] private FloatReference smallValue;
        [SerializeField] private FloatReference gravity;
        [SerializeField] private BoolReference isFalling;
        [SerializeField] private FloatReference downRayLength;
        [SerializeField] private BoolReference onMovingPlatform;
        [SerializeField] private IntReference downHitsStorageLength;
        [SerializeField] private GameObjectReference standingOnLastFrame;
        [SerializeField] private BoolReference isStandingOnLastFrameNotNull;
        [SerializeField] private LayerMaskReference midHeightOneWayPlatformMask;
        [SerializeField] private LayerMaskReference stairsMask;
        [SerializeField] private Collider2DReference standingOnCollider;
        [SerializeField] private Vector2Reference colliderBottomCenterPosition;
        [SerializeField] private FloatReference smallestDistance;
        [SerializeField] private IntReference downHitsStorageSmallestDistanceIndex;
        [SerializeField] private BoolReference downHitConnected;
        [SerializeField] private RaycastHit2DReference raycastDownHitAt;
        [SerializeField] private Vector3Reference crossBelowSlopeAngle;
        [SerializeField] private GameObjectReference standingOnWithSmallestDistance;
        [SerializeField] private Collider2DReference standingOnWithSmallestDistanceCollider;
        [SerializeField] private LayerMaskReference standingOnWithSmallestDistanceLayer;
        [SerializeField] private FloatReference boundsHeight;
        [SerializeField] private LayerMaskReference oneWayPlatformMask;
        [SerializeField] private LayerMaskReference movingOneWayPlatformMask;
        [SerializeField] private BoolReference hasPhysicsMaterialClosestToDownHit;
        [SerializeField] private BoolReference hasPathMovementClosestToDownHit;
        [SerializeField] private BoolReference hasMovingPlatform;
        [SerializeField] private BoolReference stickToSlopesControl;
        [SerializeField] private FloatReference stickToSlopesOffsetY;
        [SerializeField] private BoolReference isJumping;
        [SerializeField] private FloatReference stickyRaycastLength;
        [SerializeField] private FloatReference rightStickyRaycastLength;
        [SerializeField] private FloatReference leftStickyRaycastLength;
        [SerializeField] private Vector3Reference crossBelowSlopeAngleLeft;
        [SerializeField] private Vector3Reference crossBelowSlopeAngleRight;
        [SerializeField] private FloatReference belowSlopeAngleLeft;
        [SerializeField] private FloatReference belowSlopeAngleRight;
        [SerializeField] private BoolReference castFromLeft;
        [SerializeField] private BoolReference safetyBoxcastControl;
        [SerializeField] private BoolReference hasSafetyBoxcast;
        [SerializeField] private Collider2DReference safetyBoxcastCollider;
        [SerializeField] private RaycastHit2DReference leftStickyRaycast;
        [SerializeField] private RaycastHit2DReference rightStickyRaycast;
        [SerializeField] private IntReference upHitsStorageLength;
        [SerializeField] private RaycastHit2DReference raycastUpHitAt;
        [SerializeField] private FloatReference upRaycastSmallestDistance;
        [SerializeField] private BoolReference upHitConnected;
        [SerializeField] private IntReference rightHitsStorageLength;
        [SerializeField] private IntReference leftHitsStorageLength;
        [SerializeField] private FloatReference safetyBoxcastDistance;
        [SerializeField] private BoolReference isCollidingBelow;
        [SerializeField] private BoolReference isCollidingLeft;
        [SerializeField] private BoolReference isCollidingRight;
        [SerializeField] private BoolReference isCollidingAbove;
        [SerializeField] private FloatReference distanceToGroundRayMaximumLength;
        [SerializeField] private BoolReference distanceToGroundRaycastNotNull;
        [SerializeField] private RaycastHit2DReference distanceToGroundRaycast;

        /* fields */
        private const string ModelAssetPath = "DefaultPlatformerModel.asset";

        /* properties: dependencies */
        public float DistanceToGroundRayMaximumLength => distanceToGroundRayMaximumLength.Value;
        public bool IsCollidingAbove => isCollidingAbove.Value;
        public bool IsCollidingBelow => isCollidingBelow.Value;
        public bool IsCollidingLeft => isCollidingLeft.Value;
        public bool IsCollidingRight => isCollidingRight.Value;
        public float SafetyBoxcastDistance => safetyBoxcastDistance.Value;
        public int LeftHitsStorageLength => leftHitsStorageLength.Value;
        public int RightHitsStorageLength => rightHitsStorageLength.Value;
        public bool DisplayWarnings => settings.displayWarningsControl;
        public bool HasSettings => settings;
        public PhysicsController Physics => physics;
        public RaycastController Raycast => raycast;
        public RaycastHitColliderController RaycastHitCollider => raycastHitCollider;
        public LayerMaskController LayerMask => layerMask;
        public StickyRaycastController StickyRaycast => stickyRaycast;
        public SafetyBoxcastController SafetyBoxcast => safetyBoxcast;
        public Vector2 Speed => speed.Value;
        public bool GravityActive => gravityActive.Value;
        public float FallSlowFactor => fallSlowFactor.Value;
        public bool IsCollidingWithMovingPlatform => isCollidingWithMovingPlatform.Value;
        public Vector2 MovingPlatformCurrentSpeed => movingPlatformCurrentSpeed.Value;
        public bool WasTouchingCeilingLastFrame => wasTouchingCeilingLastFrame.Value;
        public float MovementDirectionThreshold => movementDirectionThreshold.Value;
        public bool CastRaysOnBothSides => castRaysOnBothSides.Value;
        public Vector2 ExternalForce => externalForce.Value;
        public int HorizontalMovementDirection => horizontalMovementDirection.Value;
        public Vector2 HorizontalRaycastFromBottom => horizontalRaycastFromBottom.Value;
        public Vector2 HorizontalRaycastToTop => horizontalRaycastToTop.Value;
        public int NumberOfHorizontalRays => numberOfHorizontalRays.Value;
        public int RightHitsStorageIndex => rightHitsStorageIndex.Value;
        public int LeftHitsStorageIndex => leftHitsStorageIndex;
        public float HorizontalRayLength => horizontalRayLength.Value;
        public int HorizontalHitsStorageLength => horizontalHitsStorageLength;
        public Vector2 CurrentRightRaycastOrigin => currentRightRaycastOrigin.Value;
        public Vector2 CurrentLeftRaycastOrigin => currentLeftRaycastOrigin.Value;
        public int NumberOfHorizontalRaysPerSide => numberOfHorizontalRaysPerSide.Value;
        public bool WasGroundedLastFrame => wasGroundedLastFrame.Value;
        public float CurrentRightHitDistance => currentRightHitDistance.Value;
        public float CurrentLeftHitDistance => currentLeftHitDistance.Value;
        public Collider2D CurrentRightHitCollider => currentRightHitCollider.Value;
        public Collider2D CurrentLeftHitCollider => currentLeftHitCollider.Value;
        public Collider2D IgnoredCollider => ignoredCollider.Value;
        public float CurrentRightHitAngle => currentRightHitAngle.Value;

        public float CurrentLeftHitAngle => currentLeftHitAngle.Value;
        public float MaximumSlopeAngle => maximumSlopeAngle.Value;
        public bool IsGrounded => isGrounded.Value;
        public Vector2 NewPosition => newPosition.Value;
        public float SmallValue => smallValue.Value;
        public float Gravity => gravity.Value;
        public bool IsFalling => isFalling.Value;
        public bool OnMovingPlatform => onMovingPlatform.Value;
        public float DownRayLength => downRayLength.Value;
        public int DownHitsStorageLength => downHitsStorageLength.Value;
        public int NumberOfVerticalRaysPerSide => numberOfHorizontalRaysPerSide.Value;
        public GameObject StandingOnLastFrame => standingOnLastFrame.Value;
        public bool IsStandingOnLastFrameNotNull => isStandingOnLastFrameNotNull.Value;
        public LayerMask MidHeightOneWayPlatformMask => midHeightOneWayPlatformMask.Value;
        public LayerMask StairsMask => stairsMask.Value;
        public Collider2D StandingOnCollider => standingOnCollider.Value;
        public Vector2 ColliderBottomCenterPosition => colliderBottomCenterPosition.Value;
        public float SmallestDistance => smallestDistance.Value;
        public int DownHitsStorageSmallestDistanceIndex => downHitsStorageSmallestDistanceIndex.Value;
        public bool DownHitConnected => downHitConnected.Value;

        public float CurrentDownHitSmallestDistance => currentDownHitSmallestDistance.Value;
        public RaycastHit2D RaycastDownHitAt => raycastDownHitAt.Value;
        public Vector3 CrossBelowSlopeAngle => crossBelowSlopeAngle.Value;
        public GameObject StandingOnWithSmallestDistance => standingOnWithSmallestDistance.Value;
        public Collider2D StandingOnWithSmallestDistanceCollider => standingOnWithSmallestDistanceCollider.Value;
        public LayerMask StandingOnWithSmallestDistanceLayer => standingOnWithSmallestDistanceLayer.Value;
        public float BoundsHeight => boundsHeight.Value;
        public LayerMask OneWayPlatformMask => oneWayPlatformMask.Value;
        public LayerMask MovingOneWayPlatformMask => movingOneWayPlatformMask.Value;
        public bool HasPhysicsMaterialDataClosestToDownHit => hasPhysicsMaterialClosestToDownHit.Value;
        public bool HasPathMovementControllerClosestToDownHit => hasPathMovementClosestToDownHit.Value;
        public bool HasMovingPlatform => hasMovingPlatform.Value;
        public bool StickToSlopesControl => stickToSlopesControl.Value;
        public float StickToSlopesOffsetY => stickToSlopesOffsetY.Value;
        public bool IsJumping => isJumping.Value;
        public float StickyRaycastLength => stickyRaycastLength.Value;
        public float LeftStickyRaycastLength => leftStickyRaycastLength.Value;
        public float RightStickyRaycastLength => rightStickyRaycastLength.Value;
        public Vector3 CrossBelowSlopeAngleLeft => crossBelowSlopeAngleLeft.Value;
        public Vector3 CrossBelowSlopeAngleRight => crossBelowSlopeAngleRight.Value;
        public float BelowSlopeAngleLeft => belowSlopeAngleLeft.Value;
        public float BelowSlopeAngleRight => belowSlopeAngleRight.Value;
        public bool CastFromLeft => castFromLeft.Value;
        public bool SafetyBoxcastControl => safetyBoxcastControl.Value;
        public bool HasSafetyBoxcast => hasSafetyBoxcast.Value;
        public Collider2D SafetyBoxcastCollider => safetyBoxcastCollider.Value;
        public RaycastHit2D LeftStickyRaycast => leftStickyRaycast.Value;
        public RaycastHit2D RightStickyRaycast => rightStickyRaycast.Value;
        public int UpHitsStorageLength => upHitsStorageLength.Value;
        public RaycastHit2D RaycastUpHitAt => raycastUpHitAt.Value;
        public float UpRaycastSmallestDistance => upRaycastSmallestDistance.Value;
        public bool UpHitConnected => upHitConnected.Value;
        public bool DistanceToGroundRaycastNotNull => distanceToGroundRaycastNotNull.Value;
        public RaycastHit2D DistanceToGroundRaycast => distanceToGroundRaycast.Value;

        /* properties */
        public static readonly string ModelPath = $"{PlatformerScriptableObjectsPath}{ModelAssetPath}";
    }
}