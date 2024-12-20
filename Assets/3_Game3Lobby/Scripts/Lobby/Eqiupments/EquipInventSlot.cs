using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipInventSlot : MonoBehaviour
{
    public string id;
    public void ChangeButton()
    {
        LobbyManager.Instance.ChangeEqipSkin(id);
    }
}
