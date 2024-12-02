using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
//using static TowerDefense.EnemySpwanModel;

namespace TowerDefense
{
    [System.Serializable]
    public class WaveData
    {
        public List<EnemyGroup> enemyGroups;
        public List<int> currentSpawnerIndexLists;
        public int currentEnemyGroupIndex;

        public List<int> GetAllSpawnPoints()
        {
            return enemyGroups.SelectMany(group => group.spawnerIndexLists).Distinct().ToList();
        }
        public List<int> GetSpawnPoints()
        {
            //Debug.Log("Get Spawnpoint");
            List<int> temp = new List<int>();
            if(enemyGroups[currentEnemyGroupIndex].isSpawnAllSpawnPoint || enemyGroups[currentEnemyGroupIndex].isFixQuantity)
            {
                for(int i = 0; i < enemyGroups[currentEnemyGroupIndex].spawnerIndexLists.Count; i++)
                {
                    temp.Add(enemyGroups[currentEnemyGroupIndex].spawnerIndexLists[i]);
                }
                currentSpawnerIndexLists = enemyGroups[currentEnemyGroupIndex].spawnerIndexLists;
            }
            else if(enemyGroups[currentEnemyGroupIndex].isRandom)
            {
                int r = Random.Range(0,enemyGroups[currentEnemyGroupIndex].spawnerIndexLists.Count);
                temp.Add(enemyGroups[currentEnemyGroupIndex].spawnerIndexLists[r]);
                currentSpawnerIndexLists.Clear();
                currentSpawnerIndexLists.Add(enemyGroups[currentEnemyGroupIndex].spawnerIndexLists[r]);
            }
            else if(enemyGroups[currentEnemyGroupIndex].isContinue)
            {
                for(int i = 0; i < currentSpawnerIndexLists.Count; i++)
                {
                    temp.Add(currentSpawnerIndexLists[i]);
                }
            }
            else if(enemyGroups[currentEnemyGroupIndex].isExceptPreviousSpawnPoint)
            {
                List<int> ti = new List<int>();
                for (int i = 0; i < enemyGroups[currentEnemyGroupIndex].spawnerIndexLists.Count; i++)
                {
                    ti.Add(enemyGroups[currentEnemyGroupIndex].spawnerIndexLists[i]);
                    //Debug.Log($"ti {i} : {ti[i]}");
                }
                for (int i = 0; i < currentSpawnerIndexLists.Count; i++)
                {
                    //Debug.Log($"remove from ti : {currentSpawnerIndexLists[i]}");
                    ti.Remove(currentSpawnerIndexLists[i]);
                }
                int r = Random.Range(0,ti.Count);
                //Debug.Log($"Random : {r} from 0-{ti.Count-1}");

                temp.Add(ti[r]);
                currentSpawnerIndexLists.Clear();
                currentSpawnerIndexLists.Add(ti[r]);
            }
            //Debug.Log($"Get Spawnpoint Done : {temp.Count}");
            return temp;
        }
    }
    [System.Serializable]
    public class EnemyGroup
    {
        [Tooltip("List of index spawn Point Used\n=====link with spawnPoint in WaveData=====")]
        public List<int> spawnerIndexLists; //What spawner used
        public bool isFixQuantity;
        [Tooltip("=====If fix Quantity=====\nList of Quantity per index spawn Point Used\n=====Link with spawnerIndexLists!=====")]
        public List<int> quantityPerPointIndex;
        public bool isRandom;
        public bool isContinue;
        public bool isExceptPreviousSpawnPoint;
        public bool isSpawnAllSpawnPoint;
        public bool includeRebellion;
        public GameObject prefab;
        public int quantity;
    }
    public class EnemySpwan : MonoBehaviour
    {
        [Header("EnemyModel")]
        private List<WaveData> waveDatas;
        private List<bool> isWaveFinish;
        public int currentWaveIndex;

        //public Vector3 spawnPos;
        public Transform spawnGO;

        [Header("Spawning")]
        public List<EnemySpawnPoint> enemySpawnPoints; 
        public List<TownShip> rebellionTownShip;
        public bool isSpawning;

        public Image waveImageFill;

        public void Init(int _index, List<WaveData> datas)
        {
            //Debug.Log($"<color=cyan>EnemySpawn Init with {datas.Count} Wave</color>");
            waveDatas = datas;
            SetEnemySpwan(_index);
            isWaveFinish = new List<bool>(new bool[datas.Count]);
            //Debug.Log($"<color=cyan>Wave finish bool size : {isWaveFinish.Count}</color>");
            waveImageFill.fillAmount = 0;
        }

