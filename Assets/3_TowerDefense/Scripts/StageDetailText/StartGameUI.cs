using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class StartGameUI : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshProUGUI desTX;
        [SerializeField] string description;

        public void ButtonStart()
        {
            if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level1Tutorial))
            {
                TutorialStatge1.Instance.StartGameButton();
            }
            else if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level4))
            {
                GameManager.Instance.LevelStageManager().levelStageSlot.LevelStart();
            }
        }
    }
}
