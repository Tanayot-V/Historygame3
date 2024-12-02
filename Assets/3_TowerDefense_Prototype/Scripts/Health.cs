using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class Health : MonoBehaviour
    {
        [Header("Attributes")]
        [SerializeField] private int hitPoints = 2; //จะโจมตีเท่าไหร่
        [SerializeField] private int currencyWorth = 50;

        private bool isDestoryed = false;

        public void TakeDamage(int dmg)
        {
            hitPoints -= dmg;
            if (hitPoints <= 0 && !isDestoryed)
            {
                EnemySpawner.onEnemyDestory.Invoke();
                LevelManager.main.IncreaseCurrency(currencyWorth);
                isDestoryed = true; 
                Destroy(gameObject);
            }
        }
    }
}
