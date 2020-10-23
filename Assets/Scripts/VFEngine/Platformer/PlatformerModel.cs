﻿using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using VFEngine.Platformer.Event.Raycast;
using VFEngine.Tools;
using UniTaskExtensions = VFEngine.Tools.UniTaskExtensions;

// ReSharper disable UnusedVariable
namespace VFEngine.Platformer
{
    using static UniTaskExtensions;
    using static DebugExtensions;
    using static PhysicsExtensions;
    using static TimeExtensions;
    using static Mathf;
    using static RaycastDirection;
    using static LayerMaskExtensions;

    [CreateAssetMenu(fileName = "PlatformerModel", menuName = "VFEngine/Platformer/Platformer Model", order = 0)]
    public class PlatformerModel : ScriptableObject, IModel
    {
        /* fields: dependencies */
        [LabelText("Platformer Data")] [SerializeField]
        private PlatformerData p;

        /* fields: methods */
        private async UniTaskVoid Initialize()
        {
            GetWarningMessages();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private void GetWarningMessages()
        {
            if (!p.DisplayWarnings) return;
            const string pl = "Platformer";
            const string rc = "Raycast";
            const string ctr = "Controller";
            const string ch = "Character";
            var settings = $"{pl} Settings";
            var warningMessage = "";
            var warningMessageCount = 0;
            if (!p.HasSettings) warningMessage += FieldString($"{settings}", $"{settings}");
            if (!p.Physics) warningMessage += FieldParentGameObjectString($"Physics {ctr}", $"{ch}");
            if (!p.Raycast) warningMessage += FieldParentGameObjectString($"{rc} {ctr}", $"{ch}");
            if (!p.RaycastHitCollider)
                warningMessage += FieldParentGameObjectString($"{rc} Hit Collider {ctr}", $"{ch}");
            if (!p.LayerMask) warningMessage += FieldParentGameObjectString($"Layer Mask {ctr}", $"{ch}");
            DebugLogWarning(warningMessageCount, warningMessage);

            string FieldString(string field, string scriptableObject)
            {
                AddWarningMessage();
                return FieldMessage(field, scriptableObject);
            }

            string FieldParentGameObjectString(string field, string gameObject)
            {
                AddWarningMessage();
                return FieldParentGameObjectMessage(field, gameObject);
            }

            void AddWarningMessage()
            {
                warningMessageCount++;
            }
        }

        private async UniTaskVoid RunPlatformer()
        {
            var pTask1 = Async(ApplyGravity());
            var pTask2 = Async(InitializeFrame());
            var task1 = await (pTask1, pTask2);
            await Async(TestMovingPlatform());
            await Async(SetHorizontalMovementDirection());
            await Async(StartRaycasts());
            // MoveTransform
            // Set Rays Params
            // Set New Speed
            // Set States
            // Set Distance To Ground
            // Reset External Force
            // On FrameExit
            // Update World Speed
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid ApplyGravity()
        {
            p.Physics.SetCurrentGravity();
            if (p.Speed.y > 0) p.Physics.ApplyAscentMultiplierToCurrentGravity();
            if (p.Speed.y < 0) p.Physics.ApplyFallMultiplierToCurrentGravity();
            if (p.GravityActive) p.Physics.ApplyGravityToVerticalSpeed();
            if (p.FallSlowFactor != 0) p.Physics.ApplyFallSlowFactorToVerticalSpeed();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid InitializeFrame()
        {
            var phTask1 = Async(p.Physics.SetNewPosition());
            var phTask2 = Async(p.Physics.ResetState());
            var rhcTask1 = Async(p.RaycastHitCollider.ClearContactList());
            var rhcTask2 = Async(p.RaycastHitCollider.SetWasGroundedLastFrame());
            var rhcTask3 = Async(p.RaycastHitCollider.SetStandingOnLastFrame());
            var rhcTask4 = Async(p.RaycastHitCollider.SetWasTouchingCeilingLastFrame());
            var rhcTask5 = Async(p.RaycastHitCollider.SetCurrentWallColliderNull());
            var rhcTask6 = Async(p.RaycastHitCollider.ResetState());
            var rTask1 = Async(p.Raycast.SetRaysParameters());
            var pTask = await (phTask1, phTask2, rhcTask1, rhcTask2, rhcTask3, rhcTask4, rhcTask5, rhcTask6, rTask1);
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid TestMovingPlatform()
        {
            if (p.IsCollidingWithMovingPlatform)
            {
                if (!SpeedNan(p.MovingPlatformCurrentSpeed)) p.Physics.TranslatePlatformSpeedToTransform();
                var platformTest = MovingPlatformTest(TimeLteZero(), AxisSpeedNan(p.MovingPlatformCurrentSpeed),
                    p.WasTouchingCeilingLastFrame);
                if (platformTest)
                {
                    var rchTask1 = Async(p.RaycastHitCollider.SetOnMovingPlatform());
                    var rchTask2 = Async(p.RaycastHitCollider.SetMovingPlatformCurrentGravity());
                    var phTask1 = Async(p.Physics.DisableGravity());
                    var phTask2 = Async(p.Physics.ApplyMovingPlatformSpeedToNewPosition());
                    var rTask1 = Async(p.Raycast.SetRaysParameters());
                    var pTask = await (rchTask1, rchTask2, phTask1, phTask2, rTask1);
                    p.Physics.StopHorizontalSpeedOnPlatformTest();
                    p.Physics.SetForcesApplied();
                }
            }

            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private static bool MovingPlatformTest(bool timeLteZero, bool axisSpeedNan, bool wasTouchingCeilingLastFrame)
        {
            return !timeLteZero && !axisSpeedNan && !wasTouchingCeilingLastFrame;
        }

        private async UniTaskVoid SetHorizontalMovementDirection()
        {
            p.Physics.SetHorizontalMovementDirectionToStored();
            if (p.Speed.x < -p.MovementDirectionThreshold || p.ExternalForce.x < -p.MovementDirectionThreshold)
                p.Physics.SetNegativeHorizontalMovementDirection();
            else if (p.Speed.x > p.MovementDirectionThreshold || p.ExternalForce.x > p.MovementDirectionThreshold)
                p.Physics.SetPositiveHorizontalMovementDirection();
            if (p.IsCollidingWithMovingPlatform && Abs(p.MovingPlatformCurrentSpeed.x) > Abs(p.Speed.x))
                p.Physics.ApplyPlatformSpeedToHorizontalMovementDirection();
            p.Physics.SetStoredHorizontalMovementDirection();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid StartRaycasts()
        {
            var rTask1 = Async(CastRaysUp());
            var rTask2 = Async(CastHorizontalRays(Right));
            var rTask3 = Async(CastRaysDown());
            var rTask4 = Async(CastHorizontalRays(Left));
            if (p.CastRaysOnBothSides)
            {
                var task1 = await (rTask1, rTask2, rTask3, rTask4);
            }
            else if (p.HorizontalMovementDirection == 1)
            {
                var task2 = await (rTask1, rTask2, rTask3);
            }
            else
            {
                var task3 = await (rTask1, rTask3, rTask4);
            }

            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid CastRaysUp()
        {
            // foo
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid CastRaysDown()
        {
            var rhcTask1 = Async(p.RaycastHitCollider.SetIsNotCollidingBelow());
            var phTask1 = Async(DetachFromMovingPlatform());
            SetIsFalling(p.SmallValue, p.NewPosition.y);
            if (!(p.Gravity > 0) || p.IsFalling)
            {
                var rhcTask2 = Async(p.RaycastHitCollider.InitializeFriction());
                var rhcTask3 = Async(InitializeDownHitsStorage());
                var rTask1 = Async(p.Raycast.InitializeDownRayLength());
                var rTask2 = Async(SetVerticalRaycast());
                var lmTask1 = Async(SetRaysBelowLayerMask());
                var task1 = await (rhcTask2, rhcTask3, rTask1, rTask2, lmTask1);
                ApplyToDownRayLength(p.OnMovingPlatform, p.NewPosition.y);
                if (p.WasGroundedLastFrame && p.IsStandingOnLastFrameNotNull)
                {
                    bool midHeightOneWayPlatformMaskContains;
                    bool stairsMaskContains;
                    bool standingOnColliderBoundsContains;
                    bool newPositionYGtZero;
                    var pTask1 = Async(SetMidHeightOneWayPlatformMaskContains());
                    var pTask2 = Async(SetStairsMaskContains());
                    var pTask3 = Async(SetStandingOnColliderBoundsContains());
                    var pTask4 = Async(SetNewPositionYGtZero());
                    var task2 = await (pTask1, pTask2, pTask3, pTask4);
                    var pTask5 = Async(ApplyToRaysBelowLayerMask(midHeightOneWayPlatformMaskContains,
                        stairsMaskContains, standingOnColliderBoundsContains, p.OnMovingPlatform, newPositionYGtZero));
                    var rTask3 = Async(p.Raycast.InitializeSmallestDistance());
                    var rhcTask4 = Async(p.RaycastHitCollider.InitializeDownHitsStorageIndex());
                    var rhcTask5 = Async(p.RaycastHitCollider.InitializeDownHitsStorageSmallestDistanceIndex());
                    var rhcTask6 = Async(p.RaycastHitCollider.InitializeDownHitConnected());
                    var task3 = await (pTask5, rTask3, rhcTask4, rhcTask5, rhcTask6);

                    async UniTaskVoid SetMidHeightOneWayPlatformMaskContains()
                    {
                        midHeightOneWayPlatformMaskContains = LayerMaskContains(p.MidHeightOneWayPlatformMask,
                            p.StandingOnLastFrame.layer);
                        await SetYieldOrSwitchToThreadPoolAsync();
                    }

                    async UniTaskVoid SetStairsMaskContains()
                    {
                        stairsMaskContains = LayerMaskContains(p.StairsMask, p.StandingOnLastFrame.layer);
                        await SetYieldOrSwitchToThreadPoolAsync();
                    }

                    async UniTaskVoid SetStandingOnColliderBoundsContains()
                    {
                        standingOnColliderBoundsContains =
                            p.StandingOnCollider.bounds.Contains(p.ColliderBottomCenterPosition);
                        await SetYieldOrSwitchToThreadPoolAsync();
                    }

                    async UniTaskVoid SetNewPositionYGtZero()
                    {
                        newPositionYGtZero = p.NewPosition.y > 0;
                        await SetYieldOrSwitchToThreadPoolAsync();
                    }

                    for (var i = 0; i < p.NumberOfVerticalRaysPerSide; i++)
                    {
                        if (p.NewPosition.y > 0 && !p.WasGroundedLastFrame)
                            p.Raycast.SetCurrentDownRaycastToIgnoreOneWayPlatform();
                        else p.Raycast.SetCurrentDownRaycast();
                        var rhcTask7 = Async(p.RaycastHitCollider.SetCurrentDownHitsStorage());
                        var rhcTask8 = Async(p.RaycastHitCollider.SetRaycastDownHitAt());
                        var task4 = await (rhcTask7, rhcTask8);
                        if (p.RaycastDownHitAt)
                        {
                            if (p.RaycastDownHitAt.collider == p.IgnoredCollider) continue;
                            var rhcTask9 = Async(p.RaycastHitCollider.SetDownHitConnected());
                            var rhcTask10 = Async(p.RaycastHitCollider.SetBelowSlopeAngleAt());
                            var rhcTask11 = Async(p.RaycastHitCollider.SetCrossBelowSlopeAngleAt());
                            var task5 = await (rhcTask9, rhcTask10, rhcTask11);
                            if (p.CrossBelowSlopeAngle.z < 0) p.RaycastHitCollider.SetNegativeBelowSlopeAngle();
                            if (p.RaycastDownHitAt.distance < p.SmallestDistance)
                            {
                                var rhcTask12 = Async(p.RaycastHitCollider.SetSmallestDistanceIndexAt());
                                var rhcTask13 = Async(p.RaycastHitCollider.SetDownHitWithSmallestDistance());
                                var rTask4 = Async(p.Raycast.SetSmallestDistanceToDownHitDistance());
                                var task6 = await (rhcTask12, rhcTask13, rTask4);
                            }
                        }

                        if (p.CurrentDownHitSmallestDistance < p.SmallValue) break;
                        p.RaycastHitCollider.AddDownHitsStorageIndex();
                    }
                    
                    if (p.DownHitConnected)
                    {
                        var notHighEnoughForOneWayPlatform = SetNotHighEnoughForOneWayPlatform(p.WasGroundedLastFrame,
                            p.SmallestDistance, p.BoundsHeight, p.OneWayPlatformMask,
                            p.StandingOnWithSmallestDistanceLayer, p.MovingOneWayPlatformMask);
                        if (notHighEnoughForOneWayPlatform)
                        {
                            await rhcTask1;
                            return;
                        }

                        var phTask2 = Async(p.Physics.SetIsNotFalling());
                        var rhcTask14 = Async(p.RaycastHitCollider.SetIsCollidingBelow());
                        var task7 = await (phTask2, rhcTask14);
                        var applyingExternalForce = SetApplyingExternalForce(p.ExternalForce.y, p.Speed.y);
                        if (applyingExternalForce)
                        {
                            var phTask3 = Async(p.Physics.ApplySpeedToHorizontalNewPosition());
                            var task8 = await (phTask3, rhcTask14);
                        }
                        else
                        {
                            p.Raycast.SetDistanceBetweenVerticalRaycastsAndSmallestDistanceDownRaycastPoint();
                            p.Physics.ApplyHalfBoundsHeightAndRayOffsetToNegativeVerticalNewPosition();
                        }

                        if (!p.WasGroundedLastFrame && p.Speed.y > 0) p.Physics.ApplySpeedToVerticalNewPosition();
                        if (Abs(p.NewPosition.y) < p.SmallValue) p.Physics.StopNewVerticalPosition();
                        if (p.HasPhysicsMaterialDataClosestToDownHit)
                            p.RaycastHitCollider.SetFrictionToDownHitWithSmallestDistancesFriction();
                        if (p.HasPathMovementControllerClosestToDownHit && p.IsGrounded)
                        {
                            var rhcTask15 = Async(p.RaycastHitCollider
                                .SetMovingPlatformToDownHitWithSmallestDistancesPathMovement());
                            var rhcTask16 = Async(p.RaycastHitCollider.SetHasMovingPlatform());
                            var task9 = await (rhcTask15, rhcTask16);
                        }
                        else
                        {
                            await phTask1;
                        }
                    }
                    else
                    {
                        var task10 = await (rhcTask1, phTask1);
                    }

                    if (p.StickToSlopesControl) StickToSlope();
                }
            }
            else
            {
                await rhcTask1;
            }

            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private void StickToSlope()
        {
            var stickToSlope = SetStickToSlope(p.NewPosition.y, p.StickToSlopesOffsetY, p.IsJumping,
                p.StickToSlopesControl, p.WasGroundedLastFrame, p.ExternalForce.y, p.HasMovingPlatform,
                p.IsStandingOnLastFrameNotNull, p.StairsMask, p.StandingOnLastFrame.layer);
            if (!stickToSlope) return;
            if (p.StickyRaycastLength == 0) p.StickyRaycast.SetStickyRaycastLength();
            //============================================================================================================ 
        }

        private static bool SetStickToSlope(float positionY, float offsetY, bool isJumping, bool stickToSlopes,
            bool wasGroundedLastFrame, float forceY, bool hasPlatform, bool standingOnLastFrameNotNull,
            LayerMask stairs, LayerMask standingOnLastFrame)
        {
            return !(positionY >= offsetY) && !(positionY <= -offsetY) && !isJumping && stickToSlopes &&
                wasGroundedLastFrame && !(forceY > 0) && !hasPlatform || !wasGroundedLastFrame &&
                standingOnLastFrameNotNull && LayerMaskContains(stairs, standingOnLastFrame) && !isJumping;
        }

        private async UniTaskVoid DetachFromMovingPlatform()
        {
            if (p.HasMovingPlatform)
            {
                var phTask = Async(p.Physics.SetGravityActive());
                var rhcTask1 = Async(p.RaycastHitCollider.SetOnMovingPlatform());
                var rhcTask2 = Async(p.RaycastHitCollider.SetMovingPlatformToNull());
                var rhcTask3 = Async(p.RaycastHitCollider.SetDoesNotHaveMovingPlatform());
                var rhcTask4 = Async(p.RaycastHitCollider.StopMovingPlatformCurrentGravity());
                var task = await (phTask, rhcTask1, rhcTask2, rhcTask3, rhcTask4);
            }

            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private static bool SetApplyingExternalForce(float forceY, float speedY)
        {
            return forceY > 0 && speedY > 0;
        }

        private static bool SetNotHighEnoughForOneWayPlatform(bool wasGroundedLastFrame, float smallestDistance,
            float boundsHeight, LayerMask oneWayPlatform, LayerMask standingOn, LayerMask movingOneWayPlatform)
        {
            return !wasGroundedLastFrame && smallestDistance < boundsHeight / 2 &&
                LayerMaskContains(oneWayPlatform, standingOn) || LayerMaskContains(movingOneWayPlatform, standingOn);
        }

        private async UniTaskVoid ApplyToRaysBelowLayerMask(bool midHeightOneWayPlatformMaskContains,
            bool stairsMaskContains, bool standingOnColliderBoundsContains, bool onMovingPlatform,
            bool newPositionYGtZero)
        {
            if (midHeightOneWayPlatformMaskContains)
                p.LayerMask.SetRaysBelowLayerMaskPlatformsToPlatformsWithoutHeight();
            if (stairsMaskContains && standingOnColliderBoundsContains)
                p.LayerMask.SetRaysBelowLayerMaskPlatformsToOneWayOrStairs();
            if (onMovingPlatform && newPositionYGtZero) p.LayerMask.SetRaysBelowLayerMaskPlatformsToOneWay();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private void ApplyToDownRayLength(bool onMovingPlatform, float y)
        {
            if (onMovingPlatform) p.Raycast.DoubleDownRayLength();
            if (y < 0) p.Raycast.SetDownRayLengthToVerticalNewPosition();
        }

        private void SetIsFalling(float value, float y)
        {
            if (y < value) p.Physics.SetIsFalling();
            else Async(p.Physics.SetIsNotFalling());
        }

        private async UniTaskVoid InitializeDownHitsStorage()
        {
            if (p.DownHitsStorageLength != p.NumberOfVerticalRaysPerSide)
                p.RaycastHitCollider.InitializeDownHitsStorage();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetRaysBelowLayerMask()
        {
            var lmTask1 = Async(p.LayerMask.SetRaysBelowLayerMaskPlatforms());
            var lmTask2 = Async(p.LayerMask.SetRaysBelowLayerMaskPlatformsWithoutOneWay());
            var task1 = await (lmTask1, lmTask2);
            p.LayerMask.SetRaysBelowLayerMaskPlatformsWithoutMidHeight();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetVerticalRaycast()
        {
            var rTask1 = Async(p.Raycast.SetVerticalRaycastFromLeft());
            var rTask2 = Async(p.Raycast.SetVerticalRaycastToRight());
            var task1 = await (rTask1, rTask2);
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid CastHorizontalRays(RaycastDirection direction)
        {
            var raysAmount = p.NumberOfHorizontalRaysPerSide;
            InitializeHorizontalHitsStorage(direction, p.HorizontalHitsStorageLength, raysAmount);
            for (var i = 0; i < raysAmount; i++)
            {
                if (p.WasGroundedLastFrame && i == 0) SetCurrentRaycastToIgnoreOneWayPlatform(direction);
                else SetCurrentRaycast(direction);
                SetCurrentSideHitsStorage(direction);
                var hitsDistance = SetHitsDistance(direction);
                if (hitsDistance > 0)
                {
                    var hitCollider = SetHitCollider(direction);
                    if (hitCollider == p.IgnoredCollider) break;
                    var rayDirection = SetRayDirection(direction);
                    if (p.HorizontalMovementDirection == rayDirection) SetHitAngle(direction);
                    var hitAngle = SetHitAngleInternal(direction);
                    if (hitAngle > p.MaximumSlopeAngle)
                    {
                        if (direction == Left)
                        {
                            var rchTask1 = Async(SetLeftIsCollidingLeft());
                            var rchTask2 = Async(SetLeftDistanceToLeftCollider());
                            var task1 = await (rchTask1, rchTask2);
                        }
                        else
                        {
                            var rchTask2 = Async(SetRightIsCollidingRight());
                            var rchTask3 = Async(SetRightDistanceToRightCollider());
                            var task2 = await (rchTask2, rchTask3);
                        }

                        if (p.HorizontalMovementDirection == rayDirection)
                        {
                            var rchTask4 = Async(SetCurrentWallCollider(direction));
                            var rchTask5 = Async(SetFailedSlopeAngle(direction));
                            var rchTask6 = Async(AddHitToContactList(direction));
                            var phTask1 = Async(SetNewHorizontalPosition(direction));
                            var phTask2 = Async(StopHorizontalSpeed());
                            var task3 = await (rchTask4, rchTask5, rchTask6, phTask1, phTask2);
                            if (!p.IsGrounded && p.Speed.y != 0) p.Physics.StopNewHorizontalPosition();
                        }

                        break;
                    }
                }

                AddHorizontalHitsStorageIndex(direction);
            }

            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid StopHorizontalSpeed()
        {
            p.Physics.StopHorizontalSpeed();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid AddHitToContactList(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.AddRightHitToContactList();
            else p.RaycastHitCollider.AddLeftHitToContactList();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetNewHorizontalPosition(RaycastDirection direction)
        {
            if (direction == Right) p.Physics.SetNewPositiveHorizontalPosition();
            else p.Physics.SetNewNegativeHorizontalPosition();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetFailedSlopeAngle(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.SetRightFailedSlopeAngle();
            else p.RaycastHitCollider.SetLeftFailedSlopeAngle();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetCurrentWallCollider(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.SetRightCurrentWallCollider();
            else p.RaycastHitCollider.SetLeftCurrentWallCollider();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetRightIsCollidingRight()
        {
            p.RaycastHitCollider.SetRightIsCollidingRight();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetRightDistanceToRightCollider()
        {
            p.RaycastHitCollider.SetRightDistanceToRightCollider();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetLeftIsCollidingLeft()
        {
            p.RaycastHitCollider.SetLeftIsCollidingLeft();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private async UniTaskVoid SetLeftDistanceToLeftCollider()
        {
            p.RaycastHitCollider.SetLeftDistanceToLeftCollider();
            await SetYieldOrSwitchToThreadPoolAsync();
        }

        private float SetHitAngleInternal(RaycastDirection direction)
        {
            return direction == Right ? p.CurrentRightHitAngle : p.CurrentLeftHitAngle;
        }

        private void SetHitAngle(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.SetRightHitAngle();
            else p.RaycastHitCollider.SetLeftHitAngle();
        }

        private static int SetRayDirection(RaycastDirection direction)
        {
            return direction == Right ? 1 : -1;
        }

        private Collider2D SetHitCollider(RaycastDirection direction)
        {
            return direction == Right ? p.CurrentRightHitCollider : p.CurrentLeftHitCollider;
        }

        private float SetHitsDistance(RaycastDirection direction)
        {
            return direction == Right ? p.CurrentRightHitDistance : p.CurrentLeftHitDistance;
        }

        private void SetCurrentSideHitsStorage(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.SetCurrentRightHitsStorage();
            else p.RaycastHitCollider.SetCurrentLeftHitsStorage();
        }

        private void SetCurrentRaycast(RaycastDirection direction)
        {
            if (direction == Right) p.Raycast.SetCurrentRightRaycast();
            else p.Raycast.SetCurrentLeftRaycast();
        }

        private void SetCurrentRaycastToIgnoreOneWayPlatform(RaycastDirection direction)
        {
            if (direction == Right) p.Raycast.SetCurrentRightRaycastToIgnoreOneWayPlatform();
            else p.Raycast.SetCurrentLeftRaycastToIgnoreOneWayPlatform();
        }

        private void InitializeHorizontalHitsStorage(RaycastDirection direction, int length, int rays)
        {
            InitializeHorizontalHitsStorageIndex(direction);
            if (length == rays) return;
            if (direction == Right) p.RaycastHitCollider.InitializeRightHitsStorage();
            else p.RaycastHitCollider.InitializeLeftHitsStorage();
        }

        private void InitializeHorizontalHitsStorageIndex(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.InitializeRightHitsStorageIndex();
            else p.RaycastHitCollider.InitializeLeftHitsStorageIndex();
        }

        private void AddHorizontalHitsStorageIndex(RaycastDirection direction)
        {
            if (direction == Right) p.RaycastHitCollider.AddToRightHitsStorageIndex();
            p.RaycastHitCollider.AddToLeftHitsStorageIndex();
        }

        /* properties: methods */
        public void OnInitialize()
        {
            Async(Initialize());
        }

        public void OnRunPlatformer()
        {
            Async(RunPlatformer());
        }
    }
}