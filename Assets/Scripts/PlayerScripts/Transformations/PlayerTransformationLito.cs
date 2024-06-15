using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformationLito : BasePlayerTransformation
{
    [SerializeField] private float _timeBeforeStart = 1;
    private float _currentTimeBeforeStart = 2f;

    public override void ExecTransformation(bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        _currentTimeBeforeStart = _timeBeforeStart;
        _isOnWater = isOnWater;        
        _isOnFloor = isOnFloor;
        _isLookingRight = isLookingRight;

        RefRigidBody.mass = 1;
        RefRigidBody.AddForce(Vector2.up * 4, ForceMode2D.Impulse);
    }

    public override void EndTransformation()
    {
        
    }

    public override void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight, bool forceOutOfWater)
    {
        _forceOutOfWater = forceOutOfWater;
        _isLookingRight = isLookingRight;
        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;

        if (_currentTimeBeforeStart > 0) _currentTimeBeforeStart -= delta;

        _hAxis = hAxis;
    }

    public override void FixedUpdateTransformation()
    {
        if (_forceOutOfWater && _isOnWater && RefRigidBody.velocity.y < 2)
            RefRigidBody.AddForce(Vector2.up * TransformationJumpForce, ForceMode2D.Impulse);

        if (_currentTimeBeforeStart > 0) return;
        RefRigidBody.velocity = new Vector2(_hAxis * TransformationSpeed, RefRigidBody.velocity.y);
    }
}
