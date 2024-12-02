using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class GoldCost : Singletons<GoldCost>
    {
        public int currentGold;
        public TMPro.TextMeshProUGUI goldTX;
        public BarrackSlot barrackSlot;
        public GameObject goldPrarent;
        public GameObject goldGenPrefab;

        void Start()
        {
            goldTX.text = "0";
        }

        void Update()
        {
        }

        public void UpdateGold()
        {
            goldTX.text = currentGold.ToString();
        }

        public void IncreaseGold(int _gold)
        {
            currentGold += _gold;
            UpdateGold();
        }

        public void ReduceGold(int _gold)
        {
            currentGold -= _gold;
            if (currentGold <= 0) currentGold = 0;
            UpdateGold();
        }

        public void UpgradeBarrack()
        {
            BarrackUpgrade upgradeTarget = barrackSlot.upgradeRef[barrackSlot.barrackState.level];
            //BarrackUpgrade upgradeTarget = barrackSlot.barrackModelSO.barrackModel.GetBarrackUpgrade(barrackSlot.barrackState.level + 1);
            currentGold -= upgradeTarget.costUpgrade;
            goldTX.text = currentGold.ToString();
            barrackSlot.SetBarrackUpgrade(upgradeTarget);
            GameManager.Instance.LevelStageUI().barracknameTX.text = "ฐานนักดาบ Lv." + barrackSlot.barrackState.level;
            GameManager.Instance.LevelStageUI().upgradeUI.SetActive(false);

            //กดอัพเกรดใน Tutorial
            if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level1Tutorial))
            {
                if (TutorialUI.Instance.arrowUpgradeUI.activeSelf)
                {
                    TutorialUI.Instance.arrowUpgradeUI.SetActive(false);
                    TutorialStatge1.Instance.barrack_SwordSlot.isStopSpawning = false;
                    StartCoroutine(UiController.Instance.WaitForSecond(1, () =>
                    {
                        TutorialStatge1.Instance.enemyMovement.isStopAttack = false;
                    }));
                }
            }
        }

        // ฟังก์ชันสำหรับเช็คว่าเงินพารามิเตอร์มากกว่าหรือไม่
        public bool IsMoreThanCurrent(float _money)
        {
            return currentGold >= _money;
        }

        public GameObject CreateGoldUI(int _amount)
        {
            GameObject gold = UiController.Instance.InstantiateUIView(goldGenPrefab, goldPrarent);
            gold.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "+"+_amount;
            return gold;
        }
    }
}
