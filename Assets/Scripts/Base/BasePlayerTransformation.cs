using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerTransformation : MonoBehaviour, ILitoTransformation
{
    [SerializeField] private DatabaseLitoTransformations _transformationData;

    public LitoTransformationType TransformationType => _transformationData.TransformationType;
    public float TransformationSpeed => _transformationData.TransformationSpeed;
    public float TransformationJumpForce => _transformationData.TransformationJumpForce;

    public AudioClip TransformationFootstepSound => _transformationData.TransformationFootstepSound;

    public abstract void ApplyTransformation();
    public abstract void UpdateTransformation(float delta);
    
}
