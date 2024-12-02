using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    [System.Serializable]
    public class StagePath
    {
        public string id;
        public string comment;
        public Transform[] paths;
    }

    [System.Serializable]
    public class FortressEmpty
    {
        public FortressSlot fortressSlot;//ป้อมปราการที่วาง
        public FortressState state;//path ป้อมปราการที่ว่าง

        public FortressEmpty(FortressSlot _fortressSlot, FortressState _fortressState)
        {
            this.fortressSlot = _fortressSlot;
            this.state = _fortressState;
        }
    }

    public class LevelStageSlot :Singletons<LevelStageSlot>
    {
        public bool isTutorial { get; private set; }

        public LevelStageModelSO stageModelSO;
        public StagePath[] stagePath;

        public BarrackSlotManager mainBarrack;
        public GameObject healthGO;

        [Header("Enemy")]
        public GameObject enemySpwan;
        public GameObject healthEnemyGroup;
        public TowerGateSlot enemyTowerGate;

        [Header("Entity")]
        public GameObject heroSpwan;
        public GameObject healthEntityGroup;
        public TowerGateSlot entityTowerGate;

        public void LevelStartDefault()
        {
            GameManager.Instance.LevelStageUI().InitUIStart();
            if (IsStageLevelType(LevelType.Level1Tutorial))
            {
                isTutorial = true;
                TutorialStatge1.Instance.StartTutorial();
            }
            else isTutorial = false;
            mainBarrack.Init();
            UiController.Instance.DestorySlot(FortessOptionsUI.Instance.fortress_Quantity_parent);
            if (IsStageLevelType(LevelType.Level4))
            {
                LevelStageLevel4.Instance.InitStartStageA();
            }

        }

        public void LevelStart()
        {

            if (IsStageLevelType(LevelType.Level1Tutorial))
            {
                isTutorial = true;
            }
            else isTutorial = false;

            if (IsStageLevelType(LevelType.Level4))
            {
                LevelStageLevel4.Instance.InitStartStageB();
            }

        }

        public bool IsStageLevelType(LevelType _LevelType)
        {
            return stageModelSO.levelStageModel.levelType == _LevelType;
        }

        public void SetupTowerHP(int _entityHP, int _enemyHP)
        {
            if (enemyTowerGate != null)
            {
                enemyTowerGate.healthTower = _enemyHP;
                enemyTowerGate.InitTowerGate();
                enemyTowerGate.SetTowerHP();
            }

            if (entityTowerGate != null)
            {
                entityTowerGate.healthTower = _entityHP;
                entityTowerGate.InitTowerGate();
                entityTowerGate.SetTowerHP();
            }
        }

        #region StopAllCharacter
        //หยุดการเคลื่อนไหวของศัตรู
        public void SetStopAllEnemy(bool _isStop)
        {
            if (enemySpwan.transform.childCount > 0)
            {
                foreach (Transform child in enemySpwan.transform)
                {
                    EnemyMovement enemy = child.gameObject.GetComponent<EnemyMovement>();
                    if (enemy != null)
                    {
                        enemy.isStopAttack = _isStop;
                        if (!_isStop) enemy.ResetSpeed();
                    }
                }
                if(_isStop) SetActionSoliders(ActionEntity.Idle);
            }
        }

        //หยุดการเคลื่อนไหวของทหาร
        public void StopStopAllSoliders(bool _isStop)
        {
            if (heroSpwan.transform.childCount > 0)
            {
                foreach (Transform child in heroSpwan.transform)
                {
                    EntityMovement solider = child.gameObject.GetComponent<EntityMovement>();
                    if (solider != null)
                    {
                        solider.isStopAttack = _isStop;
                        if (!_isStop) solider.ResetSpeed();
                    }
                }
                if (_isStop) SetActionSoliders(ActionEntity.Idle);
            }
        }

        public void SetActionSoliders(ActionEntity _actionEntity)
        {
            if (heroSpwan.transform.childCount > 0)
            {
                foreach (Transform child in heroSpwan.transform)
                {
                    EntityMovement solider = child.gameObject.GetComponent<EntityMovement>();
                    if (solider != null)
                    {
                        solider.actionEntity = _actionEntity;
                        if (_actionEntity == ActionEntity.Idle)
                        {
                            solider.anim.Idle();
                            solider.StopMoving();
                        }
                    }
                }
            }
        }
        #endregion

        #region Paths
        private Dictionary<string, Transform[]> stagePathDic = new Dictionary<string, Transform[]>();
        public Transform[] GetPathTransforms(string id)
        {
            if (stagePathDic.ContainsKey(id))
            {
                return stagePathDic[id];
            }
            else
            {
                return stagePathDic[id] = stagePath.ToList().Find(o => o.id == id).paths;
            }
        }
        #endregion

        #region Summary
        public void VictoryGame()
        {
            if (GameManager.Instance.LevelStageUI().loseUI.activeSelf) return;
            if (GameManager.Instance.LevelStageUI().victoryUI.activeSelf) return;

            GameManager.Instance.LevelStageUI().victoryUI.SetActive(true);
            Time.timeScale = 0;
            if (GameManager.Instance.BottomBarController().dialogbar.activeSelf)
            {
                GameManager.Instance.BottomBarController().Hide();
            }
            SoundManager.Instance.PlayAudioSource("Victory");
        }

        public void LoseGame()
        {
            if (GameManager.Instance.LevelStageUI().loseUI.activeSelf) return;
            if (GameManager.Instance.LevelStageUI().victoryUI.activeSelf) return;

            UiController.Instance.DestorySlot(heroSpwan);
            UiController.Instance.DestorySlot(GameManager.Instance.EnemySpwan().spawnGO.gameObject);

            GameManager.Instance.LevelStageUI().loseUI.SetActive(true);
            Time.timeScale = 0;
            if (GameManager.Instance.BottomBarController().dialogbar.activeSelf)
            {
                GameManager.Instance.BottomBarController().Hide();
            }
            SoundManager.Instance.PlayAudioSource("Lose");
        }
        #endregion
    }
}
