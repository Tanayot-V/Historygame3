using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TowerDefense;
using TowerDefense_Prototype;
using Unity.Mathematics;
using UnityEngine;
namespace TowerDefense
{
    public class TownShip : MonoBehaviour
    {
        public bool isRebellion;
        private bool isSetup;

        public GameObject soldierPrefab;
        public GameObject enemyPrefab;
        public BarrackUpgrade upgradeCurrent;


        public Transform[] standByT; // also parent
        private int indexStadByT;
        public GameObject quantityChild;
        public int quantityMax = 3;
        private List<EntityMovement> entityInTown;
        private List<EnemyMovement> enemyInTown;

        private SpriteRenderer townShipImage;
        public Sprite lightSprite;
        public Sprite fogSprite;

        public EnemySpawnPoint mySpawnpoint;

        public GameObject rebellionFxPrefab;
        public GameObject lightFxPrefab;


        // Start is called before the first frame update
        void Start()
        {
            upgradeCurrent = GameManager.Instance.LevelStageData().statusModelSO.GetUpgrade(BarrackType.SoldierSword)[0];
            StartCoroutine(SetUp());
            townShipImage = transform.GetChild(1).GetComponent<SpriteRenderer>();
            entityInTown = new List<EntityMovement>();
            enemyInTown = new List<EnemyMovement>();
        }

        public IEnumerator SetUp()
        {
            yield return new WaitForSeconds(3);
            while (true)
            {
                if (indexStadByT > 2) indexStadByT = 0;
                if (quantityChild.transform.childCount < quantityMax)
                {
                    CreateSoldier(isRebellion? enemyPrefab: soldierPrefab);
                    isSetup = true;
                    indexStadByT++;
                    if (indexStadByT > 2) indexStadByT = 0;
                }
                yield return new WaitForSeconds(1);
            }
        }

        public void CreateSoldier(GameObject prefab)
        {
            GameObject soldier = Instantiate(prefab, transform.position, Quaternion.identity);
            //soldier.GetComponent<EntityMovement>().barrackSlot = this;
            Transform[] paths = new Transform[]{
                transform,
                standByT[indexStadByT]
            };
            
            if(isRebellion)
            {
                EnemyMovement em = soldier.GetComponent<EnemyMovement>();
                em.path = paths;
                em.isTownShip = true;
                //em.SetStatus();
                em.InitStart();
                GameObject healthFill = Instantiate(LevelStageSlot.Instance.healthGO, transform.position ,Quaternion.identity);
                healthFill.transform.SetParent(LevelStageSlot.Instance.healthEnemyGroup.transform);
                healthFill.GetComponent<RectTransform>().localScale = Vector3.one;
                healthFill.GetComponent<HealthFillView>().SetInit(em.transform, em.health, em.status.hp, "enemy");

                em.healthFillView = healthFill.GetComponent<HealthFillView>();
                
                enemyInTown.Add(em);
                
                em.OnDieEvent.AddListener(() => {
                    enemyInTown.Remove(em);
                });
            }
            else
            {
                EntityMovement em = soldier.GetComponent<EntityMovement>();
                soldier.GetComponent<PathController>().AddTransforms(paths);
                em.SetBaseStatus(upgradeCurrent,0);
                em.id = soldier.name;
                soldier.name = "TownShip_" + DataCenterManager.GenerateID();
                em.Init();
                em.InitStatus();
                em.SetRunPath(soldier.GetComponent<PathController>().GetPath());
                entityInTown.Add(em);
                
                em.OnDieEvent.AddListener(() => {
                    entityInTown.Remove(em);
                });

                #region healthFillView
                GameObject healthFill = UiController.Instance.InstantiateUIView(LevelStageSlot.Instance.healthGO, LevelStageSlot.Instance.healthEntityGroup);
                healthFill.GetComponent<HealthFillView>().SetInit(soldier.transform, soldier.GetComponent<EntityMovement>().health, soldier.GetComponent<EntityMovement>().baseStatus.hp, "entity");
                soldier.GetComponent<EntityMovement>().healthFillView = healthFill.GetComponent<HealthFillView>();
                #endregion
            }
            soldier.transform.SetParent(quantityChild.transform);
        }

        // Update is called once per frame
        void Update()
        {
            if(!isSetup) return;
            bool isAllEnemiesDead = true;
            if(isRebellion)
            {
                for (int i = 0; i < enemyInTown.Count; i++)
                {
                    if(enemyInTown[i] == null)
                    {
                        continue;
                    }
                    if(enemyInTown[i].health > 0)
                    {
                        isAllEnemiesDead = false;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < entityInTown.Count; i++)
                {
                    if(entityInTown[i] == null)
                    {
                        continue; 
                    }
                    if(entityInTown[i].health > 0)
                    {
                        isAllEnemiesDead = false;
                        break;
                    }
                }
            }            

            if(isAllEnemiesDead)
            {
                Debug.Log($"<color=red>Switch side!!!!</color>");
                if(!isRebellion)
                {
                    Instantiate(rebellionFxPrefab,transform.position,quaternion.identity);
                }
                else
                {
                    Instantiate(lightFxPrefab,transform.position,quaternion.identity);
                }
                StopAllCoroutines();
                isSetup = false;
                isRebellion = !isRebellion;
                townShipImage.sprite = isRebellion? fogSprite : lightSprite;
                if (isRebellion) SoundManager.Instance.PlayAudioSource("BatrayalT");
                else SoundManager.Instance.PlayAudioSource("BatrayalF");
                StartCoroutine(SetUp());
            }
        }
    }
}

