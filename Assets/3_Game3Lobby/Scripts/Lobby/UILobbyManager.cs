using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyManager : Singletons<UILobbyManager>
{
    [SerializeField] private EquipmentManager equipmentManager;

    #region Equipment
    public void InitEquipment()
    {
        equipmentManager.InitEquipment();
    }

    public void ShowSlotClickButton(int _indexEquipType)
    {
        equipmentManager.ShowSlotClick(_indexEquipType);
    }

    public void ChangeEqipSkin(EquipmentModelSO _modelSO)
    {
        equipmentManager.ChangeEqipSkin(_modelSO);
    }

    public void ConfirmButton()
    {
        equipmentManager.ConfirmButton();
    }
    #endregion
}
