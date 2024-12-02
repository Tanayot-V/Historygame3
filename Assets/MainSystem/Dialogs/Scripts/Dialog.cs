using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialog
{
    public string dialogID;
    public List<Sentence> sentences;
    public Sprite background;

    [Header("Next Action")]
    public DialogTakeAction nextAction;
    public string nextActionID;

    public Dialog() { }

    public Dialog(string _dialogID , List<Sentence> _sentences , Sprite _background , DialogTakeAction _nextAction , string _nextActionID)
    {
        dialogID = _dialogID;
        sentences = _sentences;
        background = _background;
        nextAction = _nextAction;
        nextActionID = _nextActionID;
    }

    [System.Serializable]
    public struct Sentence
    {
        public string text;
        public string speakerID;
        public Emotion emotion;

        public Sentence(string _text, string _speakerID, Emotion _emotion)
        {
            text = _text;
            speakerID = _speakerID;
            emotion = _emotion;
        }
    }
}

public enum DialogTakeAction
{
    NONE,
    NEXTDIALOG,
    SITUATION, //Game 6
    QUIZ, //Game 5
    NOVELCHOICE //Game 4
}
