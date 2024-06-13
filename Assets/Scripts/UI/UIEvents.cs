using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIEvents
{
    public static Action<AudioClip> OnRequestPlayUIAudio;
    public static Action<LitoTransformationType, Sprite, bool> OnTransformationAdd;
    public static Action<LitoTransformationType> OnTransformationObtained;
    public static Action<LitoTransformationType> OnTransformationSwap;
}
