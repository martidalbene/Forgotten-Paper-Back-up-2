using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILitoTransformation
{
    public LitoTransformationType TransformationType { get; }
    public float TransformationSpeed { get; }
    public float TransformationJumpForce { get; }
    public AudioClip TransformationFootstepSound { get; }

    void ApplyTransformation();
    void UpdateTransformation(float delta);
}
