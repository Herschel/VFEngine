﻿using Sirenix.OdinInspector;
using UnityEngine;

namespace VFEngine.Platformer.Physics
{
    [CreateAssetMenu(fileName = "PhysicsSettings", menuName = "VFEngine/Platformer/Physics/Physics Settings", order = 0)]
    [InlineEditor]
    public class PhysicsSettings : ScriptableObject
    {
        [SerializeField] public float gravity = -30f;
        [SerializeField] public float fallMultiplier = 1f;
        [SerializeField] public float ascentMultiplier = 1f;
        [SerializeField] public Vector2 maximumVelocity = new Vector2(100f, 100f);
        [SerializeField] public float speedAccelerationOnGround = 20f;
        [SerializeField] public float speedAccelerationInAir = 5f;
        [SerializeField] public float speedFactor = 1f;
        [SerializeField] [Range(0, 90)] public float maximumSlopeAngle = 30f;

        [SerializeField] public AnimationCurve slopeAngleSpeedFactor =
            new AnimationCurve(new Keyframe(-90f, 1f), new Keyframe(0f, 1f), new Keyframe(90f, 1f));

        [SerializeField] public float physics2DPushForce = 2f;
        [SerializeField] public bool physics2DInteractionControl = true;
        [SerializeField] public bool safeSetTransformControl = true;
        [SerializeField] public bool safetyBoxcastControl = true;
        [SerializeField] public bool stickToSlopeControl = true;
        [SerializeField] public bool automaticGravityControl = true;
        [LabelText("Display Warnings")] [SerializeField] public bool displayWarningsControl = true;
    }
}