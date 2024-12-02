using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Reflection;

namespace TowerDefense
{
    [System.Serializable]
    public class UpgradeViewSlot
    {
        public BarrackType barrackType;
        public BarrackSlot barrackSlot;
    }

    public class UpgradeView : MonoBehaviour
    {
        [Header("Upgrade")]
        public GameObject panalGO;
        public TMPro.TextMeshProUGUI goldcostCurrent;
        public TMPro.TextMeshProUGUI[] costs;
        public BarrackSlot[] barrackSlots;
        public UpgradeCharacterSlot[] upgradeCharacterSlots;

        public void Show()
        {
            if (panalGO.activeSelf) return;
            panalGO.SetActive(true);
            goldcostCurrent.text = GoldCost.Instance.currentGold.ToString();
            upgradeCharacterSlots.ToList().ForEach(o => {
                o.SetInit();
            });
            Time.timeScale = 0;
            /*
            BarrackSlot[] barrackSlots = LevelStageSlot.Instance.mainBarrack.barrackSlots;

            int index = 0;
            upgradeSlot.ToList().ForEach(upgrade => {
                upgrade.id = index.ToString();
                upgrade.texts[0].text = barrackSlots[index].barrackModelSO.barrackModel.displayName;
                upgrade.texts[1].text = "LV." + barrackSlots[index].upgradeCurrent.level;
                upgrade.images[0].sprite = barrackSlots[index].upgradeCurrent.characterSP;
                upgrade.images[1].gameObject.SetActive(false);
                if (GameManager.Instance.LevelStageData().statusModelSO.IsCanUpgrade(barrackSlots[index].upgradeRef, barrackSlots[index].barrackState.level, GameManager.Instance.GoldCost().currentGold))
                {
                    upgrade.images[1].gameObject.SetActive(true);
                }
                index++;
            });*/
        }

        public void TimeScale(int _time)
        {
            Time.timeScale = _time;
        }

        public void UpgradeSolier(int _index)
        {
            upgradeCharacterSlots[_index].UpgradeStatus();
        }
    }
}
