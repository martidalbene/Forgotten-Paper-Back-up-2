using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEvents
{
    public static Action<bool> OnGamePaused;
    public static Action<string> OnUpdatePlayerDeathCounter;
    public static Action<AudioClip> OnRequestPlayUIAudio;
    public static Action<LitoTransformationType, Sprite, bool> OnTransformationAdd;
    public static Action<LitoTransformationType> OnTransformationObtained;
    public static Action<LitoTransformationType> OnTransformationSwap;
    public static Action<string> OnPencilCountUpdate;
    public static Action<string> OnPlayTimeUpdate;

    public static Action OnStartNPCDialogue;
    public static Action<string> OnUpdateNPCDialogue;
    public static Action<bool> OnEnableSkipNPCDialogue;
    public static Action OnEndNPCDialogue;
}
