﻿using UnityEngine;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace VFEngine.Platformer.Event.Raycast
{
    using static Vector2;
    using static Mathf;
    public struct RaycastCollision
    {
        #region properties

        public bool Above { get; set; }
        public bool Right { get; set; }
        public bool Below { get; set; }
        public bool Left { get; set; }
        public bool OnGround { get; set; }
        public bool OnSlope { get; set; }
        public int GroundLayer { get; set; }
        public int GroundDirection { get; set; }
        public float GroundAngle { get; set; }
        public RaycastHit2D HorizontalHit { get; set; }
        public RaycastHit2D VerticalHit { get; set; }

        #region public methods
        
        public void Initialize()
        {
            Reset();
            GroundLayer = 0;
            OnSlope = OnGround && GroundAngle != 0;
        }

        public void Reset()
        {
            Above = false;
            Right = false;
            Below = false;
            Left = false;
            OnGround = false;
            GroundDirection = 0;
            GroundAngle = 0;
            HorizontalHit = new RaycastHit2D();
            VerticalHit = new RaycastHit2D();
        }

        public void OnDownHit(RaycastHit2D hit)
        {
            Below = true;
            OnGround = true;
            GroundLayer = hit.collider.gameObject.layer;
            SetGroundMeasurements(hit.normal);
            VerticalHit = hit;
        }

        public void SetCollisionBelow(bool collision)
        {
            Below = collision;
        }

        public void OnSideHit(RaycastHit2D hit)
        {
            OnGround = true;
            SetGroundMeasurements(hit.normal);
        }

        private void SetGroundMeasurements(Vector2 normal)
        {
            SetGroundDirection(normal.x);
            SetGroundAngle(normal);
        }
        private void SetGroundDirection(float x)
        {
            GroundDirection = (int) Sign(x);
        }

        private void SetGroundAngle(Vector2 normal)
        {
            GroundAngle = Angle(normal, up);
        }

        #endregion

        #endregion
    }
}