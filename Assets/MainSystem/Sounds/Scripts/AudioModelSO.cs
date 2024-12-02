using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "AudioModelSO", menuName = "Main System/AudioModelSO", order = 1)]
public class AudioModelSO : ScriptableObject
{
    public AudioModel[] audioModels;

    public Dictionary<string, AudioModel> audioModelDic = new Dictionary<string, AudioModel>();
    public AudioModel GetAudioModel(string _id)
    {
        if (audioModelDic.ContainsKey(_id))
        {
            return audioModelDic[_id];
        }
        else
        {
            AudioModel foundDic = audioModels.ToList().FirstOrDefault(o => o.id == _id);
            if (foundDic != null)
            {
                audioModelDic[_id] = foundDic;
                return foundDic;
            }
            else
            {
                Debug.LogError($"audioClipsDic not found: {_id}");
                return audioModelDic[_id] = audioModels.ToList().Find(o => o.id == _id);
            }
        }
    }
}
