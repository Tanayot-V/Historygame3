using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentModelSO", menuName = "ScriptableObjects/EquipmentModelSO", order = 1)]
public class EquipmentModelSO : ScriptableObject
{
    public string equip_id;
    public Sprite picture;
    public string path;

    public EquipmentModelSO(string _id,Sprite _pic,string _path)
    {
        equip_id = _id;
        picture = _pic;
        path = _path;
    }
}
