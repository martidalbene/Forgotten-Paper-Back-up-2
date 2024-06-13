using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTransformationBarlito : BasePlayerTransformation
{
    [SerializeField] private float _timeBeforeStart = 1;
    private float _currentTimeBeforeStart = 2f;
    private float _waterSpeed;

    public override void ExecTransformation(bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        _currentTimeBeforeStart = _timeBeforeStart;

        _isOnFloor = isOnFloor;
        _isOnWater = isOnWater;
        _isLookingRight = isLookingRight;

        RefRigidBody.AddForce(Vector2.up * TransformationJumpForce, ForceMode2D.Impulse);
    }

    public override void UpdateTransformation(float delta, float hAxis, bool isOnWater, bool isOnFloor, bool isLookingRight)
    {
        if (_currentTimeBeforeStart > 0) _currentTimeBeforeStart -= delta;

        _isOnWater = isOnWater;
        _hAxis = hAxis;
    }

    public override void FixedUpdateTransformation()
    {
        if (_currentTimeBeforeStart <= 0)
        {
            if (!_isOnWater || RefRigidBody.velocity.x == 0) _waterSpeed = 0;

            if (_isOnWater)
            {
                if (_waterSpeed < TransformationWaterAcceleration)
                    _waterSpeed += 0.02f;

                RefRigidBody.velocity = new Vector2(_hAxis * (TransformationSpeed + _waterSpeed), RefRigidBody.velocity.y);
            }

            else
                RefRigidBody.velocity = new Vector2(_hAxis * TransformationSpeed, RefRigidBody.velocity.y);
        }
    }
}
