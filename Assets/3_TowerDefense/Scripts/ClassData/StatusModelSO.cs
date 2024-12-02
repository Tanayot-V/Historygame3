using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "StatusModelSO", menuName = "TowerDefenese/StatusModelSO", order = 1)]
    public class StatusModelSO : ScriptableObject
    {
        [Header("Enemy")]
        public EnemyStatusModel[] enemyStatusModels;

        [Header("Entity")]
        public EntityStatus general;
        public BarrackUpgrade[] barrackUpgradeSword;
        public BarrackUpgrade[] barrackUpgradeGun;
        public BarrackUpgrade[] barrackUpgradeMerchant;
        public BarrackUpgrade[] barrackUpgradeMonk;

        public EnemyStatusModel GetEnemyStatusModel(EnemyType _enemyType)
        {
            return enemyStatusModels
                .FirstOrDefault(model => model.enemyType == _enemyType);
        }

        public EnemyStatus GetEnemyStatus(EnemyType _enemyType,EntityComboType _comboType)
        {
            return enemyStatusModels
                .FirstOrDefault(model => model.enemyType == _enemyType)?
                .status.FirstOrDefault(status => status.comboType == _comboType);
            /*
            EnemyStatus enemyStatus = null;
            enemyStatusModels.ToList().ForEach(o => {
                if (_enemyType == o.enemyType) 
                {
                    o.status.ToList().ForEach(i =>
                    {
                        if (_comboType == i.comboType) enemyStatus = i;
                    });
                }
            });
            return enemyStatus;
            */
        }

        #region Upgrade
        public BarrackUpgrade[] GetUpgrade(BarrackType _barrackType)
        {
            BarrackUpgrade[] barrackUpgrades = null;
            switch (_barrackType)
            {
                case BarrackType.SoldierSword:
                    barrackUpgrades = barrackUpgradeSword;
                    break;
                case BarrackType.SoldierGun:
                    barrackUpgrades = barrackUpgradeGun;
                    break;
                case BarrackType.Merchant:
                    barrackUpgrades = barrackUpgradeMerchant;
                    break;
                case BarrackType.Monk:
                    barrackUpgrades = barrackUpgradeMonk;
                    break;
            }
            return barrackUpgrades;
        }

        public BarrackUpgrade GetBarrackUpgrade(BarrackUpgrade[] _upgradeType, int level)
        {
            BarrackUpgrade upgrade = null;
            _upgradeType.ToList().ForEach(o => {
                if (level == o.level)
                {
                    upgrade = o;
                }
            });
            return upgrade;
        }

        public bool IsCanUpgrade(BarrackUpgrade[] _upgradeType,int _currentLevel, int _currentGold)
        {
            if (_currentLevel >= 3) return false;
            if (_currentGold >= _upgradeType[_currentLevel].costUpgrade)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
