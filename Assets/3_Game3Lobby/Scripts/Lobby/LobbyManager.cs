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
        stratGamePage.GetComponent<CanvasGroupTransition>().FadeOut(() => { stratGamePage.SetActive(false); });
        transitionLobby.LobbyShow();
    }
    #endregion

    #region Equipment
    public EquipmentModelSO GetEquipmentModelSO(string _id)
    {
        return equipmentManager.equipmentDatabaseSO.GetEquipmentModelSO(_id);
    }
    #endregion
}
