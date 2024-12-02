using System.Collections;
using System.Collections.Generic;
using TowerDefense_Prototype;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public enum TowerType
    {
        EntityTower,
        EnemyTower
    }

    public class TowerGateSlot : MonoBehaviour
    {
        public TowerType towerType;
        public int healthTower;
        public int healthTowerMax;
        public Image imgFillTower;
        public TMPro.TextMeshProUGUI healthTX;
        public bool isStopHealth;

        public void InitTowerGate()
        {
            healthTowerMax = healthTower;
            healthTX.text = healthTower + "/" + healthTowerMax;
        }

        bool isVictory;
        public void TakeDamageTower(int _damage)
        {
            if (isStopHealth) _damage = 0;

            healthTower -= _damage;

            if (healthTower < 0)
            {
                healthTower = 0;
                if (towerType == TowerType.EnemyTower)
                {
                    /*
                    if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level4))
                    {
                        if (isVictory) return;
                        LevelStageLevel4.Instance.Victory();
                        isVictory = true;
                    }*/
                    if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level4)) return;

                    LevelStageSlot.Instance.VictoryGame();
                    LevelStageSlot.Instance.mainBarrack.barrackSlots[0].isStopSpawning = true;
                    GameManager.Instance.EnemySpwan().isSpawning = false;

                    UiController.Instance.DestorySlot(LevelStageSlot.Instance.heroSpwan);
                    UiController.Instance.DestorySlot(GameManager.Instance.EnemySpwan().spawnGO.gameObject);
                }
                if (towerType == TowerType.EntityTower)
                {
                    LevelStageSlot.Instance.LoseGame();
                }
                return;
            }

            SetTowerHP();
        }

        public void SetTowerHP()
        {
            float fillValue = (float)healthTower / healthTowerMax;  // Assuming max health is 10
            imgFillTower.fillAmount = fillValue;

            healthTX.text = healthTower + "/" + healthTowerMax;
        }

        public bool IsTowerHPPercent(float _healthThreshold)
        {
            if (healthTower <= healthTowerMax * _healthThreshold) isStopHealth = true;
            return isStopHealth;
        }
    }
}
