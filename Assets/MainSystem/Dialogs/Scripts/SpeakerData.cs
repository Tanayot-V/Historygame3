using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpeakerData : MonoBehaviour
{
    public List<Speaker> speakersList = new List<Speaker>();

    private Dictionary<string, Speaker> speakerDic = new Dictionary<string, Speaker>();
    public Speaker GetSpeaker(string _id)
    {
        if (speakerDic.ContainsKey(_id))
        {
            return speakerDic[_id];
        }
        else
        {
            Speaker foundSpeaker = speakersList.Find(o => o.speakerID == _id);
            if (foundSpeaker != null)
            {
                speakerDic[_id] = foundSpeaker;
                return foundSpeaker;
            }
            else
            {
                Debug.LogError($"Speaker not found: {_id}");
                return null;
            }
        }
    }
}
