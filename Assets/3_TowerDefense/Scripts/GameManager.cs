using System.Collections;
using System.Collections.Generic;
using TowerDefense_Prototype;
using UnityEngine;

namespace TowerDefense
{
    public class GameManager : Singletons<GameManager>
    {
        [Header("Data Dialogs")]
        [SerializeField] private BottomBarController bottomBarController;
        [SerializeField] private DialogController dialogController;
        [SerializeField] private DialogsData dialogsData;
        [SerializeField] private SpeakerData speakerData;

        [Header("Main Game")]
        [SerializeField] private AudioModelSO audioModelSO;
        [SerializeField] private UITransitionController uiTransitionController;
        [SerializeField] private LoadingPage loadingPage;
        [SerializeField] private LevelStageData levelStageData;
        [SerializeField] private LevelStageManager levelStageManager;
        [SerializeField] private LevelStageUI levelStageUI;
        [SerializeField] private ClickManager clickManager;
        [SerializeField] private EnemySpwan enemySpwan;
        [SerializeField] private GoldCost goldCost;

        [Header("Start Game")]
        public bool isStopStartgame;
        [SerializeField] public GeneralSkill generalSkill;
        [SerializeField] private StrongTogether StrongTogether;
        [SerializeField] private CannonSkill cannonSkill;

        public void Start()
        {
            SoundManager.Instance.Init(audioModelSO);
            SoundManager.Instance.PlayAudioSource("BGM"); 

            _StartA();

            LoadingPage().ShowStartLoading(LoadingPage().start_Prefab_Loading.GetComponent<FillAmountAnimation>().time, ()=> {
                _StartB();
            });

            void _StartA()
            {
                Debug.Log("_Start_1");
                LevelStageManager().levelStageSlot.LevelStartDefault();
                DialogController().DialogControllerInit();
                if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level4))
                {
                    levelStageUI.startGameUI.SetActive(true);
                }
                isStopStartgame = true;
            }

            void _StartB()
            {
                if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level1Tutorial))
                {
                    LevelStageManager().levelStageSlot.LevelStart();
                    DialogController().PlaySceneInit("Dialog_1");
                }
                Debug.Log("_Start");
            }
        }

        public void GameStartAfterClick()
        {
            isStopStartgame = false;
            generalSkill.StartCooldown();
        }

        public UITransitionController UITransitionController()
        {
            return DataCenterManager.GetData(ref uiTransitionController, "UITransitionController");
        }

        public LoadingPage LoadingPage()
        {
            return DataCenterManager.GetData(ref loadingPage, "main- LoadingPage");
        }

        public LevelStageData LevelStageData()
        {
            return DataCenterManager.GetData(ref levelStageData, "LevelStageData");
        }

        public LevelStageManager LevelStageManager()
        {
            return DataCenterManager.GetData(ref levelStageManager, "LevelStageManager");
        }

        public LevelStageUI LevelStageUI()
        {
            return DataCenterManager.GetData(ref levelStageUI, "LevelStageUI");
        }

        public ClickManager ClickManager()
        {
            return DataCenterManager.GetData(ref clickManager, "ClickManager");
        }

        public EnemySpwan EnemySpwan()
        {
            return DataCenterManager.GetData(ref enemySpwan, "*-* EnemySpwan");
        }

        public GoldCost GoldCost()
        {
            return DataCenterManager.GetData(ref goldCost, "GoldCost");
        }

        #region Dialog Data
        public DialogsData DialogsData()
        {
            return DataCenterManager.GetData(ref dialogsData, "DialogsData");
        }

        public DialogController DialogController()
        {
            return DataCenterManager.GetData(ref dialogController, "DialogController");
        }

        public SpeakerData SpeakerData()
        {
            return DataCenterManager.GetData(ref speakerData, "SpeakerData");
        }

        public BottomBarController BottomBarController()
        {
            return DataCenterManager.GetData(ref bottomBarController, "ButtomberController");
        }
        #endregion

    }
}
