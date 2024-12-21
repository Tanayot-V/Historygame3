using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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
    SKIN = 0,
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
    public string id;
    public EquipmentModelSO modelSO;
    public bool isLock;
    public bool isWear;

    public EquipInventorySlot(string _id, EquipmentModelSO _modelSO)
    {
        id = _id;
        modelSO = _modelSO;
    }
}

[System.Serializable]
public class EquipPageSlot
{
    public EquipType equipType;
    public Sprite icon;
}


[System.Serializable]
public class EquipTargetSlot
{
    public EquipType equipType;
    public Image showSlotIMG;
    public EquipmentModelSO modelSO;
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
    [SerializeField] private int indexCharacterEquip;
    [SerializeField] private EquipCharacterModelSO currentEquipCharModelSO;
    [SerializeField] private string[] pathBaseBadySkins;
    [SerializeField] private GameObject manSpineGO;
    [SerializeField] private GameObject womanSpineGO;
    [SerializeField] private List<EquipTargetSlot> targetPaths = new List<EquipTargetSlot>();
    [SerializeField] private List<string> pathEquips = new List<string>();
    [SerializeField] private List<string> equipsID = new List<string>();

    [Header("Inventories")]
    [SerializeField] private GameObject prefabInvent;
    [SerializeField] private GameObject prefabInventlock;
    [SerializeField] private Transform parantInvet;
    [SerializeField] private List<EquipCharacterModelSO> characterModelSOLists = new List<EquipCharacterModelSO>();
    private List<EquipmentModelSO> equipmentModelSOList_Man = new List<EquipmentModelSO>();
    private List<EquipmentModelSO> equipmentModelSOList_Woman = new List<EquipmentModelSO>();

    [SerializeField] private List<EquipInventorySlot> currentInventory = new List<EquipInventorySlot>();

    public void Start() {}

    public void InitEquipment()
    {
        InitInventory();
        indexCharacterEquip = 0;
        currentEquipType = EquipType.HEAD;
        SetupCharacter(indexCharacterEquip);
        ShowSlotClick(2);
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
        if (_indexEquipType == (int)currentEquipType) return;
        if (_indexEquipType == 0)
        {
            indexCharacterEquip += 1;
            SetupCharacter(indexCharacterEquip);
            currentEquipType = EquipType.HEAD;
            ShowSlotClick(2);
            return;
        }
        else
        {
            ChangeEqipPage((EquipType)_indexEquipType);
            SetupInventory(currentEquipCharModelSO.gender, currentEquipType);

            Button btnInvAll = ButtonGroupManager.Instance.GetButton("EqiupShowSlot", "Upper");
            ButtonGroupManager.Instance.Select(btnInvAll.GetComponent<ButtonGroup>());
        }

        void ChangeEqipPage(EquipType equipType)
        {
            currentEquipType = equipType;
            iconInventShow.sprite = databaseSO.GetEquipPageSlot(equipType).icon;
            inventShow.GetComponent<CanvasGroupTransition>().FadeIn();
        }     
    }

    private void SetupCharacter(int _index)
    {
        if (_index > characterModelSOLists.Count - 1) _index = 0;
        currentEquipCharModelSO = characterModelSOLists[_index];
        List<string> pathsBaseList = new List<string>();
        pathBaseBadySkins = currentEquipCharModelSO.GetPathBaseBody();
        indexCharacterEquip = _index;

        if (GetCurrentGender() == GenderType.Woman)
        {
            ChangeGenderSpine(GenderType.Woman);
        }
        else
        {
            ChangeGenderSpine(GenderType.Man);
        }

        pathEquips.Clear();
        equipsID.Clear();
        equipsID.Add("Character ID: "+ currentEquipCharModelSO.charID);
        targetPaths.ForEach(o => {
            var modelSO = currentEquipCharModelSO.GetEquipmentModelSObyType(o.equipType);
            if (modelSO != null)
            {
                o.modelSO = modelSO;
                pathEquips.Add(o.modelSO.path);
                equipsID.Add(o.modelSO.equip_id);
            }
        });
        targetBasebody.GetComponent<SpineEntitySkinByPath>().ChangeSkinFormPath(pathEquips.ToArray());
    }

    private void ChangeGenderSpine(GenderType _genderType)
    {
        manSpineGO.SetActive(false);
        womanSpineGO.SetActive(false);
        switch (_genderType)
        {
            case GenderType.Man:
                manSpineGO.SetActive(true);
                targetBasebody = manSpineGO;
                break;
            case GenderType.Woman:
                womanSpineGO.SetActive(true);
                targetBasebody = womanSpineGO;
                break;
        }
    }

    public GenderType GetCurrentGender()
    {
        GenderType genderType = GenderType.Man;
        if(currentEquipCharModelSO.gender != GenderType.Man) genderType = GenderType.Woman;
        return genderType;
    }

    public void ChangeEqipSkin(EquipmentModelSO _modelSO)
    {
        pathEquips.Clear();
        equipsID.Clear();
        equipsID.Add("Character ID: " + currentEquipCharModelSO.charID);
        targetPaths.ForEach(o => {
            if (_modelSO.equipType == o.equipType)
            {
                o.modelSO = _modelSO;
                o.showSlotIMG.sprite = o.modelSO.picture;
            }
            pathEquips.Add(o.modelSO.path);
            equipsID.Add(o.modelSO.equip_id);
        });
        targetBasebody.GetComponent<SpineEntitySkinByPath>().ChangeSkinFormPath(pathEquips.ToArray());
    }
    #endregion

    #region Inventory
    public void InitInventory()
    {
        databaseSO.equipmentModelSOLists.ForEach(o => {
            if (o != null)
            {
                if (o.genderType == GenderType.Man)
                {
                    equipmentModelSOList_Man.Add(o);
                }
                else
                {
                    if (o != null) equipmentModelSOList_Woman.Add(o);
                }
            }
        });
    }

    private void SetupInventory(GenderType _genderType, EquipType _equipType)
    {
        Debug.Log(_genderType+"|"+_equipType);
        currentInventory.Clear();
        switch (_genderType)
        {
            case GenderType.Man:
                equipmentModelSOList_Man.ForEach(o => {
                    if(o.equipType == _equipType) currentInventory.Add(new EquipInventorySlot(o.equip_id,o));
                });
                break;
            case GenderType.Woman:
                equipmentModelSOList_Woman.ForEach(o => {
                    if (o.equipType == _equipType) currentInventory.Add(new EquipInventorySlot(o.equip_id, o));
                });
                break;
        }

        int inventCount = 12 - currentInventory.Count;
        Debug.Log(currentInventory.Count);
        UiController.Instance.DestorySlot(parantInvet.gameObject);
        currentInventory.ForEach(o => {
            GameObject slot = UiController.Instance.InstantiateUIView(prefabInvent);
            slot.transform.SetParent(parantInvet);
            slot.GetComponent<EquipInventSlot>().InitSlot(o.modelSO); 
        });

        for (int i = 0; i < inventCount; i++)
        {
            GameObject slot = UiController.Instance.InstantiateUIView(prefabInventlock);
            slot.transform.SetParent(parantInvet);
        }
    }

    #endregion

}
