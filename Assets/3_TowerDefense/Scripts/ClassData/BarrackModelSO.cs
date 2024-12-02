using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "BarrackModelSO", menuName = "TowerDefenese/BarrackModelSO", order = 1)]
    public class BarrackModelSO : ScriptableObject
    {
        public BarrackModel barrackModel;

        public BarrackType GetBarrackType()
        {
            return barrackModel.barrackType;
        }

        public FortressUpgrade GetFortressUpgrade(FortressUpgradeType _type)
        {
            return barrackModel.fortressUpgrades.FirstOrDefault(o => o.upgradeType == _type);
        }
    }
}
