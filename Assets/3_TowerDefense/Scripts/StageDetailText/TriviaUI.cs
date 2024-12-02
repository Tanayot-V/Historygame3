using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class TriviaUI : MonoBehaviour
    {
        public string triviaUI;
        [SerializeField] TMPro.TextMeshProUGUI desTX;

        public void SetInit()
        {
        }

        public void ButtonClick()
        {
            switch (triviaUI)
            {
                case "Level_1":
                    GameManager.Instance.LevelStageUI().LoadNewScene("3_TowerDefense_Loading");
                    break;
                case "Level_4_Next":
                    GameManager.Instance.LevelStageUI().LoadNewScene("3_TowerDefense_Level4");
                    break;
                case "Level_4_Home":
                    GameManager.Instance.LevelStageUI().LoadNewScene("3_TowerDefense_Loading");
                    break;
            }
            this.gameObject.SetActive(false);
        }

    }
}
