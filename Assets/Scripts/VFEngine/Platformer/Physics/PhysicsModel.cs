﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VFEngine.Tools;
using static UnityEngine.Time;
using UniTaskExtensions = VFEngine.Tools.UniTaskExtensions;

namespace VFEngine.Platformer.Physics
{
    using static ScriptableObjectExtensions;
    using static Quaternion;
    using static UniTaskExtensions;

    [CreateAssetMenu(fileName = "PhysicsModel", menuName = "VFEngine/Platformer/Physics/Physics Model", order = 0)]
    public class PhysicsModel : ScriptableObject, IModel
    {
        /* fields */
        [LabelText("Physics Data")] [SerializeField]
        private PhysicsData p;

        private const string AssetPath = "Physics/DefaultPhysicsModel.asset";

        /* fields: methods */
        private void InitializeData()
        {
            p.Initialize();
        }

        private void InitializeModel()
        {
            p.State.Reset();
            if (p.AutomaticGravityControl && !p.HasGravityController) p.Transform.rotation = identity;
        }

        private async UniTaskVoid SetGravityAsyncInternal()
        {
            p.CurrentGravity = p.Gravity;
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid ApplyAscentMultiplierToGravityAsyncInternal()
        {
            p.CurrentGravity /= p.AscentMultiplier;
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid ApplyFallMultiplierToGravityAsyncInternal()
        {
            p.CurrentGravity *= p.FallMultiplier;
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid ApplyGravityToSpeedAsyncInternal()
        {
            p.Speed = new Vector2(p.Speed.x, ApplyGravityTime());
            await SetYieldOrSwitchToThreadPoolAsync();

            float ApplyGravityTime()
            {
                return p.Speed.y + (p.CurrentGravity + p.MovingPlatformCurrentGravity) * deltaTime;
            }
        }

        private async UniTaskVoid ApplyFallSlowFactorToSpeedAsyncInternal()
        {
            p.Speed = new Vector2(p.Speed.x, ApplyFallSlowFactor());
            await SetYieldOrSwitchToThreadPoolAsync();

            float ApplyFallSlowFactor()
            {
                return p.Speed.y * p.FallSlowFactor;
            }
        }

        private async UniTaskVoid SetNewPositionAsyncInternal()
        {
            p.NewPosition = p.Speed * deltaTime;
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid ResetStateAsyncInternal()
        {
            p.State.Reset();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid TranslatePlatformSpeedToTransformAsyncInternal()
        {
            p.Transform.Translate(p.MovingPlatformCurrentSpeed * deltaTime);
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid DisableGravityAsyncInternal()
        {
            p.State.SetGravity(false);
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid ApplyPlatformSpeedToNewPositionAsyncInternal()
        {
            p.NewPosition = new Vector2(p.NewPosition.x, p.MovingPlatformCurrentSpeed.y * deltaTime);
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid StopSpeedAsyncInternal()
        {
            p.Speed = p.NewPosition / deltaTime;
            p.Speed = new Vector2(-p.Speed.x, p.Speed.y);
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        /* properties */
        public static string ModelPath => $"{DefaultPath}{PlatformerPath}{AssetPath}";

        /* properties: methods */
        public void Initialize()
        {
            InitializeData();
            InitializeModel();
        }

        public UniTask<UniTaskVoid> SetGravityAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(SetGravityAsyncInternal());
            }
            finally
            {
                SetGravityAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> ApplyAscentMultiplierToGravityAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(ApplyAscentMultiplierToGravityAsyncInternal());
            }
            finally
            {
                ApplyAscentMultiplierToGravityAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> ApplyFallMultiplierToGravityAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(ApplyFallMultiplierToGravityAsyncInternal());
            }
            finally
            {
                ApplyFallMultiplierToGravityAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> ApplyGravityToSpeedAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(ApplyGravityToSpeedAsyncInternal());
            }
            finally
            {
                ApplyGravityToSpeedAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> ApplyFallSlowFactorToSpeedAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(ApplyFallSlowFactorToSpeedAsyncInternal());
            }
            finally
            {
                ApplyFallSlowFactorToSpeedAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> SetNewPositionAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(SetNewPositionAsyncInternal());
            }
            finally
            {
                SetNewPositionAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> ResetStateAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(ResetStateAsyncInternal());
            }
            finally
            {
                ResetStateAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> TranslatePlatformSpeedToTransformAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(TranslatePlatformSpeedToTransformAsyncInternal());
            }
            finally
            {
                TranslatePlatformSpeedToTransformAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> DisableGravityAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(DisableGravityAsyncInternal());
            }
            finally
            {
                DisableGravityAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> ApplyPlatformSpeedToNewPositionAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(ApplyPlatformSpeedToNewPositionAsyncInternal());
            }
            finally
            {
                ApplyPlatformSpeedToNewPositionAsyncInternal().Forget();
            }
        }

        public UniTask<UniTaskVoid> StopSpeedAsync()
        {
            try
            {
                return new UniTask<UniTaskVoid>(StopSpeedAsyncInternal());
            }
            finally
            {
                StopSpeedAsyncInternal().Forget();
            }
        }
    }
}

/*

//private async UniTaskVoid OnSetMovementDirectionAsyncInternal(bool onMovingPlatform, Vector3 platformSpeed)
//  {
/*
const float movementDirectionThreshold = 0.0001f;
movementDirection = storedMovementDirection;
if (speed.x < -movementDirectionThreshold || externalForce.x < -movementDirectionThreshold)
    movementDirection = -1;
else if (speed.x > movementDirectionThreshold || externalForce.x > movementDirectionThreshold)
    movementDirection = 1;
if (onMovingPlatform && Abs(platformSpeed.x) > Abs(speed.x)) movementDirection = Sign(platformSpeed.x);
storedMovementDirection = movementDirection;
await Yield();
*/
//  }

//    private async UniTaskVoid OnRaycastHorizontalAsyncInternal(float raycastDirection, float distanceToWall,
//        float boundsWidth, float raycastOffset, bool isGrounded)
//   {
/*
if (Abs(movementDirection - raycastDirection) < Tolerance)
{
    newPosition.x = raycastDirection <= 0
        ? -distanceToWall + boundsWidth / 2 + raycastOffset * 2
        : distanceToWall - boundsWidth / 2 - raycastOffset * 2;
    if (!isGrounded && speed.y != 0) newPosition.x = 0; // prevent pushback if airborne.
    speed.x = 0;
}

await Yield();
*/
//   }

//  private async UniTaskVoid OnRaycastBelowAsyncInternal()
//  {
//state.IsFalling = newPosition.y < -SmallValue;
//await Yield();
//   }

//    private async UniTaskVoid OnRaycastBelowHitAsyncInternal(bool raycastHit, float distanceToGround,
//        float boundsHeight, float raycastOffset, bool wasGroundedLastFrame)
//    {
/*
if (raycastHit)
{
    //state.IsFalling = false;
    newPosition.y = externalForce.y > 0 && speed.y > 0
        ? speed.y * deltaTime
        : -distanceToGround + boundsHeight / 2 * raycastOffset;
    if (!wasGroundedLastFrame && speed.y > 0) newPosition.y += speed.y * deltaTime;
    if (Abs(newPosition.y) < SmallValue) newPosition.y = 0;
}

await Yield();
*/
//   }
/*    private async UniTaskVoid OnMovingPlatformTestAsyncInternal(bool raycastHit,
        PathMovementController movingPlatformTest, PathMovementController movingPlatform, bool isGrounded,
        bool onMovingPlatform)
    {
        if (raycastHit && !movingPlatformTest && !isGrounded) DetachFromMovingPlatform(movingPlatform);
        else if (onMovingPlatform) DetachFromMovingPlatform(movingPlatform);
        await Yield();
    }

    private async UniTaskVoid OnStickToSlopeAsyncInternal(float belowSlopeAngleLeft, float belowSlopeAngleRight,
        RaycastHit2D stickyRaycast, Object ignoredCollider, float raycastOriginY, float boundsHeight)
    {
        /*
        if (stickToSlopeControl && belowSlopeAngleLeft > 0 && belowSlopeAngleRight < 0f && stickyRaycast &&
            stickyRaycast.collider != ignoredCollider) SetNewPositionY(stickyRaycast, raycastOriginY, boundsHeight);
        await Yield();
        */
/*   }

   private async UniTaskVoid OnStickyRaycastAsyncInternal(RaycastHit2D stickyRaycast, Object ignoredCollider,
       float raycastOriginY, float boundsHeight)
   {
       if (stickyRaycast && stickyRaycast.collider != ignoredCollider)
           SetNewPositionY(stickyRaycast, raycastOriginY, boundsHeight);
       await Yield();
   }

   private async UniTaskVoid OnRaycastAboveHitAsyncInternal(bool raycastHit, float smallestDistance,
       float boundsHeight, bool isGrounded, bool hitCeilingLastFrame)
   {
       /*
       if (raycastHit)
       {
           newPosition.y = smallestDistance - boundsHeight / 2;
           if (isGrounded && newPosition.y < 0) newPosition.y = 0;
           if (!hitCeilingLastFrame) speed = new Vector2(speed.x, 0f);
           speed.y = 0; // apply vertical force
       }

       await Yield();
       */
/*        }

        private async UniTaskVoid OnMoveTransformAsyncInternal(RaycastHit2D safetyBoxcast)
        {
            /*
            if (safetyBoxcast && Abs(safetyBoxcast.distance - newPosition.magnitude) != 0) newPosition = Vector2.zero;
            else p.transform.Translate(newPosition, Self);
            await Yield();
            */
/*      }

      private async UniTaskVoid OnSetNewSpeedAsyncInternal(bool isGrounded, float belowSlopeAngle,
          bool onMovingPlatform)
      {
          /*
          if (deltaTime > 0) speed = newPosition / deltaTime;
          if (isGrounded) speed.x *= slopeAngleSpeedFactor.Evaluate(Abs(belowSlopeAngle) * Sign(speed.y));
          if (!onMovingPlatform)
          {
              speed.x = Clamp(speed.x, -maximumVelocity.x, maximumVelocity.x);
              speed.y = Clamp(speed.y, -maximumVelocity.y, maximumVelocity.y);
          }

          await Yield();
          */
/*    }

    private async UniTaskVoid OnRaycastColliderHitAsyncInternal(IEnumerable<RaycastHit2D> contactList)
    {
        /*
        if (physics2DInteractionControl)
            foreach (var hit in contactList)
            {
                var body = hit.collider.attachedRigidbody;
                if (!body || body.isKinematic || body.bodyType == Static) continue;
                var pushDirection = new Vector3(externalForce.x, 0, 0);
                body.velocity = pushDirection.normalized * physics2DPushForce;
            }

        await Yield();
        */
/*  }

  private async UniTaskVoid OnSetExternalForceAsyncInternal()
  {
      /*
      externalForce.x = 0;
      externalForce.y = 0;
      await Yield();
      */
/*}

private async UniTaskVoid OnSetWorldSpeedAsyncInternal()
{
    /*
    worldSpeed = speed;
    await Yield();
    */
/*}

private void DetachFromMovingPlatform(PathMovementController movingPlatform)
{
    if (!movingPlatform) return;
    //state.GravityActive = true;
}

private void SetNewPositionY(RaycastHit2D stickyRaycast, float raycastOriginY, float boundsHeight)
{
    /*
    newPosition.y = -Abs(stickyRaycast.point.y - raycastOriginY) + boundsHeight / 2;
    */
//}
//}
//}