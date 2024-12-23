using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singletons<LobbyManager>
{
    [SerializeField] private UILobbyManager uILobbyManager;
    [SerializeField] private GameObject stratGamePage;
    [SerializeField] private EquipmentManager equipmentManager;
    [SerializeField] private TransitionLobby transitionLobby;

    public void Start()
    {
        StartGameButton();
    }

    #region StartGame-Page
    public void StartGameButton()
    {
        if(stratGamePage.activeSelf) stratGamePage.GetComponent<CanvasGroupTransition>().FadeOut(() => { stratGamePage.SetActive(false); });
        uILobbyManager.InitEquipment();
        transitionLobby.LobbyShow();
    }
    #endregion

    #region Equipment
    public EquipmentModelSO GetEquipmentModelSO(string _id)
    {
        return equipmentManager.GetEquipmentModelSO(_id);
    }
    #endregion
}
