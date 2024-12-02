using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EffectModelSO", menuName = "Main System/EffectModelSO", order = 1)]
public class EffectModelSO : ScriptableObject
{
    public EffectModel[] effectModels;

    public Dictionary<string, EffectModel> effectModelDic = new Dictionary<string, EffectModel>();
    public EffectModel GetEffectModel(string _id)
    {
        if (effectModelDic.ContainsKey(_id))
        {
            return effectModelDic[_id];
        }
        else
        {
            EffectModel foundDic = effectModels.ToList().FirstOrDefault(o => o.id == _id);
            if (foundDic != null)
            {
                effectModelDic[_id] = foundDic;
                return foundDic;
            }
            else
            {
                Debug.LogError($"audioClipsDic not found: {_id}");
                return effectModelDic[_id] = effectModels.ToList().Find(o => o.id == _id);
            }
        }
    }
}
