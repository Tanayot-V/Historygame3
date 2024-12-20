using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentModelSO", menuName = "ScriptableObjects/EquipmentModelSO", order = 1)]
public class EquipmentModelSO : ScriptableObject
{
    public string equip_id;
    public string displayName;
    public Sprite picture;
    public GenderType genderType;
    public EquipType equipType;
    public string path;

    public EquipmentModelSO(string _id,Sprite _pic, GenderType _genderType, EquipType _equipType, string _path)
    {
        equip_id = _id;
        picture = _pic;
        genderType = _genderType;
        equipType = _equipType;
        path = _path;
    }
}
