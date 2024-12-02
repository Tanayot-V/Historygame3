using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class SummaryUI : MonoBehaviour
    {
        public string summaryID;
        public void ButtonClick()
        {
            switch (summaryID)
            {
                case "Level1_Win":
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_10");
                    break;
                case "Level1_Lose":
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_11");
                    break;
                case "Level4_Win":
                    Time.timeScale = 1;
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_1");
                    break;
                case "Level4_Lose":
                    Time.timeScale = 1;
                    GameManager.Instance.DialogController().PlaySceneInit("Dialog_2");
                    break;
            }
            this.gameObject.SetActive(false);
        }
    }
}
