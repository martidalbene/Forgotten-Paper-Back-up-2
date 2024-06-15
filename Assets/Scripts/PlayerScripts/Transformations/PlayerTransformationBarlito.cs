using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformationBarlito : BasePlayerTransformation
{
    [SerializeField] private float _timeBeforeStart = 1;
    private float _currentTimeBeforeStart = 2f;
    private float _waterSpeed;

    private float _latesthAxis;

    public override void ExecTransformation(bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        _currentTimeBeforeStart = _timeBeforeStart;

        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
        _isLookingRight = isLookingRight;

        RefRigidBody.velocity = new Vector2(RefRigidBody.velocity.x / 2, RefRigidBody.velocity.y / 2);
    }

    public override void EndTransformation()
    {
        if (_isOnWater) PlayerEvents.OnForceOutOfWater(true);
    }

    public override void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight, bool forceOutOfWater)
    {
        if (_currentTimeBeforeStart > 0) _currentTimeBeforeStart -= delta;

        _isOnWater = isOnWater;
        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
        _isLookingRight = isLookingRight;
        _hAxis = hAxis;
    }

    public override void FixedUpdateTransformation()
    {
        if (_currentTimeBeforeStart > 0) return;

        if (_hAxis != 0) _latesthAxis = _hAxis;

        if (_isOnFloor)
        {
            RefRigidBody.velocity = Vector3.zero;
            OnForceTransformToBase?.Invoke();
            return;
        }

        if ((RefRigidBody.velocity.x <= 1 && RefRigidBody.velocity.x >= -1) || !_isOnWater) _waterSpeed = 0;
        if (_waterSpeed < TransformationWaterAcceleration && _isOnWater)
            _waterSpeed += 0.1f;

        if (_hAxis != 0)
            RefRigidBody.velocity = new Vector2(_latesthAxis * (TransformationSpeed + _waterSpeed), RefRigidBody.velocity.y);

    }
}
