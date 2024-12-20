using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "EquipmentDatabaseSO", menuName = "ScriptableObjects/EquipmentDatabaseSO", order = 1)]
public class EquipmentDatabaseSO : ScriptableObject
{
    public List<EquipmentModelSO> equipmentModelSOs = new List<EquipmentModelSO>();

    private Dictionary<string, EquipmentModelSO> equipmentModelSODic = new Dictionary<string, EquipmentModelSO>();
    public EquipmentModelSO GetEquipmentModelSO(string _equipid)
    {
        if (equipmentModelSODic.ContainsKey(_equipid))
        {
            return equipmentModelSODic[_equipid];
        }
        else
        {
            return equipmentModelSODic[_equipid] = equipmentModelSOs.ToList().Find(o => o.equip_id == _equipid);
        }
    }
}
