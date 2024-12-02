using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TowerDefense
{
    public class LevelStageUI : MonoBehaviour
    {
        [Header("FortessUI")]
        public SelectFortessUI selectFortessUI;
        public FortessOptionsUI fortessOptionsUI;

        [Header("UpgradeUI")]
        public GameObject upgradeUI;
        public GameObject upgradeViewUI;
        public TMPro.TextMeshProUGUI upgradeLvTX;

        public TMPro.TextMeshProUGUI barracknameTX;
        public GameObject demolitionUI;
        public GameObject towerFightUI;
        public GameObject startGameUI;
        public GameObject triviaUI;
        public GameObject victoryUI;
        public GameObject loseUI;
        public GameObject popupUI;
        public GameObject statusTimeUI;
        public TMPro.TextMeshProUGUI waveTX;
        public GameObject cutscene;

        public void InitUIStart()
        {
            demolitionUI.SetActive(false);
            towerFightUI.SetActive(false);
            startGameUI.SetActive(false);
            triviaUI.SetActive(false);
            victoryUI.SetActive(false);
            loseUI.SetActive(false);
            if(popupUI != null) popupUI.SetActive(false);
            cutscene.SetActive(false);
        }

        public void LoadNewScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ShowPopupUI(string _id)
        {
            popupUI.GetComponent<PopupUI>().Show(_id, LevelStageSlot.Instance.stageModelSO.levelStageModel.stageDetail.popups[0]);
        }
    }
}
