using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerTransformation : MonoBehaviour, ILitoTransformation
{
    [SerializeField] private DatabaseLitoTransformations _transformationData;
    private Rigidbody2D _rBody;
    private AudioSource _audioSource;

    public float _hAxis;
    public bool _isOnFloor;
    public bool _isLookingRight;
    public bool _isOnWater;
    public bool _forceOutOfWater;

    public Rigidbody2D RefRigidBody => _rBody;
    public AudioSource RefAudioSource => _audioSource;

    public LitoTransformationType TransformationType => _transformationData.TransformationType;
    public float TransformationSpeed => _transformationData.TransformationSpeed;
    public float TransformationMaxAcceleration => _transformationData.TransformationMaxAcceleration;
    public float TransformationJumpForce => _transformationData.TransformationJumpForce;
    public float TransformationGravityScale => _transformationData.TransformationGravityScale;
    public bool TransformationCanStandOnWater => _transformationData.TransformationCanStandOnWater;
    public float TransformationWaterAcceleration => _transformationData.TransformationOnWaterAcceleration;
    public AudioClip TransformationSound => _transformationData.TransformationAudio;
    public AudioClip TransformationJumpSound => _transformationData.TransformationJumpAudio;
    public AudioClip TransformationFootstepSound => _transformationData.TransformationFootstepSound;
    public Sprite TransformationSprite => _transformationData.TransformationIcon;

    public Action OnForceTransformToBase;
    public Action OnPlayerColliderHit;

    public virtual void InitializeTransformation(Rigidbody2D rBody, AudioSource audioSource)
    {
        _rBody = rBody;
        _audioSource = audioSource;
    }
    public abstract void EndTransformation();
    public virtual void ExecTransformation(bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        _isOnWater = isOnWater;
        _isOnFloor = isOnFloor;
        _isLookingRight = isLookingRight;
    }
    public abstract void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight, bool forceOutOfWater);
    public abstract void FixedUpdateTransformation();

    public virtual void PlayerColliderHit()
    {

    }
}
