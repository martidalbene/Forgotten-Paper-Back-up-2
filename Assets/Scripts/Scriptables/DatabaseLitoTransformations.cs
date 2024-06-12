using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LitoTransformationType
{
    Lito, Avionlito, Barlito
}

[CreateAssetMenu(fileName = "NewLitoTransformationData", menuName = "Scriptables/Lito/Transformations")]
public class DatabaseLitoTransformations : ScriptableObject
{
    [SerializeField] private LitoTransformationType _type = LitoTransformationType.Lito;

    [Header("Transformation settings")]
    [SerializeField] private float _speed;
    [SerializeField] private float _maxAcceleration;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _gravityScale;

    [Header("Sound settings")]
    [SerializeField] private AudioClip _transformAudioClip;
    [SerializeField] private List<AudioClip> _footstepsAudioClips;

    [Header("UI settings")]
    [SerializeField] private Sprite _lockedIcon;
    [SerializeField] private Sprite _unlockedIcon;

    public LitoTransformationType TransformationType => _type;

    public float TransformationSpeed => _speed;
    public float TransformationMaxAcceleration => _maxAcceleration;
    public float TransformationJumpForce => _jumpForce;
    public float TransformationGravityScale => _gravityScale;

    public AudioClip TransformationAudio => _transformAudioClip;
    public AudioClip TransformationFootstepSound => _footstepsAudioClips[Random.Range(0, _footstepsAudioClips.Count)];

    public Sprite TransformationLockedIcon => _lockedIcon;
    public Sprite TransformationUnlockedIcon => _unlockedIcon;

}
