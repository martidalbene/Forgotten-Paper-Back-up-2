using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Interactable objects & menues")]
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _chatPanel;
    [SerializeField] private Text _chatText;
    [SerializeField] private Text _chatSkipButton;
    [SerializeField] private HorizontalLayoutGroup _transformationsPanel;
    [SerializeField] private GameObject _transformationSelector;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _pencilCounter;
    [SerializeField] private TextMeshProUGUI _theTime;
    [SerializeField] private TextMeshProUGUI _deathCounter;

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

    private void SubscribeToEvents()
    {
        UIEvents.OnGamePaused += OnGamePaused;
        UIEvents.OnUpdatePlayerDeathCounter += OnPlayerDeathsUpdate;
        UIEvents.OnRequestPlayUIAudio += OnUISound;
        UIEvents.OnTransformationAdd += OnTransformationAdd;
        UIEvents.OnTransformationObtained += OnTransformationAvailable;
        UIEvents.OnTransformationSwap += OnTransformationSwap;
        UIEvents.OnPencilCountUpdate += OnPencilCounterUpdate;
        UIEvents.OnPlayTimeUpdate += OnPlaytimeUpdate;
        UIEvents.OnStartNPCDialogue += NPCChatStart;
        UIEvents.OnUpdateNPCDialogue += NPCChatUpdate;
        UIEvents.OnEnableSkipNPCDialogue += NPCChatOnSkipButton;
        UIEvents.OnEndNPCDialogue += NPCChatEnd;
    }

    private void UnsubscribeFromEvents()
    {
        UIEvents.OnGamePaused -= OnGamePaused;
        UIEvents.OnUpdatePlayerDeathCounter -= OnPlayerDeathsUpdate;
        UIEvents.OnRequestPlayUIAudio -= OnUISound;
        UIEvents.OnTransformationAdd -= OnTransformationAdd;
        UIEvents.OnTransformationObtained -= OnTransformationAvailable;
        UIEvents.OnTransformationSwap -= OnTransformationSwap;
        UIEvents.OnPencilCountUpdate -= OnPencilCounterUpdate;
        UIEvents.OnPlayTimeUpdate -= OnPlaytimeUpdate;
        UIEvents.OnStartNPCDialogue -= NPCChatStart;
        UIEvents.OnUpdateNPCDialogue -= NPCChatUpdate;
        UIEvents.OnEndNPCDialogue -= NPCChatEnd;
        UIEvents.OnEnableSkipNPCDialogue += NPCChatOnSkipButton;
    }

    private void OnGamePaused(bool showPauseMenu)
    {
        _pauseMenu.SetActive(showPauseMenu);
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
        _transformationSelector.transform.localScale = transfImage.gameObject.transform.parent.localScale;
    }

    private void OnPencilCounterUpdate(string newText)
    {
        _pencilCounter.text = newText;
    }

    private void OnPlaytimeUpdate(string newText)
    {
        _theTime.text = newText;
    }

    private void OnPlayerDeathsUpdate(string newText)
    {
        _deathCounter.text = newText;
    }

    private void NPCChatStart()
    {
        _chatText.text = string.Empty;
        _chatPanel.SetActive(true);
        _chatSkipButton.gameObject.SetActive(false);
    }

    private void NPCChatUpdate(string newText)
    {
        _chatText.text = newText;
    }

    private void NPCChatOnSkipButton(bool show)
    {
        _chatSkipButton.gameObject.SetActive(show);
    }

    private void NPCChatEnd()
    {
        _chatText.text = string.Empty;
        _chatSkipButton.gameObject.SetActive(false);
        _chatPanel.SetActive(false);
    }
}
