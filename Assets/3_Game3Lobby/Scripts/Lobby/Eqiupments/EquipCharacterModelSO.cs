using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EquipCharacterModelSO", menuName = "ScriptableObjects/EquipCharacterModelSO", order = 1)]
public class EquipCharacterModelSO : ScriptableObject
{
    public string charID;
    public string displayName;
    public GenderType gender;
    public EquipmentModelSO[] equipModels;

    public string[] GetPaths()
    {
        List<string> paths = new List<string>();
        equipModels.ToList().ForEach(o => {
            paths.Add(o.path);
        });
        return paths.ToArray();
    }
}
