using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerEvents
{
    private static LitoTransformationType _currentPlayerTransformation;

    public static Action<ItemType> OnItemGrab;
    public static Action OnWaterTouch;
    public static Action<bool> OnForceOutOfWater;

    public static LitoTransformationType PlayerCurrentTransformation => _currentPlayerTransformation;

    public static void UpdatePlayerTransformation(LitoTransformationType newTransform)
    {
        _currentPlayerTransformation = newTransform;
    }
}
