using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TowerDefense
{
    public class FortessOptionsUI : Singletons<FortessOptionsUI>
    {
        [Header("Reference")]
        public SelectFortessUI selectFortessUI;
        public GameObject optionUI;
        public GameObject fortress_Quantity_parent;
        public GameObject fortress_Quantity_prefab;
        public GameObject fortressSpriteGO;

        [Header("Upsize")]
        [SerializeField] int price5C = 100;
        [SerializeField] int price7C = 500;
        public GameObject[] canUps;
        public GameObject unlockGO;
        public Image characterIMG;
        public TMPro.TextMeshProUGUI capacityTX;
        public TMPro.TextMeshProUGUI priceUpSizeTX;
        public Button upSizeButton;
        private int currentSoldierMax;

        [Header("Upgrade")]//x1.5
        [SerializeField] int costUpgrade = 100;
        public TMPro.TextMeshProUGUI costTX;
        public TMPro.TextMeshProUGUI[] statusTX;
        public GameObject canUp2;
        public Button upgradeBT;
        public GameObject canupShowGO;
        public GameObject canTupShowGO;

        [Header("Demolition")]
        public Image demolitionSR;
        public Sprite[] demolitionSP;

        public void Update(){}

        public void Show()
        {
            if (optionUI.activeSelf) return;
            if (selectFortessUI.currentFortressSlot != null && selectFortessUI.currentFortressSlot.IsCanDemolition())
            {
                demolitionSR.sprite = demolitionSP[0];
            }
            else
            {
                demolitionSR.sprite = demolitionSP[1];
            }
            currentSoldierMax = selectFortessUI.currentFortressSlot.soldierMax;
            GameManager.Instance.UITransitionController().FortressOptionUIShow(optionUI);
            SetInitUpSize();
            SetInitUpgrade();
        }

        public void Hide()
        {
            Time.timeScale = 1;
            GameManager.Instance.UITransitionController().FortressOptionUIHide(optionUI);
        }

        #region UpSize
        private void SetInitUpSize()
        {
            characterIMG.sprite = selectFortessUI.currentFortressSlot.barrackSlot.barrackModelSO.barrackModel.characterFullSP;
            unlockGO.SetActive(false);
            if (currentSoldierMax == 3) _SetInitUpSize(price5C, "3            5");
            if (currentSoldierMax == 5) _SetInitUpSize(price7C, "5            7");

            if (IsCanTUnlockLevel())
            {
                if (currentSoldierMax == 5)
                {
                    unlockGO.SetActive(true);
                    canUps.ToList().ForEach(o => { o.SetActive(false); });
                    upSizeButton.interactable = false;
                }
            }

            void _SetInitUpSize(int _price, string _capacity)
            {
                unlockGO.SetActive(false);
                priceUpSizeTX.text = price5C.ToString();
                capacityTX.text = _capacity;
                canUps.ToList().ForEach(o => { o.SetActive(false); });
                if (GoldCost.Instance.IsMoreThanCurrent(price5C))
                {
                    canUps.ToList().ForEach(o => { o.SetActive(true); });
                    upSizeButton.interactable = true;
                }
                else upSizeButton.interactable = false;
            }

            bool IsCanTUnlockLevel()
            {
                return
                    LevelStageSlot.Instance.IsStageLevelType(LevelType.Level1Play) ||
                    LevelStageSlot.Instance.IsStageLevelType(LevelType.Level4);
            }
        }

        public void UpSizeButton()
        {
            if (selectFortessUI.currentFortressSlot == null) return;
            if (currentSoldierMax == 3)
            {
                if (GoldCost.Instance.IsMoreThanCurrent(price5C))
                {
                    GoldCost.Instance.ReduceGold(price5C);
                    selectFortessUI.currentFortressSlot.UpSizeFortress(5);
                }
            }
            if (currentSoldierMax == 5)
            {
                if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level4)) return;
                if (GoldCost.Instance.IsMoreThanCurrent(price7C))
                {
                    GoldCost.Instance.ReduceGold(price7C);
                    selectFortessUI.currentFortressSlot.UpSizeFortress(7);
                }
            }
            Hide();
        }
        #endregion

        #region Upgreade
        //Fix โค้ดแบบอันเดียว เดี๋ยวกลับมาแก้ T_T
        public void SetInitUpgrade()
        {
            if (selectFortessUI.currentFortressSlot.barrackSlot.barrackType != BarrackType.SoldierSword)
            {
                canTupShowGO.gameObject.SetActive(true);
                canupShowGO.gameObject.SetActive(false);
            }
            else
            {
                canTupShowGO.gameObject.SetActive(false);
                canupShowGO.gameObject.SetActive(true);

                costTX.text = costUpgrade.ToString();
                canUp2.SetActive(false);
                upgradeBT.interactable = false;
                if (GoldCost.Instance.IsMoreThanCurrent(costUpgrade))
                {
                    canTupShowGO.gameObject.SetActive(false);
                    canupShowGO.gameObject.SetActive(true);

                    canUp2.SetActive(true);
                    upgradeBT.interactable = true;
                }
            }
        }

        //FortressEffect Upgreade
        public void UpgradeAttactFortressButton()
        {
            if (selectFortessUI.currentFortressSlot.barrackSlot.barrackType == BarrackType.SoldierSword)
            {
                if (GoldCost.Instance.IsMoreThanCurrent(costUpgrade))
                {
                    GoldCost.Instance.ReduceGold(costUpgrade);
                    selectFortessUI.currentFortressSlot.UpgreadeEffect(FortressUpgradeType.Acttack);
                    Hide();
                }
            }
        }
        #endregion

        #region Demolition
        public void DemolitionButton()
        {
            if (selectFortessUI.currentFortressSlot != null && selectFortessUI.currentFortressSlot.IsCanDemolition())
            {
                selectFortessUI.currentFortressSlot.GetComponent<FortressSlot>().Demolition();
                optionUI.SetActive(false);
                Time.timeScale = 1;
                //Hide();
            }
        }
        #endregion
    }
}
