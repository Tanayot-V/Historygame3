using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class FortressSelectSlotUI : MonoBehaviour
    {
        public BarrackModelSO barrackModelSO;
        [SerializeField] Image img_BG;
        [SerializeField] Image img_Icon;
        [SerializeField] TMPro.TextMeshProUGUI text_Name;
        [SerializeField] TMPro.TextMeshProUGUI text_Gold;
        bool isCanBuild;
        int costFortress;

        public void InitUI(BarrackModelSO _barrackModelSO)
        {
            barrackModelSO = _barrackModelSO;
            if (barrackModelSO != null)
            {
                BarrackModel barrackModel = _barrackModelSO.barrackModel;
                img_Icon.sprite = barrackModel.fortressUISP;
                costFortress = barrackModel.costFortress;
                isCanBuild = GameManager.Instance.GoldCost().IsMoreThanCurrent(costFortress);
                text_Name.text = barrackModel.fortressName;
                text_Gold.text = barrackModel.costFortress.ToString();

                if (isCanBuild)
                {
                    img_BG.GetComponent<Image>().color = Color.white;
                    this.GetComponent<CanvasGroup>().alpha = 1;
                }
                else
                {
                    img_BG.GetComponent<Image>().color = Color.gray;
                    this.GetComponent<CanvasGroup>().alpha = 0.5f;
                }
            }
        }

        public void Button()
        {
            SoundManager.Instance.PlayAudioSource("Click");
            if (isCanBuild)
            {
                GameManager.Instance.GoldCost().ReduceGold(costFortress);
                GameManager.Instance.LevelStageUI().selectFortessUI.BarrackTypeButton((int)barrackModelSO.GetBarrackType());
            }
            else Debug.Log("Can't Build."); return;
        }
    }
}
