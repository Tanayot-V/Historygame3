using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioModel
{
    public string id;
    public AudioType audioType;
    public AudioClip audioClip;
}

public enum AudioType
{
    BGM,
    Effect
}
