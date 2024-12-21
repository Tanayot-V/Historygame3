using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Analytics;
using static Spine.Unity.Examples.EquipSystemExample;

[CreateAssetMenu(fileName = "EquipmentDatabaseSO", menuName = "ScriptableObjects/EquipmentDatabaseSO", order = 1)]
public class EquipmentDatabaseSO : ScriptableObject
{
    public List<EquipCharacterModelSO> characterModelSOLists = new List<EquipCharacterModelSO>();
    public List<EquipPageSlot> equipPageSlotLists = new List<EquipPageSlot>(); //Show Slot UI 
    public List<EquipmentModelSO> equipmentModelSOLists = new List<EquipmentModelSO>();

    private Dictionary<EquipType, EquipPageSlot> equipTypeDIC = new Dictionary<EquipType, EquipPageSlot>();
    public EquipPageSlot GetEquipPageSlot(EquipType _equipType)
    {
        if (equipTypeDIC.ContainsKey(_equipType))
        {
            return equipTypeDIC[_equipType];
        }
        else
        {
            return equipTypeDIC[_equipType] = equipPageSlotLists.ToList().Find(o => o.equipType == _equipType);
        }
    }

    private Dictionary<string, EquipmentModelSO> equipmentModelSODic = new Dictionary<string, EquipmentModelSO>();
    public EquipmentModelSO GetEquipmentModelSO(string _equipid)
    {
        if (equipmentModelSODic.ContainsKey(_equipid))
        {
            return equipmentModelSODic[_equipid];
        }
        else
        {
            return equipmentModelSODic[_equipid] = equipmentModelSOLists.ToList().Find(o => o.equip_id == _equipid);
        }
    }  
}
