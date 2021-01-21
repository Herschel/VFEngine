﻿using System.Collections.Generic;
using UnityEngine;
using VFEngine.Tools;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable NotAccessedField.Local
namespace VFEngine.Platformer.Event.Raycast.ScriptableObjects
{
    using static ScriptableObjectExtensions;
    using static Debug;
    using static Vector2;

    [CreateAssetMenu(fileName = "RaycastData", menuName = PlatformerRaycastDataPath, order = 0)]
    public class RaycastData : ScriptableObject
    {
        #region events

        #endregion

        #region properties

        public bool CastRaysOnBothSides { get; private set; }

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
            None
        }

        #endregion

        #region fields

        private Vector2 originalColliderSize;
        private Vector2 originalColliderOffset;
        private Vector2 originalRaycastOriginSize;
        private Vector2 colliderBottomCenterPosition;
        private Vector2 colliderLeftCenterPosition;
        private Vector2 colliderRightCenterPosition;
        private Vector2 colliderTopCenterPosition;
        private bool drawRaycastGizmosControl;
        private bool displayWarnings;
        private int numberOfHorizontalRays;
        private int numberOfVerticalRays;
        private float rayOffset;
        private float crouchedRaycastLengthMultiplier;
        private float distanceToGroundRaycastMaximumLength;
        private bool performSafetyBoxcast;
        private float stickyRaycastLength;
        private float stickyRaycastOffsetY;
        private BoxCollider2D boxCollider;
        private Transform transform;
        private const float SmallValue = 0.0001f;
        private const float MovingPlatformsGravity = -500f;
        private float obstacleHeightTolerance;
        private Vector2 horizontalRaycastFromBottom;
        private Vector2 horizontalRaycastToTop;
        private Vector2 verticalRaycastFromLeft;
        private Vector2 verticalRaycastToRight;
        private Vector2 aboveRaycastStart;
        private Vector2 aboveRaycastEnd;
        private Vector2 raycastOrigin;
        private Direction direction;
        private Bounds bounds;
        private Collision collision;

        private struct Collision
        {
            public bool Right { get; set; }
            public bool Left { get; set; }
            public bool Above { get; set; }
            public bool Below { get; set; }
            public bool Colliding => Right || Left || Above || Below;
            public float DistanceToLeftCollider { get; set; }
            public float DistanceToRightCollider { get; set; }
            public float HorizontalSlopeAngle { get; set; }
            public float BelowSlopeAngle { get; set; }
            public bool PassedSlopeAngle { get; set; }
            public bool OnMovingPlatform { get; set; }
            public bool IsGrounded => Below;
            public bool GroundedLastFrame { get; set; }
            public bool CollidedWithCeilingLastFrame { get; set; }
            public bool GroundedEvent { get; set; }
            public bool BoundsResized { get; set; }
            public bool CollidingWithLevelBounds { get; set; }
            public GameObject StandingOn { get; set; }
            public GameObject StandingOnLastFrame { get; set; }
            public Collider2D StandingOnCollider { get; set; }
            public GameObject CurrentWallCollider { get; set; }
            public float Friction { get; set; }
            public float DistanceToGround { get; set; }
            public GameObject MovingPlatform { get; set; }
            public float MovingPlatformCurrentGravity { get; set; }
            public Collider2D IgnoredCollider { get; set; }
            public bool StairsCollisionControl { get; set; }
            public Vector2 CrossBelowSlopeAngle { get; set; }
            public RaycastHit2D[] HorizontalHitStorage { get; set; }
            public RaycastHit2D[] BelowHitStorage { get; set; }
            public RaycastHit2D[] AboveHitStorage { get; set; }
            public RaycastHit StickToSlopeRaycast { get; set; }
            public RaycastHit LeftStickToSlopeRaycast { get; set; }
            public RaycastHit RightStickToSlopeRaycast { get; set; }
            public RaycastHit DistanceToGroundRaycast { get; set; }
            public List<RaycastHit2D> ContactList { get; set; }

            public void Reset()
            {
                Left = false;
                Right = false;
                Above = false;
                DistanceToLeftCollider = -1;
                DistanceToRightCollider = 1;
                PassedSlopeAngle = false;
                GroundedEvent = false;
                HorizontalSlopeAngle = 0f;
            }
        }

