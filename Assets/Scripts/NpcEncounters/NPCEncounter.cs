using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCEncounter : MonoBehaviour
{
    [SerializeField] private DatabaseDialogues _dialogueData;

    // Values
    private bool _isPlayerListening;
    private bool _isAlreadyInDialog = false;
    private bool _canGoNextLine = false;
    private bool _dialogCompleted = false;

    private int _dialogCurrentLineIndex = 0;
    private string _dialogCurrentLine;
    private float _dialogCurrentWordTime;

    private void Start()
    {
        NPCEncounterEvents.OnNPCNextDialogue += NextLine;
    }

    private void OnDestroy()
    {
        NPCEncounterEvents.OnNPCNextDialogue -= NextLine;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _isPlayerListening = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        _isPlayerListening = false;
    }

    void Update()
    {
        if (_isPlayerListening)
        {
            // Check if this dialogue is started, otherwise start it
            if (!_isAlreadyInDialog)
            {
                ResetDialogue();
                _isAlreadyInDialog = true;
                UIEvents.OnStartNPCDialogue?.Invoke();

                if (!_dialogCompleted)
                {
                    GameManager.Instance.OnPauseUnpauseTimer(true);
                    PlayerEvents.OnEnableDisableControls(false);
                }
                else UIEvents.OnEnableSkipNPCDialogue(true);
            }
        }

        else
        {
            // Reset if the player goes away
            if (_isAlreadyInDialog)
            {
                ResetDialogue();
                _isAlreadyInDialog = false;
                UIEvents.OnEndNPCDialogue?.Invoke();
                GameManager.Instance.OnPauseUnpauseTimer(false);
            }
        }

        if (_dialogCompleted) _canGoNextLine = true;
    }

    private void FixedUpdate()
    {
        if (_isPlayerListening) NPCDialogueExec(Time.deltaTime);
    }

    private void NPCDialogueExec(float delta)
    {
        if (_canGoNextLine && !_dialogCompleted) return;

        if (_dialogCurrentWordTime > 0)
        {
            _dialogCurrentWordTime -= delta;
            return;
        }

        if (_dialogueData.DialogueLine(_dialogCurrentLineIndex) != _dialogCurrentLine)
        {
            _canGoNextLine = false;
            _dialogCurrentLine = _dialogueData.DialogueLine(_dialogCurrentLineIndex).Substring(0, _dialogCurrentLine.Length + 1);
            UIEvents.OnUpdateNPCDialogue(_dialogCurrentLine);
        }
        else if (!_canGoNextLine)
        {
            _canGoNextLine = true;
            UIEvents.OnEnableSkipNPCDialogue(true);
        }

        _dialogCurrentWordTime = _dialogueData.DialogueSpeed;
    }


    private void ResetDialogue()
    {
        _dialogCurrentLine = string.Empty;
        _dialogCurrentWordTime = 0;
        _dialogCurrentLineIndex = 0;
    }


    public void NextLine()
    {
        if (!_isPlayerListening || !_canGoNextLine) return;
        _canGoNextLine = false;
        _dialogCurrentLine = string.Empty;
        _dialogCurrentWordTime = _dialogueData.DialogueSpeed;

        if (_dialogCurrentLineIndex == _dialogueData.DialogueLength)
        {
            UIEvents.OnEndNPCDialogue?.Invoke();

            if (!_dialogCompleted)
            {
                _dialogCompleted = true;
                PlayerEvents.OnEnableDisableControls(true);
            }
        }

        else
        {
            _dialogCurrentLineIndex++;
            if (_dialogCompleted) UIEvents.OnEnableSkipNPCDialogue(true);
            else UIEvents.OnEnableSkipNPCDialogue(false);
        }
    }


}
