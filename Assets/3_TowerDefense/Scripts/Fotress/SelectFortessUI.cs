using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace TowerDefense
{
    public class SelectFortessUI : MonoBehaviour
    {
        [Header("Reference")]
        public GameObject selectFortessUI;
        public GameObject selectUIPrarent;
        public GameObject[] prefabSelectUI;

        [Header("State")]
        public FortressSlot currentFortressSlot;
        bool isShow;

        public void Show(FortressSlot _fortressSlot)
        {
            if (isShow) return;
            currentFortressSlot = _fortressSlot;

            bool subFortressSlot = currentFortressSlot.GetComponent<SubFortressSlot>() != null;
            int emptySlot = 0;
            UiController.Instance.DestorySlot(selectUIPrarent);
            if (subFortressSlot)
            {
                SubBarrackSlot[] subBarrackSlots = currentFortressSlot.GetComponent<SubFortressSlot>().subBarrackSlotManager.subBarrackSlots;
                subBarrackSlots.ToList().ForEach(o =>
                {
                    GameObject uiObj = Instantiate(prefabSelectUI[0]);
                    uiObj.transform.SetParent(selectUIPrarent.transform);
                    uiObj.GetComponent<FortressSelectSlotUI>().InitUI(o.referenceBarrack.barrackModelSO);
                    uiObj.GetComponent<RectTransform>().localScale = Vector3.one;
                });
                emptySlot = 4 - subBarrackSlots.Length;
            }
            else
            {
                BarrackSlot[] barrackSlots = LevelStageSlot.Instance.mainBarrack.barrackSlots;
                barrackSlots.ToList().ForEach(o =>
                {
                    GameObject uiObj = UiController.Instance.InstantiateUIView(prefabSelectUI[0], selectUIPrarent);
                    uiObj.GetComponent<FortressSelectSlotUI>().InitUI(o.barrackModelSO);
                });
                emptySlot = 4 - barrackSlots.Length;
            }

            for (int i = 0; i < emptySlot; i++)
            {
                GameObject uiObj = Instantiate(prefabSelectUI[1]);
                uiObj.transform.SetParent(selectUIPrarent.transform);
            }
            GameManager.Instance.UITransitionController().FortressOptionUIShow(selectFortessUI);
            isShow = true;
            if (LevelStageSlot.Instance.isTutorial)
            {
                if (TutorialUI.Instance.arrowFortressUI.activeSelf) TutorialUI.Instance.arrowFortressUI.SetActive(false);
                if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_7"))
                {
                    TutorialUI.Instance.arrowSelectFortressUI.SetActive(true);
                }
            }
        }

        public void Hide()
        {
            if (LevelStageSlot.Instance.isTutorial)
            {
                if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_7"))
                {
                    if (TutorialStatge1.Instance.fortressSlot_1.fortressType == BarrackType.Empty) return;
                }
                if (TutorialUI.Instance.arrowFortressUI.activeSelf) TutorialUI.Instance.arrowFortressUI.SetActive(false);
                if (TutorialUI.Instance.arrowSelectFortressUI.activeSelf) TutorialUI.Instance.arrowSelectFortressUI.SetActive(false);
            }

            currentFortressSlot = null;
            UiController.Instance.DestorySlot(selectUIPrarent);
            GameManager.Instance.UITransitionController().FortressOptionUIHide(selectFortessUI);

            StartCoroutine
            (UiController.Instance.WaitForSecond(0.15f, () => {
                isShow = false;
            }));
        }

        public void BarrackTypeButton(int index)
        {
            if (currentFortressSlot == null) return;
            currentFortressSlot.SetFortress((BarrackType)index);
            Hide();

            if (LevelStageSlot.Instance.isTutorial)
            {
                if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_7"))
                {
                    TutorialStatge1.Instance.SetTutorial("Tutorial_7_1");
                }
            }
        }
    }
}
