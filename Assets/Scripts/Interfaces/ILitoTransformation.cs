using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILitoTransformation
{
    public LitoTransformationType TransformationType { get; }

    public float TransformationSpeed { get; }
    public float TransformationJumpForce { get; }
    public float TransformationGravityScale { get; }
    public bool TransformationCanStandOnWater { get; }

    public AudioClip TransformationSound { get; }
    public AudioClip TransformationJumpSound { get; }
    public AudioClip TransformationFootstepSound { get; }

    void InitializeTransformation(Rigidbody2D rBody, AudioSource audioSource);
    void EndTransformation();
    void ExecTransformation(bool isOnWater, bool isFalling, bool isLookingRight);
    void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isFalling, bool isLookingRight, bool forceOutOfWater);
    void FixedUpdateTransformation();

    void PlayerColliderHit();
}
