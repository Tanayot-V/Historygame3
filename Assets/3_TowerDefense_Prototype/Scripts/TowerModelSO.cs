using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    [CreateAssetMenu(fileName = "TowerModelSO", menuName = "TowerDefenese/TowerModelSO", order = 1)]
    public class TowerModelSO : ScriptableObject
    {
        public Tower[] towers;

        public TowerModelSO(Tower[] _towers)
        {
            towers = _towers;
        }
    }
}
