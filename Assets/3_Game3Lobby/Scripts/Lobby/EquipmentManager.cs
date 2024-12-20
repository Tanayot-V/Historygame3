using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CharacterEquip
{
    public string charID;
    public string displayName;
    public GenderType gender;
    public EquipmentModelSO[] paths;
}

public enum EquipType
{
    NONE = 0,
    HEAD = 1,
    UPPERWEAR = 2,
    LOWERWEAR = 3,
    ACCESSORIES = 4
}

public enum GenderType
{
    Man = 0,
    Woman = 1,
}

[System.Serializable]
public class EquipInventorySlot
{
    public string invent_id;
    public EquipmentModelSO equipmentModelSO;
    public bool isLock;
    public bool isWear;
}

[System.Serializable]
public class EquipPageSlot
{
    public EquipType equipType;
    public Sprite icon;
}

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private EquipmentDatabaseSO databaseSO;
    [SerializeField] private GameObject targetBasebody;

    [Header("UIEquipPage")]
    [SerializeField] private EquipType currentEquipType;
    [SerializeField] private Image iconInventShow;
    [SerializeField] private Sprite[] pictureShowSlots;
    [SerializeField] private GameObject inventShow;

    [Header("Current BaseBody")]
    [SerializeField] private CharacterEquip characterEquip;
    [SerializeField] private GameObject manSpineGO;
    [SerializeField] private GameObject womanSpineGO;
    [SerializeField] private List<string> baseBody_ids = new List<string>();

    public List<string> ar = new List<string>();

    [Header("Inventories")]
    [SerializeField] private List<EquipInventorySlot> inventoryLists = new List<EquipInventorySlot>();

    public void Start()
    {
        //InitEquipment(baseBody_ids);
    }

    public void InitEquipment(List<string> _baseBody_ids)
    {
        List<string> paths = new List<string>();
        _baseBody_ids.ForEach(id => { paths.Add(LobbyManager.Instance.GetEquipmentModelSO(id).path); });
        targetBasebody.GetComponent<SpineEntitySkinByPath>().ChangeSkinFormPath(paths.ToArray());
    }

    public EquipmentModelSO GetEquipmentModelSO(string _id)
    {
        return databaseSO.GetEquipmentModelSO(_id);
    }

    #region UIEquipPage
    public void ShowSlotClick(int _indexEquipType)
    {
        if (_indexEquipType == 6)
        {
            if (GetCurrentGender() == GenderType.Man)
            {
                ChangeGenderSpine(GenderType.Woman);
            }
            else
            {
                ChangeGenderSpine(GenderType.Man);
            }
        }
        else ChangeEqipPage((EquipType)_indexEquipType);

        void ChangeEqipPage(EquipType equipType)
        {
            currentEquipType = equipType;
            iconInventShow.sprite = databaseSO.GetEquipPageSlot(equipType).icon;
            inventShow.GetComponent<CanvasGroupTransition>().FadeIn();
        }     
    }

    private void ChangeGenderSpine(GenderType _genderType)
    {
        manSpineGO.SetActive(false);
        womanSpineGO.SetActive(false);
        switch (_genderType)
        {
            case GenderType.Man:
                manSpineGO.SetActive(true);
                baseBody_ids[5] = "Man";
                targetBasebody = manSpineGO;
                break;
            case GenderType.Woman:
                womanSpineGO.SetActive(true);
                baseBody_ids[5] = "Woman";
                targetBasebody = womanSpineGO;
                break;
        }
    }

    public GenderType GetCurrentGender()
    {
        GenderType genderType = GenderType.Man;
        if(baseBody_ids[5] != "Man") genderType = GenderType.Woman;
        return genderType;
    }

    public void ChangeEqipSkin(string _id)
    {
        if (_id == "default") ar.Clear();
        ar.Add(_id);
        targetBasebody.GetComponent<SpineEntitySkinByPath>().ChangeSkinFormPath(ar.ToArray());
    }
    #endregion

}