        public void SetEnemySpwan(int _index)
        {
            currentWaveIndex = _index;
            List<int> allSpawnPointsIndex = waveDatas[currentWaveIndex].GetAllSpawnPoints();
            for (int i = 0; i < enemySpawnPoints.Count; i++)
            {
                enemySpawnPoints[i].ShowIncomingIcon(allSpawnPointsIndex.Contains(i));
            }

            if (LevelStageSlot.Instance.isTutorial) GameManager.Instance.LevelStageUI().waveTX.text = "TUTORIAL";
            else GameManager.Instance.LevelStageUI().waveTX.text = "Stage " + (currentWaveIndex + 1)+"/10";

        }

        public void StartBtnClick()
        {
            StartCoroutine(StartWaveProgress());
        }

        void Start()
        {
            isSpawning = false;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                foreach (Transform child in LevelStageSlot.Instance.healthEnemyGroup.transform)
                {
                    Destroy(child.gameObject);
                }
                foreach (Transform child in spawnGO)
                {
                    Destroy(child.gameObject);
                }
            }

            if(Input.GetKeyDown(KeyCode.N))
            {
                YouWin();
            }
        
            if (!isSpawning) return;

            if(isWaveFinish[currentWaveIndex])
            {
                if (spawnGO.childCount <= 0)
                {
                    OnAllEnemiesInWaveDie();
                }
            }
        }

        private void General()
        {
            if(LevelStageLevel4.Instance.generalBarrack.isGeneral)
            {
                LevelStageLevel4.Instance.GeneralBack();
            }

            //Debug.Log($"<color=Yellow>General Die!!!!!!!!</color>");
        }

        private IEnumerator StartWaveProgress()
        {
            //Debug.Log("Start Spawning!!!");
            if(isSpawning) yield break;
            isSpawning = true;
            while (currentWaveIndex < waveDatas.Count)
            {
                yield return null;
                if(!isSpawning) yield break;
                if(isWaveFinish[currentWaveIndex]) continue;
                yield return new WaitForSeconds(3);
                waveDatas[currentWaveIndex].currentEnemyGroupIndex = 0;
                while (waveDatas[currentWaveIndex].currentEnemyGroupIndex < waveDatas[currentWaveIndex].enemyGroups.Count)
                {
                    int cegit = waveDatas[currentWaveIndex].currentEnemyGroupIndex; // cegit = Current Enemy Group Index Temp
                    int spawncont = 0;
                    GameObject original = waveDatas[currentWaveIndex].enemyGroups[cegit].prefab;
                    List<int> SpawnPointIndexs = waveDatas[currentWaveIndex].GetSpawnPoints();
                    //Debug.Log($"GetSpawnPoints : {SpawnPointIndexs.Count}");
                    List<EnemySpawnPoint> spawnPointListT = new List<EnemySpawnPoint>();
                    for (int i = 0; i < SpawnPointIndexs.Count; i++)
                    {
                        spawnPointListT.Add(enemySpawnPoints[SpawnPointIndexs[i]]);
                    }
                    //Debug.Log( $"<color=yellow>Wave {currentWaveIndex} Group {cegit} Include Rebellion = {waveDatas[currentWaveIndex].enemyGroups[cegit].includeRebellion}</color>");
                    if(waveDatas[currentWaveIndex].enemyGroups[cegit].includeRebellion)
                    {
                        for (int i = 0; i < rebellionTownShip.Count; i++)
                        {
                            if(rebellionTownShip[i].isRebellion)
                            {
                                //Debug.Log($"<color=red>Rebellion Add to SpawnList</color>");
                                spawnPointListT.Add(rebellionTownShip[i].mySpawnpoint);
                            }
                        }
                    }

                    float waitTime = 1f / waveDatas[currentWaveIndex].enemyGroups[cegit].quantity;
                    while (spawncont < waveDatas[currentWaveIndex].enemyGroups[cegit].quantity)
                    {
                        WalkPath wp = null;
                        for (int i = 0; i < spawnPointListT.Count; i++)
                        {
                            if(spawncont == 0)
                            {
                                wp = spawnPointListT[i].GetRandomWalkPath();
                            }
                            else
                            {
                                wp = spawnPointListT[i].GetContinueWalkPath();
                            }
                            CreateEnemy(original,wp.waypoints[0].position,wp.waypoints.ToArray());
                            if(waveDatas[currentWaveIndex].enemyGroups[cegit].isFixQuantity)
                            {
                                spawncont++;
                            }
                        }
                        yield return new WaitForSeconds(waitTime);
                        if(!waveDatas[currentWaveIndex].enemyGroups[cegit].isFixQuantity)
                        {
                            spawncont++;
                        }
                    }                    
                    waveDatas[currentWaveIndex].currentEnemyGroupIndex++;
                    //Debug.Log($"<color=green>End Enemy Group : {waveDatas[currentWaveIndex].currentEnemyGroupIndex}</color>");
                }
                isWaveFinish[currentWaveIndex] = true;
            }
        }

