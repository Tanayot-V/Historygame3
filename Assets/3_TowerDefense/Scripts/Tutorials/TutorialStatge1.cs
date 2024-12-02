using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class TutorialStatge1 : Singletons<TutorialStatge1>
    {
        public string currentTutorial;
        public LevelStageSlot levelStageSlot;
        public BarrackSlot barrack_SwordSlot;
        public FortressSlot fortressSlot_1;
        public EnemySpwan enemySpwan;

        [Header("InStage")]
        public EntityMovement entityMovement;
        public EnemyMovement enemyMovement;

        public void Start()
        { }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                //Skip Tutorial
                currentTutorial = "Tutorial_9";
                SkipTutprial();
            }
        }
        public void StartTutorial()
        {
            currentTutorial = "Tutorial_1";
            barrack_SwordSlot.gameObject.SetActive(true);
            barrack_SwordSlot.enabled = true;

            enemySpwan.gameObject.SetActive(false);
            enemySpwan.enabled = false;

            GameManager.Instance.LevelStageManager().levelStageSlot.SetupTowerHP(100000,100);
        }

        //Set Tutorial หลัง Dialogs
        public void SetTutorial(string _actionID)
        {
            currentTutorial = _actionID;
            switch (_actionID)
            {
                //สปอนศํตรูตัวแรก
                case "Tutorial_2":
                    enemySpwan.SetEnemySpwan(0);
                    enemySpwan.gameObject.SetActive(true);
                    enemySpwan.enabled = true;
                    break;
                //สปอนศํตรูชั้นดี
                case "Tutorial_2_1":
                    enemySpwan.SetEnemySpwan(1);
                    break;
                //อัพเกรด
                case "Tutorial_3":
                    TutorialUI.Instance.arrowUpgradeUI.SetActive(true);
                    GameManager.Instance.GoldCost().IncreaseGold(20);
                    break;
                //สอนกดบุก
                case "Tutorial_4":
                    //enemySpwan.SetEnemySpwan(2);
                    TutorialUI.Instance.arrowFightTownUI.SetActive(true);
                    break;
                case "Tutorial_5":
                    TutorialUI.Instance.arrowHpEnemyUI.SetActive(false);
                    enemySpwan.SetEnemySpwan(2);
                    break;
                //สอนทหารสามตัว
                case "Tutorial_7":
                    TutorialUI.Instance.arrowFortressUI.SetActive(true);
                    break;
                case "Tutorial_7_1":
                    barrack_SwordSlot.isStopSpawning = false;
                    TutorialUI.Instance.statusBarrackUI.SetActive(true);
                    barrack_SwordSlot.barrackState.soldierMaxStack = 3;
                    barrack_SwordSlot.spawnInterval = 3;
                    break;
                //รีเซ็ตขึ้นเริ่มเล่นเกม
                case "Tutorial_8":
                    GameManager.Instance.LevelStageUI().towerFightUI.SetActive(false);
                    TutorialUI.Instance.arrowFightTownUI.SetActive(true);
                    break;
                //รีเซ็ตขึ้นเริ่มเล่นเกม
                case "Tutorial_9":
                    GameManager.Instance.LevelStageUI().startGameUI.SetActive(true);
                    GameManager.Instance.LevelStageUI().towerFightUI.SetActive(false);
                    break;
                //หลังคัทซีนจอมทัพ
                case "Tutorial_10":
                    LevelStageSlot.Instance.SetStopAllEnemy(false);
                    LevelStageSlot.Instance.StopStopAllSoliders(false);
                    barrack_SwordSlot.isStopSpawning = false;
                    break;
            }
        }

        public void StartGameButton()
        {
            LevelStageManager levelStageManager = GameManager.Instance.LevelStageManager();
            //Reset Start Games
            levelStageManager.levelStageSlot.stageModelSO = GameManager.Instance.LevelStageData().levelStageModelSO[1];
            levelStageManager.currentLevelStageModelSO = GameManager.Instance.LevelStageData().levelStageModelSO[1];
            levelStageManager.levelStageSlot.stageModelSO.levelStageModel.baseType = BaseType.Defensive;
            UiController.Instance.DestorySlot(levelStageSlot.heroSpwan);

            enemySpwan.gameObject.SetActive(true);
            //enemySpwan.Init(0, levelStageManager.levelStageSlot.stageModelSO.levelStageModel.enemySpwanModels);
            enemySpwan.SetEnemySpwan(0);
            barrack_SwordSlot.InitBarrack();
            barrack_SwordSlot.barrackState.soldierMaxStack = 10;
            fortressSlot_1.SetDefault();
            levelStageManager.levelStageSlot.LevelStart();
            GameManager.Instance.LevelStageUI().barracknameTX.text = "ฐานนักดาบ Lv." + barrack_SwordSlot.barrackState.level;

            levelStageManager.levelStageSlot.SetupTowerHP(500, 500);
            GameManager.Instance.GoldCost().IncreaseGold(10);
        }

        public bool IsCurrentTutorialID(string _id)
        {
            return currentTutorial == _id;
        }

        public void Ganeral()
        {
            GameManager.Instance.LevelStageUI().cutscene.SetActive(true);
            LevelStageSlot.Instance.SetStopAllEnemy(true);
            LevelStageSlot.Instance.StopStopAllSoliders(true);
            //enemySpwan.isStopSpwan = true;
            enemySpwan.isSpawning = false;
            barrack_SwordSlot.isStopSpawning = true;
            StartCoroutine(UiController.Instance.WaitForSecond(3, () => {
                GameManager.Instance.LevelStageUI().cutscene.SetActive(false);
                GameManager.Instance.DialogController().PlaySceneInit("Dialog_9");
                LevelStageSlot.Instance.mainBarrack.barrackSlots[0].CreateGeneral(LevelStageSlot.Instance.mainBarrack.barrackSlots[0].transform.position);
            }));
        }

        public void SkipTutprial()
        {
            currentTutorial = "Tutorial_9";
            GameManager.Instance.LevelStageUI().towerFightUI.SetActive(false);
            if (!GameManager.Instance.LevelStageUI().statusTimeUI.activeSelf) GameManager.Instance.LevelStageUI().statusTimeUI.SetActive(true);
            if (GameManager.Instance.BottomBarController().dialogbar.activeSelf) GameManager.Instance.BottomBarController().dialogbar.SetActive(false);
            if (entityMovement != null) Destroy(entityMovement.gameObject);
            if (enemyMovement != null) Destroy(enemyMovement.gameObject);
            StartGameButton();
        }
    }
}
