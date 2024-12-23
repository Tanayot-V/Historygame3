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
    public Sprite picture;
    public EquipmentModelSO[] equipModels;

    public string[] GetPaths()
    {
        List<string> paths = new List<string>();
        equipModels.ToList().ForEach(o => {
            paths.Add(o.path);
        });
        return paths.ToArray();
    }

    public string[] GetPathBaseBody()
    {
        return new string[] { equipModels[0].path, equipModels[1].path };
    }

    private Dictionary<EquipType, EquipmentModelSO> equipTypeDIC = new Dictionary<EquipType, EquipmentModelSO>();
    public EquipmentModelSO GetEquipmentModelSObyType(EquipType _equipType)
    {
        if (equipTypeDIC.ContainsKey(_equipType))
        {
            return equipTypeDIC[_equipType];
        }
        else
        {
            return equipTypeDIC[_equipType] = equipModels.ToList().Find(o => o.equipType == _equipType);
        }
    }
}