        private struct Bounds
        {
            public Vector2 Size { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 BottomPosition { get; set; }
            public Vector2 LeftPosition { get; set; }
            public Vector2 TopPosition { get; set; }
            public Vector2 RightPosition { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public Vector2 RaycastBounds { get; set; }
            public Vector2 TopLeft { get; set; }
            public Vector2 TopRight { get; set; }
            public Vector2 BottomLeft { get; set; }
            public Vector2 BottomRight { get; set; }
            public Vector2 Top { get; set; }
            public Vector2 Bottom { get; set; }
            public Vector2 Right { get; set; }
            public Vector2 Left { get; set; }
            public Vector2 Center { get; set; }
        }

        #endregion

        #region initialization

        private void Initialize(ref BoxCollider2D collider, ref GameObject character, RaycastSettings settings)
        {
            ApplySettings(settings);
            InitializeDefault(ref collider, ref character);
        }

        private void ApplySettings(RaycastSettings settings)
        {
            drawRaycastGizmosControl = settings.drawRaycastGizmosControl;
            displayWarnings = settings.displayWarnings;
            numberOfHorizontalRays = settings.numberOfHorizontalRays;
            numberOfVerticalRays = settings.numberOfVerticalRays;
            rayOffset = settings.rayOffset;
            crouchedRaycastLengthMultiplier = settings.crouchedRaycastLengthMultiplier;
            CastRaysOnBothSides = settings.castRaysOnBothSides;
            distanceToGroundRaycastMaximumLength = settings.distanceToGroundRaycastMaximumLength;
            performSafetyBoxcast = settings.performSafetyBoxcast;
            stickyRaycastLength = settings.stickyRaycastLength;
            stickyRaycastOffsetY = settings.stickyRaycastOffsetY;
            obstacleHeightTolerance = settings.obstacleHeightTolerance;
        }

        private bool DisplayBoxColliderWarning => boxCollider.offset.x != 0 && displayWarnings;

        private void InitializeDefault(ref BoxCollider2D collider, ref GameObject character)
        {
            boxCollider = collider;
            transform = character.transform;
            originalColliderSize = collider.size;
            originalColliderOffset = collider.offset;
            if (DisplayBoxColliderWarning) LogWarning("collider x offset must be zero.");
            collision.Friction = 0;
            collision.ContactList = new List<RaycastHit2D>();
            collision.HorizontalHitStorage = new RaycastHit2D[numberOfHorizontalRays];
            collision.BelowHitStorage = new RaycastHit2D[numberOfVerticalRays];
            collision.AboveHitStorage = new RaycastHit2D[numberOfVerticalRays];
            collision.CurrentWallCollider = null;
            collision.Reset();
            UpdateBounds(collider, character);
            bounds.RaycastBounds = new Vector2(bounds.Width, bounds.Height);
            bounds.Top = (bounds.TopLeft + bounds.TopRight) / 2;
            bounds.Bottom = (bounds.BottomLeft + bounds.BottomRight) / 2;
            bounds.Right = (bounds.TopRight + bounds.BottomRight) / 2;
            bounds.Left = (bounds.TopLeft + bounds.BottomLeft) / 2;
            horizontalRaycastFromBottom = zero;
            horizontalRaycastToTop = zero;
            verticalRaycastFromLeft = zero;
            verticalRaycastToRight = zero;
            aboveRaycastStart = zero;
            aboveRaycastEnd = zero;
            raycastOrigin = zero;
        }

        private void UpdateBounds(BoxCollider2D collider, GameObject character)
        {
            var offset = collider.offset;
            var size = collider.size;
            var top = offset.y + size.y / 2f;
            var bottom = offset.y - size.y / 2f;
            var left = offset.x - size.x / 2f;
            var right = offset.x + size.x / 2f;
            SetBounds(top, bottom, left, right);
            var topLeft = character.transform.TransformPoint(bounds.TopLeft);
            var topRight = character.transform.TransformPoint(bounds.TopRight);
            var bottomLeft = character.transform.TransformPoint(bounds.BottomLeft);
            var bottomRight = character.transform.TransformPoint(bounds.BottomRight);
            SetBounds(topLeft, topRight, bottomLeft, bottomRight);
            SetBoundsCenter(collider.bounds.center);
            var width = Distance(bounds.BottomLeft, bounds.BottomRight);
            var height = Distance(bounds.BottomLeft, bounds.TopLeft);
            SetBounds(width, height);
        }

        private void SetBoundsCenter(Vector2 center)
        {
            bounds.Center = center;
        }

        private void SetBounds(float width, float height)
        {
            SetBoundsWidth(width);
            SetBoundsHeight(height);
        }

        private void SetBoundsWidth(float width)
        {
            bounds.Width = width;
        }

        private void SetBoundsHeight(float height)
        {
            bounds.Height = height;
        }

        private void SetBounds(float boundsTop, float boundsBottom, float boundsLeft, float boundsRight)
        {
            SetBoundsTopLeft(boundsLeft, boundsTop);
            SetBoundsTopRight(boundsRight, boundsTop);
            SetBoundsBottomLeft(boundsLeft, boundsBottom);
            SetBoundsBottomRight(boundsRight, boundsBottom);
        }

        private void SetBounds(Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
        {
            SetBoundsTopLeft(topLeft);
            SetBoundsTopRight(topRight);
            SetBoundsBottomLeft(bottomLeft);
            SetBoundsBottomRight(bottomRight);
        }

        private void SetBoundsTopLeft(float x, float y)
        {
            bounds.TopLeft = new Vector2(x, y);
        }

        private void SetBoundsTopLeft(Vector2 topLeft)
        {
            bounds.TopLeft = topLeft;
        }

        private void SetBoundsTopRight(float x, float y)
        {
            bounds.TopRight = new Vector2(x, y);
        }

        private void SetBoundsTopRight(Vector2 topRight)
        {
            bounds.TopRight = topRight;
        }

        private void SetBoundsBottomLeft(float x, float y)
        {
            bounds.BottomLeft = new Vector2(x, y);
        }

        private void SetBoundsBottomLeft(Vector2 bottomLeft)
        {
            bounds.BottomLeft = bottomLeft;
        }

        private void SetBoundsBottomRight(float x, float y)
        {
            bounds.BottomRight = new Vector2(x, y);
        }

        private void SetBoundsBottomRight(Vector2 bottomRight)
        {
            bounds.BottomRight = bottomRight;
        }

        #endregion

        #region public methods

        #endregion

        #region private methods

        #endregion

        #region event handlers

        public void OnInitialize(ref BoxCollider2D collider, ref GameObject character, RaycastSettings settings)
        {
            Initialize(ref collider, ref character, settings);
        }

        #endregion
    }
}

#region hide

/*
 SetIgnoreFriction(false);
        SetIndex(0);
        SetRaycastLength(0);
        IgnorePlatformsTime = 0;
        SetRaycastOrigin(new Vector2());
        SetRaycastHit(new RaycastHit2D());
        private bool OnSlopeCollision => collision.OnGround && collision.GroundAngle != 0;
        collision.OnSlope = OnSlopeCollision;
        private void InitializeRaycastCollisionDefault()
        {
            
        }
        private void SetRaycastCount()
        {
            HorizontalRays = RaycastCount(bounds.size.y);
            VerticalRays = RaycastCount(bounds.size.x);
        }

        private int RaycastCount(float size)
        {
            return (int) Round(size / spacing);
        }

        private void SetRaycastSpacing()
        {
            HorizontalSpacing = RaycastSpacing(bounds.size.y, HorizontalRays);
            VerticalSpacing = RaycastSpacing(bounds.size.x, VerticalRays);
        }

        private static float RaycastSpacing(float axis, int rays)
        {
            return axis / (rays - 1);
        }

        private void SetCollisionRight(bool colliding)
        {
            collision.Right = colliding;
        }

        private void SetCollisionLeft(bool colliding)
        {
            collision.Left = colliding;
        }

        private void SetCollisionOnGround(bool colliding)
        {
            collision.OnGround = colliding;
        }

        private void SetCollisionGroundDirectionAxis(int axis)
        {
            collision.GroundDirectionAxis = axis;
        }

        private void SetCollisionHorizontalHit(RaycastHit2D hit)
        {
            collision.HorizontalHit = hit;
        }

        private void SetCollisionVerticalHit(RaycastHit2D hit)
        {
            collision.VerticalHit = hit;
        }

        private void UpdateOrigins(BoxCollider2D boxCollider)
        {
            bounds = boxCollider.bounds;
            bounds.Expand(SkinWidth * 2);
            origins.BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            origins.BottomRight = new Vector2(bounds.max.x, bounds.min.y);
            origins.TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            origins.TopRight = new Vector2(bounds.max.x, bounds.max.y);
        }

        /*private void InitializeFrame(BoxCollider2D boxCollider)
        {
            ResetCollision();
            UpdateBounds(boxCollider);
        }*/ /*

        private void SetGroundCollisionRaycast(int deltaMoveXDirectionAxis, int index, LayerMask layer)
        {
            SetRaycastOrigin(GroundCollisionRaycastOrigin(deltaMoveXDirectionAxis, index));
            SetRaycastHit(GroundCollisionRaycastHit(layer));
        }
        
        private void SetRaycastOrigin(Vector2 raycastOrigin)
        {
            Origin = raycastOrigin;
        }
        private Vector2 GroundCollisionRaycastOrigin(int deltaMoveXDirectionAxis, int index)
        {
            var groundCollisionOrigin = deltaMoveXDirectionAxis == 1 ? origins.BottomLeft : origins.BottomRight;
            origin += (deltaMoveXDirectionAxis == 1 ? right : left) * (VerticalSpacing * index);
            origin.y += SkinWidth * 2;
            return groundCollisionOrigin;
        }
        private RaycastHit2D GroundCollisionRaycastHit(LayerMask layer)
        {
            return Raycast(Origin, down, SkinWidth * 4f, layer);
        }

        private void SetGroundCollisionRaycast(LayerMask layer)
        {
            SetRaycastHit(GroundCollisionRaycastHit(layer));
        }

        private void SetCollisionOnGroundCollisionRaycastHit()
        {
            SetCollisionOnGround(true);
            SetCollisionGroundAngle(HitAngle);
            SetCollisionGroundLayer(Hit.collider.gameObject.layer);
            SetCollisionVerticalHit(Hit);
            SetCollisionBelow(true);
        }

        private void SetCollisionGroundAngle(float angle)
        {
            collision.GroundAngle = angle;
        }

        private void SetCollisionGroundLayer(LayerMask layer)
        {
            collision.GroundLayer = layer;
        }

        private void CastGroundCollisionRay()
        {
            CastRay(Origin, down * SkinWidth * 2, blue);
        }

        private static void CastRay(Vector2 start, Vector2 direction, Color color)
        {
            DrawRay(start, direction, color);
        }

        private void ResetCollision()
        {
            SetCollisionAbove(false);
            SetCollisionBelow(false);
            SetCollisionLeft(false);
            SetCollisionRight(false);
            SetCollisionHorizontalHit(new RaycastHit2D());
            SetCollisionVerticalHit(new RaycastHit2D());
            SetCollisionOnGround(false);
            SetCollisionGroundAngle(0);
            SetCollisionGroundDirectionAxis(0);
        }
        
        /*private Vector2 GroundCollisionRaycastDirection => down * SkinWidth * 2;

        private void OnGroundCollisionRaycastHitInternal()
        {
            SetCollisionOnGroundCollisionRaycastHit();
            CastRay(Origin, GroundCollisionRaycastDirection, blue);
        }*/
/*private int RaycastHitDirection => (int) Sign(Hit.normal.x);
private LayerMask RaycastHitLayer => Hit.collider.gameObject.layer;

private void SetCollisionOnGroundCollisionRaycastHit()
{
    SetCollisionOnRaycastHitGround(true);
    SetCollisionGroundLayer(RaycastHitLayer);
    SetCollisionVerticalHit(Hit);
    SetCollisionBelow(true);
}

private void SetCollisionOnRaycastHitGround(bool onGround)
{
    SetCollisionOnGround(onGround);
    SetGroundCollision(HitAngle, RaycastHitDirection);
}

private void SetCollisionGroundLayer(LayerMask layer)
{
    collision.GroundLayer = layer;
}

private void SetCollisionGroundAngle(float angle)
{
    collision.GroundAngle = angle;
}

private static void CastRay(Vector2 start, Vector2 direction, Color color)
{
    DrawRay(start, direction, color);
}*/
/*
        private void SlopeBehavior()
        {
            SetCollisionBelow(true);
        }

        private void SetCollisionBelow(bool colliding)
        {
            collision.Below = colliding;
        }

        private void SetHorizontalCollisionRaycast(int index, int deltaMoveXDirectionAxis, float deltaMoveDistanceX,
            LayerMask layer)
        {
            SetIndex(index);
            SetRaycastLength(RaycastLength(deltaMoveDistanceX));
            //SetRaycastOrigin(HorizontalCollisionRaycastOrigin(deltaMoveXDirectionAxis));
            SetRaycastHit(HorizontalCollisionRaycastHit(deltaMoveXDirectionAxis, layer));
            //CastRay(Origin, right * deltaMoveXDirectionAxis * Length, red);
        }

        private void SetRaycastLength(float length)
        {
            Length = length;
        }

        private float RaycastLength(float deltaMoveDistance)
        {
            return deltaMoveDistance + SkinWidth;
        }

        private RaycastHit2D HorizontalCollisionRaycastHit(int deltaMoveXDirectionAxis, LayerMask layer)
        {
            return RaycastHit(Origin, right * deltaMoveXDirectionAxis, Length, layer);
        }

        private static RaycastHit2D RaycastHit(Vector2 raycastOrigin, Vector2 direction, float distance,
            LayerMask layer)
        {
            return Raycast(raycastOrigin, direction, distance, layer);
        }

        //private Vector2 HorizontalCollisionRaycastOrigin(int deltaMoveXDirectionAxis)
        //{
            //return RaycastOrigin(deltaMoveXDirectionAxis == -1, Index, HorizontalSpacing, up);
        //}

        private Vector2 RaycastOrigin(bool directionAxis, int index, float raycastSpacing, Vector2 direction)
        {
            return (directionAxis ? origins.BottomLeft : origins.BottomRight) + direction * (raycastSpacing * index);
        }

        

        private void SetRaycastHit(RaycastHit2D hit)
        {
            Hit = hit;
        }

        private void SetIndex(int index)
        {
            //Index = index;
        }

        private float HorizontalCollisionHitClimbingSlopeLength(float deltaMoveDistanceX)
        {
            return Min(deltaMoveDistanceX + SkinWidth, Hit.distance);
        }

        private void SetCollisionOnHorizontalCollisionHitMaximumSlope(int deltaMoveXDirectionAxis)
        {
            SetCollisionLeft(deltaMoveXDirectionAxis < 0);
            SetCollisionRight(deltaMoveXDirectionAxis > 0);
            SetCollisionHorizontalHit(Hit);
        }

        private void StopHorizontalSpeed(LayerMask layer)
        {
            SetRaycastOrigin(origins.BottomRight);
            SetCollisionHorizontalHit(StopHorizontalSpeedHit(layer));
        }

        private Vector2 StopHorizontalSpeedRaycastDirection => left * collision.GroundDirectionAxis;

        private RaycastHit2D StopHorizontalSpeedHit(LayerMask layer)
        {
            return Raycast(Origin, StopHorizontalSpeedRaycastDirection, 1f, layer);
        }

        private void SetVerticalCollisionRaycast(int index, int deltaMoveYDirectionAxis, float deltaMoveDistanceY,
            float deltaMoveX, LayerMask layer)
        {
            SetIndex(index);
            SetRaycastLength(RaycastLength(deltaMoveDistanceY));
            //SetRaycastOrigin(VerticalCollisionRaycastOrigin(deltaMoveYDirectionAxis, deltaMoveX));
            SetRaycastHit(VerticalCollisionRaycast(deltaMoveYDirectionAxis, layer));
            //CastRay(Origin, up * deltaMoveYDirectionAxis * Length, red);
        }

        //private Vector2 VerticalCollisionRaycastOrigin(int deltaMoveYDirectionAxis, float deltaMoveX)
        //{
            //return (deltaMoveYDirectionAxis == -1 ? origins.BottomLeft : bounds.TopLeft) +
            //       right * (VerticalSpacing * Index * deltaMoveX);
        //}

        private RaycastHit2D VerticalCollisionRaycast(int deltaMoveYDirectionAxis, LayerMask layer)
        {
            return Raycast(Origin, up * deltaMoveYDirectionAxis, Length, layer);
        }

        private void SetCollisionOnVerticalCollisionRaycastHit(int deltaMoveYDirectionAxis)
        {
            SetCollisionAbove(deltaMoveYDirectionAxis > 0);
            SetCollisionBelow(deltaMoveYDirectionAxis < 0);
            SetCollisionVerticalHit(Hit);
        }

        private void SetCollisionAbove(bool colliding)
        {
            collision.Above = colliding;
        }

        private void ClimbSteepSlope(int deltaMoveXDirectionAxis, float deltaMoveDistanceX, float deltaMoveY,
            LayerMask layer)
        {
            SetRaycastLength(ClimbSteepSlopeLength(deltaMoveDistanceX));
            SetRaycastOrigin(ClimbSteepSlopeOrigin(deltaMoveXDirectionAxis, deltaMoveY));
            SetRaycastHit(ClimbSteepSlopeHit(deltaMoveXDirectionAxis, layer));
        }

        private float ClimbSteepSlopeLength(float deltaMoveDistanceX)
        {
            return deltaMoveDistanceX + SkinWidth * 2;
        }

        private Vector2 ClimbSteepSlopeOrigin(int deltaMoveXDirectionAxis, float deltaMoveY)
        {
            return (deltaMoveXDirectionAxis == -1 ? origins.BottomLeft : origins.BottomRight) + up * deltaMoveY;
        }

        private RaycastHit2D ClimbSteepSlopeHit(int deltaMoveXDirectionAxis, LayerMask layer)
        {
            return Raycast(Origin, right * deltaMoveXDirectionAxis, Length, layer);
        }

        private void SetGroundCollision(float angle, int direction)
        {
            SetCollisionGroundAngle(angle);
            SetCollisionGroundDirectionAxis(direction);
        }

        private void ClimbSteepSlopeHit()
        {
            //SetGroundCollision(HitAngle, RaycastHitDirection);
        }

        private void ClimbMildSlope(int deltaMoveXDirectionAxis, Vector2 deltaMove, LayerMask layer)
        {
            SetRaycastOrigin(ClimbMildSlopeOrigin(deltaMoveXDirectionAxis, deltaMove));
            SetRaycastHit(ClimbMildSlopeHit(layer));
            //CastRay(Origin, down, yellow);
        }

        private Vector2 ClimbMildSlopeOrigin(int deltaMoveXDirectionAxis, Vector2 deltaMove)
        {
            return (deltaMoveXDirectionAxis == -1 ? origins.BottomLeft : origins.BottomRight) + deltaMove;
        }

        private RaycastHit2D ClimbMildSlopeHit(LayerMask layer)
        {
            return Raycast(Origin, down, 1f, layer);
        }

        private void DescendMildSlope(int deltaMoveXDirectionAxis, float deltaMoveDistanceY, float deltaMoveX,
            LayerMask layer)
        {
            SetRaycastLength(DescendMildSlopeLength(deltaMoveDistanceY));
            SetRaycastOrigin(DescendMildSlopeOrigin(deltaMoveXDirectionAxis, deltaMoveX));
            SetRaycastHit(DescendMildSlopeHit(layer));
        }

        private float DescendMildSlopeLength(float deltaMoveDistanceY)
        {
            return deltaMoveDistanceY + SkinWidth;
        }

        private Vector2 DescendMildSlopeOrigin(int deltaMoveXDirectionAxis, float deltaMoveX)
        {
            return (deltaMoveXDirectionAxis == -1 ? origins.BottomRight : origins.BottomLeft) + right * deltaMoveX;
        }

        private RaycastHit2D DescendMildSlopeHit(LayerMask layer)
        {
            return Raycast(Origin, down, Length, layer);
        }

        private void DescendMildSlopeHit()
        {
            SetGroundCollision(HitAngle, (int) Sign(Hit.normal.x));
        }

        private void DescendSteepSlope(int deltaMoveXDirectionAxis, Vector2 deltaMove, LayerMask layer)
        {
            SetRaycastOrigin(DescendSteepSlopeOrigin(deltaMoveXDirectionAxis, deltaMove));
            SetRaycastHit(DescendSteepSlopeHit(layer));
            //CastRay(Origin, down, yellow);
        }

        private Vector2 DescendSteepSlopeOrigin(int deltaMoveXDirectionAxis, Vector2 deltaMove)
        {
            return (deltaMoveXDirectionAxis == 1 ? origins.BottomLeft : origins.BottomRight) + deltaMove;
        }

        private RaycastHit2D DescendSteepSlopeHit(LayerMask layer)
        {
            return Raycast(Origin, down, 1f, layer);
        }

        private void ResetFriction()
        {
            SetIgnoreFriction(false);
        }

        private void SetIgnoreFriction(bool ignore)
        {
            IgnoreFriction = ignore;
        }
public bool IgnoreFriction { get; private set; }
public int VerticalRays { get; private set; }
public float SkinWidth { get; private set; }
private float VerticalSpacing { get; set; }
public float HitAngle => Angle(Hit.normal, up);
public RaycastHit2D Hit { get; private set; }
public Vector2 OriginsBottomLeft => origins.BottomLeft;
public Vector2 OriginsBottomRight => origins.BottomRight;
private Vector2 Origin { get; set; }
public bool OnGround => collision.OnGround;
public bool OnSlope => collision.OnSlope;
public float GroundAngle => collision.GroundAngle;
public int GroundDirectionAxis => collision.GroundDirectionAxis;
public int HorizontalRays { get; private set; }
private float HorizontalSpacing { get; set; }
public Vector2 OriginsTopLeft => origins.TopLeft;
public const float Tolerance = 0;
public LayerMask GroundLayer => collision.GroundLayer;
public RaycastHit2D VerticalHit => collision.VerticalHit;
public bool CollidingBelow => collision.Below;
public bool CollidingAbove => collision.Above;
public float IgnorePlatformsTime { get; private set; }
private float Length { get; set; }

private bool displayWarnings;
private bool drawGizmos;
private float spacing;
private float oneWayPlatformDelay;
private float ladderClimbThreshold;
private float ladderDelay;
private Vector2 origin;
private Bounds bounds;
private Origins origins;
private Collision collision;
private struct Origins
{
    public Vector2 TopLeft { get; set; }
    public Vector2 TopRight { get; set; }
    public Vector2 BottomLeft { get; set; }
    public Vector2 BottomRight { get; set; }
}

private struct Collision
{
    public bool Above { get; set; }
    public bool Right { get; set; }
    public bool Below { get; set; }
    public bool Left { get; set; }
    public bool OnGround { get; set; }
    public bool OnSlope { get; set; }
    public int GroundDirectionAxis { get; set; }
    public float GroundAngle { get; set; }
    public LayerMask GroundLayer { get; set; }
    public RaycastHit2D HorizontalHit { get; set; }
    public RaycastHit2D VerticalHit { get; set; }
}ApplySettings(settings);
InitializeDefault();
UpdateOrigins(boxCollider);
InitializeRaycastCollisionDefault();
SetRaycastCount();
SetRaycastSpacing();spacing = settings.spacing;
SkinWidth = settings.skinWidth;
oneWayPlatformDelay = settings.oneWayPlatformDelay;
ladderClimbThreshold = settings.ladderClimbThreshold;
ladderDelay = settings.ladderDelay;
public void OnInitialize(BoxCollider2D boxCollider, RaycastSettings settings)
        {
            Initialize(boxCollider, settings);
        }

        /*public void OnInitializeFrame(BoxCollider2D boxCollider)
        {
            InitializeFrame(boxCollider);
        }*/ /*

        public void OnUpdateOrigins(BoxCollider2D boxCollider)
        {
            UpdateOrigins(boxCollider);
        }

        public void OnSetGroundCollisionRaycast(int index, int deltaMoveXDirectionAxis, LayerMask layer)
        {
            SetGroundCollisionRaycast(index, deltaMoveXDirectionAxis, layer);
        }

        public void OnSetGroundCollisionRaycast(LayerMask layer)
        {
            SetGroundCollisionRaycast(layer);
        }

        public void OnSetCollisionOnGroundCollisionRaycastHit()
        {
            SetCollisionOnGroundCollisionRaycastHit();
        }

        public void OnCastGroundCollisionRay()
        {
            CastGroundCollisionRay();
        }

        public void OnResetCollision()
        {
            ResetCollision();
        }

        /*public void OnSetGroundCollisionRaycastHit(LayerMask layer)
        {
            SetRaycastHit(GroundCollisionRaycastHit(layer));
        }*/
/*public void OnGroundCollisionRaycastHit()
{
    OnGroundCollisionRaycastHitInternal();
}*/ /*

        public void OnSlopeBehavior()
        {
            SlopeBehavior();
        }

        private void CastRayOnMove(Transform transform, Vector2 deltaMove)
        {
            CastRay(transform.position, deltaMove * 3f, green);
        }

        public void OnSetHorizontalCollisionRaycast(int index, int deltaMoveXDirectionAxis, float deltaMoveDistanceX,
            LayerMask layer)
        {
            SetHorizontalCollisionRaycast(index, deltaMoveXDirectionAxis, deltaMoveDistanceX, layer);
        }

        public void OnHorizontalCollisionRaycastHitClimbingSlope()
        {
            //SetCollisionOnRaycastHitGround(true);
        }

        public void OnHorizontalCollisionRaycastHitClimbingSlope(float deltaMoveDistanceX)
        {
            SetRaycastLength(HorizontalCollisionHitClimbingSlopeLength(deltaMoveDistanceX));
        }

        public void OnHorizontalCollisionRaycastHitMaximumSlope(int deltaMoveXDirectionAxis)
        {
            SetCollisionOnHorizontalCollisionHitMaximumSlope(deltaMoveXDirectionAxis);
        }

        public void OnStopHorizontalSpeed(LayerMask layer)
        {
            StopHorizontalSpeed(layer);
        }

        public void OnSetVerticalCollisionRaycast(int index, int deltaMoveYDirectionAxis, float deltaMoveDistanceY,
            float deltaMoveX, LayerMask layer)
        {
            SetVerticalCollisionRaycast(index, deltaMoveYDirectionAxis, deltaMoveDistanceY, deltaMoveX, layer);
        }

        public void OnSetVerticalCollisionRaycastHit(int deltaMoveYDirectionAxis, LayerMask layer)
        {
            SetRaycastHit(VerticalCollisionRaycast(deltaMoveYDirectionAxis, layer));
        }

        public void OnSetVerticalCollisionRaycastLengthOnHit()
        {
            SetRaycastLength(Hit.distance);
        }

        public void OnVerticalCollisionRaycastHit(int deltaMoveYDirectionAxis)
        {
            SetCollisionOnVerticalCollisionRaycastHit(deltaMoveYDirectionAxis);
        }

        public void OnClimbSteepSlope(int deltaMoveXDirectionAxis, float deltaMoveDistanceX, float deltaMoveY,
            LayerMask layer)
        {
            ClimbSteepSlope(deltaMoveXDirectionAxis, deltaMoveDistanceX, deltaMoveY, layer);
        }

        public void OnClimbSteepSlopeHit()
        {
            ClimbSteepSlopeHit();
        }

        public void OnClimbMildSlope(int deltaMoveXDirectionAxis, Vector2 deltaMove, LayerMask layer)
        {
            ClimbMildSlope(deltaMoveXDirectionAxis, deltaMove, layer);
        }

        public void OnDescendMildSlope(int deltaMoveXDirectionAxis, float deltaMoveDistanceY, float deltaMoveX,
            LayerMask layer)
        {
            DescendMildSlope(deltaMoveXDirectionAxis, deltaMoveDistanceY, deltaMoveX, layer);
        }

        public void OnDescendMildSlopeHit()
        {
            DescendMildSlopeHit();
        }

        public void OnDescendSteepSlope(int deltaMoveXDirectionAxis, Vector2 deltaMove, LayerMask layer)
        {
            DescendSteepSlope(deltaMoveXDirectionAxis, deltaMove, layer);
        }

        public void OnCastRayOnMove(GameObject character, Vector2 deltaMove)
        {
            CastRayOnMove(character.transform, deltaMove);
        }

        public void OnResetFriction()
        {
            ResetFriction();
        }*/

#endregion