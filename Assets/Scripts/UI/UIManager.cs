using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HorizontalLayoutGroup _transformationsPanel;
    [SerializeField] private GameObject _transformationSelector;

    [Header("Objects")]
    [SerializeField] private GameObject _transformationItemObject;

    [Header("Settings")]
    [SerializeField] private AudioClip _transformationSwapAudioClip;

    // Values
    private Dictionary<LitoTransformationType, Image> _transformationsList = new Dictionary<LitoTransformationType, Image>();
    private AudioSource _uiSounds;

    // Events
    private void Awake()
    {
        SubscribeToEvents();
    }

    private void Start()
    {
        _uiSounds = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void SubscribeToEvents()
    {
        UIEvents.OnRequestPlayUIAudio += OnUISound;
        UIEvents.OnTransformationAdd += OnTransformationAdd;
        UIEvents.OnTransformationObtained += OnTransformationAvailable;
        UIEvents.OnTransformationSwap += OnTransformationSwap;
    }

    void UnsubscribeFromEvents()
    {
        UIEvents.OnRequestPlayUIAudio -= OnUISound;
        UIEvents.OnTransformationAdd -= OnTransformationAdd;
        UIEvents.OnTransformationObtained -= OnTransformationAvailable;
        UIEvents.OnTransformationSwap -= OnTransformationSwap;
    }

    private void OnUISound(AudioClip clip)
    {
        _uiSounds.PlayOneShot(clip);
    }

    private void OnTransformationAdd(LitoTransformationType transformationType, Sprite transformationSprite, bool isOwned)
    {
        if (_transformationsList.ContainsKey(transformationType)) return;

        GameObject newTransformation = Instantiate(_transformationItemObject);
        newTransformation.transform.SetParent(_transformationsPanel.gameObject.transform, false);
        
        Image newTransfImage = newTransformation.GetComponent<Image>();
        newTransfImage.sprite = transformationSprite;

        if (!isOwned) newTransfImage.color = Color.black;

        _transformationsList.Add(transformationType, newTransfImage);
    }

    private void OnTransformationAvailable(LitoTransformationType availableTransform)
    {
        if (!_transformationsList.ContainsKey(availableTransform)) return;

        _transformationsList.TryGetValue(availableTransform, out Image transfImage);
        transfImage.color = Color.white;
    }

    private void OnTransformationSwap(LitoTransformationType selectedTransformation)
    {
        if (!_transformationsList.ContainsKey(selectedTransformation)) return;

        _uiSounds.PlayOneShot(_transformationSwapAudioClip);
        _transformationsList.TryGetValue(selectedTransformation, out Image transfImage);
        _transformationSelector.transform.position = transfImage.gameObject.transform.position;
        _transformationSelector.transform.localScale = transfImage.gameObject.transform.localScale;
    }
}
