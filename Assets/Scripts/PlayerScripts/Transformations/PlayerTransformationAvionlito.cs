using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformationAvionlito : BasePlayerTransformation
{
    [SerializeField] private float _timeBeforeStart = 1;
    private float _currentTimeBeforeStart = 2f;

    public override void ExecTransformation(bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        _currentTimeBeforeStart = _timeBeforeStart;
        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
        _isLookingRight = isLookingRight;

        if (_isLookingRight) RefRigidBody.AddForce((Vector2.up + -Vector2.right) * TransformationJumpForce, ForceMode2D.Impulse);
        else RefRigidBody.AddForce((Vector2.up + Vector2.right) * TransformationJumpForce, ForceMode2D.Impulse);
    }

    public override void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        if (_currentTimeBeforeStart > 0) _currentTimeBeforeStart -= delta;

        _hAxis = hAxis;
        _isLookingRight = isLookingRight;
        _isOnFloor = isOnFloor;
    }

    public override void FixedUpdateTransformation()
    {
        if (_currentTimeBeforeStart <= 0)
        {
            if (_isOnFloor || RefRigidBody.velocity.x == 0)
            {
                RefRigidBody.velocity = Vector3.zero;
                OnForceTransformToBase?.Invoke();
            }
            else
            {
                if (_isLookingRight) RefRigidBody.velocity = new Vector2(-1 * TransformationSpeed, -1f);
                else RefRigidBody.velocity = new Vector2(1 * TransformationSpeed, -1f);
            }
        }
    }
}
