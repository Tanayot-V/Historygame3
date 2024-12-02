using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Background
{
    public string id;
    public Sprite sprite;
}

public class BackgroundData: Singletons<BackgroundData>
{
    public List<Background> backgroundLists = new List<Background>();

    public Dictionary<string, Background> backgroundDic = new Dictionary<string, Background>();
    public Background GetBackground(string _id)
    {
        if (backgroundDic.ContainsKey(_id))
        {
            return backgroundDic[_id];
        }
        else
        {
            Background foundDic = backgroundLists.Find(o => o.id == _id);
            if (foundDic != null)
            {
                backgroundDic[_id] = foundDic;
                return foundDic;
            }
            else
            {
                Debug.LogError($"Background not found: {_id}");
                return null;
            }
        }
    }
}