        private void YouWin()
        {
            LevelStageSlot.Instance.VictoryGame();
            //Debug.Log($"<color=green>YOU WIN!!!!!!!!</color>");
        }

        private void OnAllEnemiesInWaveDie()
        {
            currentWaveIndex++;
            if(currentWaveIndex >= waveDatas.Count)
            {
                for (int i = 0; i < enemySpawnPoints.Count; i++)
                {
                    enemySpawnPoints[i].ShowIncomingIcon(false);
                }
                isSpawning = false;
                YouWin();
                return;
            }
            GameManager.Instance.LevelStageUI().waveTX.text = "Stage " + (currentWaveIndex + 1)+"/10";
            waveImageFill.fillAmount = (currentWaveIndex + 1f) / 10f;
            List<int> allSpawnPointsIndex = waveDatas[currentWaveIndex].GetAllSpawnPoints();
            for (int i = 0; i < enemySpawnPoints.Count; i++)
            {
                enemySpawnPoints[i].ShowIncomingIcon(allSpawnPointsIndex.Contains(i));
            }
            General();
        }

        public void CreateEnemy(GameObject enemyPrefabs,Vector3 spawnPos, Transform[] enemyPath)
        {
            //if (currentEnemyIndex < enemyPrefabs.Count && currentEnemyIndex < quantityMax)
            //{
                // สร้างศัตรูตามลำดับในลิสต์
                GameObject enemy = Instantiate(enemyPrefabs, spawnPos, Quaternion.identity);
                enemy.GetComponent<EnemyMovement>().path = enemyPath;
                enemy.GetComponent<EnemyMovement>().InitStart();
                enemy.transform.SetParent(spawnGO);

                GameObject healthFill = Instantiate(LevelStageSlot.Instance.healthGO, transform.position ,Quaternion.identity);
                healthFill.transform.SetParent(LevelStageSlot.Instance.healthEnemyGroup.transform);
                healthFill.GetComponent<RectTransform>().localScale = Vector3.one;
                healthFill.GetComponent<HealthFillView>().SetInit(enemy.transform, enemy.GetComponent<EnemyMovement>().health, enemy.GetComponent<EnemyMovement>().status.hp, "enemy");

                enemy.GetComponent<EnemyMovement>().healthFillView = healthFill.GetComponent<HealthFillView>();
                //currentEnemyIndex++;

                #region Tutorial Event
                //AddTutorial
                if (LevelStageSlot.Instance.IsStageLevelType(LevelType.Level1Play))
                {
                    if (enemy.GetComponent<EnemyMovement>().spineSkinModelSO.IsSpineID("Enemy_General"))
                    {
                        //wave ที่4 ปล่อยจอมพล
                        //Debug.Log("ปล่อยจอมพล!!");
                        TutorialStatge1.Instance.Ganeral();
                    }
                }

                if (LevelStageSlot.Instance.isTutorial)
                {
                    switch (TutorialStatge1.Instance.currentTutorial)
                    {
                        case "Tutorial_2":
                            enemy.AddComponent<TutorialCharacter>().currentID = "Enemy_Lv1_G1";
                            TutorialStatge1.Instance.enemyMovement = enemy.GetComponent<EnemyMovement>();
                            break;
                        case "Tutorial_2_1":
                            enemy.AddComponent<TutorialCharacter>().currentID = "Enemy_Lv2_G1";
                            TutorialStatge1.Instance.enemyMovement = enemy.GetComponent<EnemyMovement>();
                            TutorialStatge1.Instance.entityMovement.health = 3;
                            break;
                        case "Tutorial_5":
                            enemy.AddComponent<TutorialCharacter>().currentID = "Enemy_Lv2_G3";
                            TutorialStatge1.Instance.enemyMovement = enemy.GetComponent<EnemyMovement>();
                            if (TutorialStatge1.Instance.entityMovement != null)
                            {
                                TutorialStatge1.Instance.entityMovement.health = 3;
                                TutorialStatge1.Instance.entityMovement.isStopAttack = false;
                            }
                            break;
                    }
                }
                #endregion
        }                
    }
}
