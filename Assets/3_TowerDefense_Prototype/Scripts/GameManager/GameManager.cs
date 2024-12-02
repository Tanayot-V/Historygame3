using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class GameManager : Singletons<GameManager>
    {
        [SerializeField] private AudioModelSO audioModelSO;
        [SerializeField] private UITransitionController uiTransitionController;
        [SerializeField] private LoadingPage loadingPage;
        [SerializeField] private SpineSkinData spineSkinData;

        public void Start()
        {
            SoundManager.Instance.Init(audioModelSO);
            SoundManager.Instance.PlayAudioSource("BGM_1");
            UITransitionController();
            spineSkinData.InitData();
            //LoadingPage().ShowMiniLoading(3);
        }

        public UITransitionController UITransitionController()
        {
            return DataCenterManager.GetData(ref uiTransitionController, "UITransitionController");
        }

        public LoadingPage LoadingPage()
        {
            return DataCenterManager.GetData(ref loadingPage, "main- LoadingPage");
        }

        public SpineSkinData SpineSkinData()
        {
            return DataCenterManager.GetData(ref spineSkinData, "SpineSkinData");
        }
    }
}
