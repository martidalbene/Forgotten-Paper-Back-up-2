using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerEvents
{
    private static LitoTransformationType _currentPlayerTransformation;
    private static bool _npcNextDialogue;   

    public static Action<ItemType> OnItemGrab;
    public static Action OnEnemyDamage;
    public static Action OnWaterTouch;
    public static Action<bool> OnForceOutOfWater;
    public static Action<bool> OnEnableDisableControls;

    public static LitoTransformationType PlayerCurrentTransformation => _currentPlayerTransformation;
    public static bool NPCNextDialogue => _npcNextDialogue;

    public static void UpdatePlayerTransformation(LitoTransformationType newTransform)
    {
        _currentPlayerTransformation = newTransform;
    }

    public static void OnNPCNextDialogue(bool inputNextNPCDialogue)
    {
        _npcNextDialogue = inputNextNPCDialogue;
    }
}
