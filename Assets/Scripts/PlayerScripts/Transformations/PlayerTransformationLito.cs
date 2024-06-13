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

        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
        _isLookingRight = isLookingRight;

        RefRigidBody.AddForce(Vector2.up * TransformationJumpForce, ForceMode2D.Impulse);
        print("==?=");
    }

    public override void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        if (_currentTimeBeforeStart > 0) _currentTimeBeforeStart -= delta;

        _hAxis = hAxis;
    }

    public override void FixedUpdateTransformation()
    {
        if (_currentTimeBeforeStart <= 0)
            RefRigidBody.velocity = new Vector2(_hAxis * TransformationSpeed, RefRigidBody.velocity.y);
    }
}
