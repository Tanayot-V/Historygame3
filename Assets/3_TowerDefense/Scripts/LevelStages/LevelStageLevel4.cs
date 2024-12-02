using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefense
{
    public class LevelStageLevel4 : Singletons<LevelStageLevel4>
    {
        [SerializeField] EnemySpwan[] enemySpwans;
        [Header("General")]
        public BarrackSlot generalBarrack;
        public EntityMovement general;
        bool isSwpanGeneral = false;
        [SerializeField] GeneralSkill skillGenernal;

        [Header("Tower HP")]
        [SerializeField] TowerGateSlot entityTowerGateSlot;
        [SerializeField] GameObject enemyTowerUI;
        [SerializeField] TowerGateSlot enemyTowerGateSlot;

        //ก่อนกด Start
        public void InitStartStageA()
        {
            LevelStageSlot.Instance.stageModelSO.levelStageModel.baseType = BaseType.Defensive;
            LevelStageSlot.Instance.SetupTowerHP(500, 0);
            if (enemyTowerGateSlot != null) enemyTowerGateSlot.gameObject.SetActive(false);
            GameManager.Instance.GoldCost().IncreaseGold(70);
            //GameManager.Instance.GoldCost().IncreaseGold(99999);
        }

        //หลังกด Start
        public void InitStartStageB()
        {
            int index = 0;
            enemySpwans.ToList().ForEach(o => {
                o.Init(0, LevelStageSlot.Instance.stageModelSO.levelStageModel.enemyWaves);
                index++;
            });

            enemyTowerUI.SetActive(false);
        }
        public void Update()
        {
            /*
            if (Input.GetKeyDown(KeyCode.Space))
            {
                General();
            }
            
            if (entityTowerGateSlot.IsTowerHPPercent(0.15f))
            {
                if(!isSwpanGeneral) General();
            }*/
        }

        public void General()
        {
            if (generalBarrack.isGeneral) return;
            //Stop เพื่อน ShowCutsnece ก่อน
            Time.timeScale = 0;
            GameManager.Instance.LevelStageUI().cutscene.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
            GameManager.Instance.LevelStageUI().cutscene.SetActive(true);
            StartCoroutine(UiController.Instance.WaitForSecondRealtime(3, ()=> {
                GameManager.Instance.LevelStageUI().cutscene.SetActive(false);
                Time.timeScale = 1;
            }));
            //LevelStageSlot.Instance.stageModelSO.levelStageModel.baseType = BaseType.Offensive;
            //GameManager.Instance.ClickManager().ATKEnemyTown();
            general = generalBarrack.CreateGeneral(new Vector3(23,4)).GetComponent<EntityMovement>();
            isSwpanGeneral = true;

            if (enemyTowerGateSlot != null)
            {
                enemyTowerUI.SetActive(true);
                enemyTowerGateSlot.gameObject.SetActive(true);
            }

            skillGenernal.StartCooldown();
        }

        public void GeneralBack()
        {
            if (general == null) return;
            general.StopMoving();
            StartCoroutine(UiController.Instance.WaitForSecond(1, () => {
                Destroy(general.gameObject);
            }));
            /*
             * general.health = 0;
            general.GetComponent<SpineAnimationController>().DeadAnim();
            StartCoroutine(UiController.Instance.WaitForSecond(1, () => {
                Destroy(general.gameObject);
            }));*/
        }

        //ไม่ได้ใช้แล้วจ้า
        public void Victory()
        {
            LevelStageSlot.Instance.VictoryGame();
            enemyTowerGateSlot.gameObject.SetActive(false);
            UiController.Instance.DestorySlot(GameManager.Instance.EnemySpwan().spawnGO.gameObject);

            LevelStageSlot.Instance.StopStopAllSoliders(true);
            LevelStageSlot.Instance.SetActionSoliders(ActionEntity.Idle);
            Debug.Log("Victory Level_4");
        }
    }
}