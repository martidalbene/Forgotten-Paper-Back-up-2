using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformationAvionlito : BasePlayerTransformation
{
    [SerializeField] private AudioClip _bumpAudio;
    [SerializeField] private float _timeBeforeStart = 1;
    private float _currentTimeBeforeStart = 2f;

    public override void InitializeTransformation(Rigidbody2D rBody, AudioSource audioSource)
    {
        base.InitializeTransformation(rBody, audioSource);
        OnPlayerColliderHit += PlayerColliderHit;
    }

    public override void ExecTransformation(bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        _currentTimeBeforeStart = _timeBeforeStart;
        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
        _isLookingRight = isLookingRight;

        if (_isLookingRight) RefRigidBody.AddForce((Vector2.up + -Vector2.right) * TransformationJumpForce, ForceMode2D.Impulse);
        else RefRigidBody.AddForce((Vector2.up + Vector2.right) * TransformationJumpForce, ForceMode2D.Impulse);
    }

    public override void EndTransformation()
    {

    }

    public override void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight, bool forceOutOfWater)
    {
        if (_currentTimeBeforeStart > 0) _currentTimeBeforeStart -= delta;

        _hAxis = hAxis;
        _isLookingRight = isLookingRight;
        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
    }

    public override void FixedUpdateTransformation()
    {
        if (_currentTimeBeforeStart > 0) return;

        if (_isLookingRight) RefRigidBody.velocity = new Vector2(-1 * TransformationSpeed, -1f);
        else RefRigidBody.velocity = new Vector2(1 * TransformationSpeed, -1f);

        if (_isOnFloor) PlayerColliderHit();
    }

    public override void PlayerColliderHit()
    {
        RefAudioSource.PlayOneShot(_bumpAudio);
        RefRigidBody.velocity = Vector3.zero;
        OnForceTransformToBase?.Invoke();
    }
}
