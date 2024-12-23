using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquipInventSlot : MonoBehaviour
{
    [SerializeField] private Image imageItem;
    [SerializeField] private EquipInventorySlot equipInventory;
    public void ChangeButton()
    {
        UILobbyManager.Instance.ChangeEqipSkin(equipInventory.modelSO);
    }

    public void InitSlot(EquipmentModelSO _modelSO)
    {
        equipInventory.modelSO = _modelSO;
        imageItem.sprite = equipInventory.modelSO.picture;
    }
}
