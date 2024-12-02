using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(fileName = "NewSpeaker", menuName = "Main System/New Speaker")]
[System.Serializable]
public class Speaker : ScriptableObject
{
    public string speakerID;
    public string speakerName;
    public Color textColor;
    public List<Character> characters;

    public Sprite GetSpriteEmotion(Emotion _emotion)
    {
        Character character = characters.Find(o => o.emotion == _emotion);
        return character.sprite;
    }

    [System.Serializable]
    public struct Character
    {
        public Emotion emotion;
        public Sprite sprite;
        //Spine
    }
}

public enum Emotion
{
    IDLE,
    HAPPY,
    ANGRY,
    HUNGRY
}
