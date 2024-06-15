using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Scriptables/Dialogues")]
public class DatabaseDialogues : ScriptableObject
{
    [SerializeField, Range(0.001f, 0.1f)] private float _wordsSpeed = 0.1f;
    [SerializeField] private string _npcName = "Character";
    [SerializeField] private string _playerName = "Lito";
    [SerializeField] private List<string> _dialogueTextLines = new List<string>();

    public float DialogueSpeed => _wordsSpeed;
    public int DialogueLength => _dialogueTextLines.Count - 1;
    public List<string> Dialogue => _dialogueTextLines;
    
    public string DialogueLine(int index)
    {
        if (_dialogueTextLines[index] != null)
        {
            // Replace with character names
            string line = _dialogueTextLines[index];
            line = line.Replace("@talker", _npcName);
            line = line.Replace("@player", _playerName);
            return line;
        }
        else return "Index given out of range.";
    }
}
