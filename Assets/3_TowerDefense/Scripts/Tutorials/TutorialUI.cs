using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TutorialUI : Singletons<TutorialUI>
    {
        public GameObject statusBarrackUI;
        public GameObject arrowBarrackUI;
        public GameObject arrowFortressUI;
        public GameObject arrowSelectFortressUI;
        public GameObject arrowUpgradeUI;
        public GameObject arrowFightTownUI;
        public GameObject arrowHpEnemyUI;

        public void Start()
        {
            statusBarrackUI.SetActive(false);
            arrowBarrackUI.SetActive(false);
            arrowFortressUI.SetActive(false);
            arrowSelectFortressUI.SetActive(false);
            arrowUpgradeUI.SetActive(false);
            arrowFightTownUI.SetActive(false);
            arrowHpEnemyUI.SetActive(false);
            GameManager.Instance.LevelStageUI().selectFortessUI.Hide();
            GameManager.Instance.LevelStageUI().upgradeUI.SetActive(false);
            GameManager.Instance.LevelStageUI().demolitionUI.SetActive(false);
            GameManager.Instance.LevelStageUI().towerFightUI.SetActive(false);
        }
    }
}
