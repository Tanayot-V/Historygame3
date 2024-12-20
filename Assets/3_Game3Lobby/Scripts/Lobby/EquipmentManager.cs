using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EquipSlot
{
    public string slot_id;
    public EquipType equipType;
    public string equip_id;
}

public enum EquipType
{
    HEAD,
    CLOTH,
    ACCESSORIES,
    WEAPON
}

[System.Serializable]
public class EquipInventorySlot
{
    public string invent_id;
    public EquipmentModelSO equipmentModelSO;
    public bool isLock;
    public bool isWear;
}

public class EquipmentManager : MonoBehaviour
{
    public EquipmentDatabaseSO equipmentDatabaseSO;
    public GameObject targetBasebody;

    [Header("Current BaseBody")]
    public List<string> baseBody_ids = new List<string>();

    [Header("Inventories")]
    public List<EquipInventorySlot> headInvents = new List<EquipInventorySlot>();
    public List<EquipInventorySlot> colthInvents = new List<EquipInventorySlot>();
    public List<EquipInventorySlot> accInvents = new List<EquipInventorySlot>();
    public List<EquipInventorySlot> weaponInvents = new List<EquipInventorySlot>();

    public void Start()
    {
        InitEquipment(baseBody_ids);
    }
    public void InitEquipment(List<string> _baseBody_ids)
    {
        List<string> paths = new List<string>();
        _baseBody_ids.ForEach(id => { paths.Add(LobbyManager.Instance.GetEquipmentModelSO(id).path); });
        targetBasebody.GetComponent<SpineEntitySkinByPath>().ChangeSkinFormPath(paths.ToArray());
    }
}
