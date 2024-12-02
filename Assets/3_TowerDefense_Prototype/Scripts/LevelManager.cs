using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager main;
        private void Awake()
        {
            main = this;
        }
        
        public Transform startPoint;
        public Transform[] path;

        public int currency;

        void Start()
        {
            currency = 500;
        }

        public void IncreaseCurrency(int amount)
        {
            currency += amount;
        }

        public bool SpendCurrency(int amount)
        {
            if (amount <= currency)
            {
                //Buy Item
                currency -= amount;
                return true;
            }
            else
            {
                Debug.Log("You do not have enough to purchase this item");
                return false;
            }
        }
    }
}

