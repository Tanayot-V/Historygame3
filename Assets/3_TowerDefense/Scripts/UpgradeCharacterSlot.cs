using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class UpgradeCharacterSlot : MonoBehaviour
    {
        public BarrackSlot barrackSlot;
        [SerializeField] BarrackUpgrade[] upgradeRef;
        [SerializeField] BarrackUpgrade upgradeCurrent;
        public TMPro.TextMeshProUGUI levelShowTX;
        public TMPro.TextMeshProUGUI levelTargetTX;
        public TMPro.TextMeshProUGUI costTX;
        public TMPro.TextMeshProUGUI[] statusTX;
        public Button buttons;
        public GameObject canUpGO;
        int costUpgrade;

        public void SetInit()
        {
            upgradeRef = barrackSlot.upgradeRef;
            upgradeCurrent = barrackSlot.upgradeCurrent;
            levelShowTX.text = "LV." + upgradeCurrent.level.ToString();
            if (upgradeCurrent.level == 1) levelTargetTX.text = "LV.1      LV.2";
            else if (upgradeCurrent.level == 2) levelTargetTX.text = "LV.2      LV.3";
            else if (upgradeCurrent.level == 3) levelTargetTX.text = "LV.3      MAX LV.";

            if (upgradeCurrent.level < 3)
            {
                costUpgrade = upgradeRef[upgradeCurrent.level].costUpgrade;
                costTX.text = costUpgrade.ToString();
            }
            else
            {
                costUpgrade = 0;
                costTX.text = "MaX LV";
            }

            canUpGO.SetActive(false);
            buttons.interactable = true;

            if (upgradeCurrent.level == 3)
            {
                buttons.interactable = false; ;
            }
            if (upgradeCurrent.level == 3) return;
            if (GoldCost.Instance.IsMoreThanCurrent(costUpgrade))
            {
                canUpGO.SetActive(true);
                buttons.interactable = true;
            }
            else buttons.interactable = false;
        }

        public void UpgradeStatus()
        {
            if (!buttons.interactable) return;
            if (upgradeCurrent.level == 3) return;
            barrackSlot.SetBarrackUpgrade(upgradeRef[upgradeCurrent.level]);
            GoldCost.Instance.ReduceGold(costUpgrade);
            GameManager.Instance.LevelStageUI().upgradeViewUI.GetComponent<UpgradeView>().panalGO.SetActive(false);
            //GameManager.Instance.LevelStageUI().upgradeViewUI.GetComponent<UpgradeView>().Show();
            Time.timeScale = 1;
        }
    }
}
