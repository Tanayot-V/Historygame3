using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public enum TowerType
    {
        generic,
        green,
        Red,
        Yellow
    }

    [System.Serializable]
    public class Tower
    {
        public string name;
        public int cost;
        public GameObject prefab;
        public TowerType towerType;

        public Tower(string _name, int _cost, GameObject _prefab, TowerType _towerType)
        {
            name = _name;
            cost = _cost;
            prefab = _prefab;
        }
    }
}
