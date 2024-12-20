using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singletons<LobbyManager>
{
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private GameObject stratGamePage;
    [SerializeField] private TransitionLobby transitionLobby;

    public void Start()
    {
        StartGameButton();
    }

    #region StartGame-Page
    public void StartGameButton()
    {
        if(stratGamePage.activeSelf) stratGamePage.GetComponent<CanvasGroupTransition>().FadeOut(() => { stratGamePage.SetActive(false); });
        ShowSlotClickButton(1);
        transitionLobby.LobbyShow();
    }
    #endregion

    #region Equipment
    public EquipmentModelSO GetEquipmentModelSO(string _id)
    {
        return equipmentManager.GetEquipmentModelSO(_id);
    }
    #region UIEquipPage
    public void ShowSlotClickButton(int _indexEquipType)
    {
        equipmentManager.ShowSlotClick(_indexEquipType);
    }

    public void ChangeEqipSkin(string _id)
    {
        equipmentManager.ChangeEqipSkin(_id);
    }
    #endregion
    #endregion
}
