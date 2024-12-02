using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TowerDefense_Prototype
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject[] enemyPrefabs;

        [Header("Attributes")]
        [SerializeField] private int baseEnemies = 8; //จำนวน Enemies
        [SerializeField] private float enemiesPerSecond = 0.5f; //ระยะเวลาการเกิด
        [SerializeField] private float timeBetweenWaves = 5f; //คลูดาวน์เวฟ
        [SerializeField] private float difficultyScalingFactor = 0.75f; //ศูตรูจะเพิ่มขึ้นเรื่อยๆ
        [SerializeField] private float enemiesPerSecondCap = 10f;

        [Header("Events")]
        public static UnityEvent onEnemyDestory = new UnityEvent();

        private int currentWave = 1;
        private float timeSinceLastSpawn;
        private int enemiesAlive;
        private int enemiesLeftToSpawn;
        private float eps; //enemies per secode;
        private bool isSpawning = false;

        private void Awake()
        {
            onEnemyDestory.AddListener(EnemyDestoryed);
        }

        public void Start()
        {
            BuildManager.main.Init();
            StartCoroutine(StartWave());
        }

        public void Update()
        {
            if (!isSpawning) return;

            timeSinceLastSpawn += Time.deltaTime;

            if (timeSinceLastSpawn >= (1f / eps) && enemiesLeftToSpawn > 0)
            {
                SpwanEnemy();
                enemiesLeftToSpawn--;
                enemiesAlive++;
                timeSinceLastSpawn = 0f;
            }

            if (enemiesAlive == 0 && enemiesLeftToSpawn == 0)
            {
                EndWave();
            }
        }

        private void EnemyDestoryed()
        {
            enemiesAlive--;
        }

        private IEnumerator StartWave()
        {
            yield return new WaitForSeconds(timeBetweenWaves);

            isSpawning = true;
            enemiesLeftToSpawn = EnemiesPerWave();
            eps = EnemiesPerSecond();
        }

        private void EndWave()
        {
            isSpawning = false;
            timeSinceLastSpawn = 0f;
            currentWave++;
            StartCoroutine(StartWave());
        }

        private void SpwanEnemy()
        {
            int rand = Random.Range(0, enemyPrefabs.Length);
            GameObject prefabToSpwan = enemyPrefabs[rand];
            Instantiate(prefabToSpwan, LevelManager.main.startPoint.position, Quaternion.identity);
            Debug.Log("Spwan Enemy");
        }

        private int EnemiesPerWave()
        {
            //ศูตรูจะเพิ่มขึ้นเรื่อยๆ
            return Mathf.RoundToInt(baseEnemies * Mathf.Pow(currentWave, difficultyScalingFactor));
        }

        private float EnemiesPerSecond()
        {
            //ศูตรูจะเพิ่มขึ้นเรื่อยๆ
            return Mathf.Clamp(enemiesPerSecond * Mathf.Pow(currentWave, difficultyScalingFactor) , 0f , enemiesPerSecondCap);
        }
    }
}
