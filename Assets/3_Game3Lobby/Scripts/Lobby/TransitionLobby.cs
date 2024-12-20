using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionLobby : MonoBehaviour
{
    public Animator animLobby;

    public void LobbyShow()
    {
        animLobby.gameObject.SetActive(true);
        animLobby.speed = 1.25f;
        animLobby.Play("LobbyOpen");
    }

    public void LobbyHide()
    {
        animLobby.Play("LobbyClose");
    }

    public void LeaderboardShow()
    {

    }

    public void LeaderboardHide()
    {

    }

    public void DiaryShow()
    {

    }

    public void DiaryHide()
    {

    }

    public void CombatModeShow()
    {

    }

    public void CombatModeHide()
    {

    }
}
