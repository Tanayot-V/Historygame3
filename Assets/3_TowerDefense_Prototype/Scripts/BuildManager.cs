using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class BuildManager : MonoBehaviour
    {
        public static BuildManager main;

        private void Awake()
        {
            main = this;
        }

        [Header("Referencens")]
        [SerializeField] TowerModelSO towerModelSO;
        [SerializeField] private Tower[] towers;

        private int selectedTower = 0;

        public void Init()
        {
            towers = towerModelSO.towers;
        }

        public Tower GetSlectedTower()
        {
            return towers[selectedTower];
        }

        public void SetSelectedTower(int _selectTower)
        {
            selectedTower = _selectTower;
        }
    }
}
