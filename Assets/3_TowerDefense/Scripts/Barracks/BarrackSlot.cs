using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using System;
using TowerDefense_Prototype;

namespace TowerDefense
{
    public class BarrackSlot : MonoBehaviour
    {
        [Header("Reference Base")]
        public BarrackModelSO barrackModelSO;
        public BarrackType barrackType;
        public Transform[] paths;
        public BarrackUpgrade[] upgradeRef;
        public BarrackUpgrade upgradeCurrent;

        [Header("Reference UI")]
        [SerializeField] private TMPro.TextMeshProUGUI regenTimeTX;
        [SerializeField] private TMPro.TextMeshProUGUI regenAmountTX;

        [Header("General")]
        public bool isGeneral;
        public GameObject general;

        [Header("Spawning")]
        public BarrackState barrackState;
        public FortressEmpty fortressTarget;
        public List<EntityMovement> entityPool = new List<EntityMovement>();
        public bool isStopSpawning = false;
        public bool isSpawning = false;
        public float spawnInterval; // Interval between spawning soldiers
        private float spawnTimer = 0.0f; // Timer to track time between spawns

        private LevelStageManager levelStageManager;
        
        public void InitBarrack()
        {
            if(levelStageManager == null) levelStageManager = GameManager.Instance.LevelStageManager();
            upgradeRef = GameManager.Instance.LevelStageData().statusModelSO.GetUpgrade(barrackType);
            //SetBarrackUpgrade(barrackModelSO.barrackModel.GetBarrackUpgrade(1));
            SetBarrackUpgrade(GameManager.Instance.LevelStageData().statusModelSO.GetBarrackUpgrade(upgradeRef,1));

            if (upgradeCurrent != null)
            {
                barrackState.soldierCurrentStack = 0;
                barrackState.SetBarrackType(barrackModelSO.GetBarrackType());
                spawnInterval = upgradeCurrent.respawnTime;//barrackModelSO.barrackModel.regenTime;

                #region Tutorial Event
                if (LevelStageSlot.Instance.isTutorial)
                {
                    if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_1"))
                    {
                        barrackState.soldierMaxStack = 1;
                    }
                }
                #endregion
            }

            isSpawning = true;
        }

        //ใช้กับอัพเกรด
        public void SetBarrackUpgrade(BarrackUpgrade _barrackUpgrade)
        {
            barrackState.level = _barrackUpgrade.level;
            upgradeCurrent = _barrackUpgrade; _StatusCom7();
            spawnInterval = _barrackUpgrade.respawnTime;

            #region Tutorial Event
            if (LevelStageSlot.Instance.isTutorial)
            {
                if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_3"))
                {
                    spawnInterval = 0;
                }
            }
            #endregion

            void _StatusCom7()
            {
                // ตรวจสอบว่า entityStatus มีขนาดเพียงพอ
                if (upgradeCurrent.entityStatus.Length < 7)
                {
                    // สร้างอาร์เรย์ใหม่ที่ใหญ่ขึ้นและคัดลอกข้อมูลเดิม
                    var newEntityStatus = new EntityStatus[7];
                    upgradeCurrent.entityStatus.CopyTo(newEntityStatus, 0);
                    upgradeCurrent.entityStatus = newEntityStatus;
                }

                if (upgradeCurrent.entityStatus[2] != null)
                {
                    // Index 3: Clone จาก Index 2
                    upgradeCurrent.entityStatus[3] = upgradeCurrent.entityStatus[2].Clone();
                    upgradeCurrent.entityStatus[3].id = "Combo4_LV" + barrackState.level;
                    upgradeCurrent.entityStatus[3].entityCombo = EntityComboType.Combo4;

                    // Index 4: Scale จาก Index 3 (HP และ DMG * 1.75)
                    upgradeCurrent.entityStatus[4] = upgradeCurrent.entityStatus[3].ScaleStats(1.75f, 1.75f);
                    upgradeCurrent.entityStatus[4].id = "Combo5_LV" + barrackState.level;
                    upgradeCurrent.entityStatus[4].entityCombo = EntityComboType.Combo5;

                    // Index 5: Clone จาก Index 4
                    upgradeCurrent.entityStatus[5] = upgradeCurrent.entityStatus[4].Clone();
                    upgradeCurrent.entityStatus[5].id = "Combo6_LV" + barrackState.level;
                    upgradeCurrent.entityStatus[5].entityCombo = EntityComboType.Combo6;

                    // Index 6: Scale จาก Index 5 (HP และ DMG * 2.5)
                    upgradeCurrent.entityStatus[6] = upgradeCurrent.entityStatus[5].ScaleStats(2.5f, 2.5f);
                    upgradeCurrent.entityStatus[6].id = "Combo7_LV" + barrackState.level;
                    upgradeCurrent.entityStatus[6].entityCombo = EntityComboType.Combo7;
                }
            }
        }

        void Update()
        {
            if (isStopSpawning) return;
            if (!isSpawning) return;
            else
            {
                if (entityPool.Count > 0)
                {
                    entityPool.RemoveAll(entity => entity == null || entity.gameObject == null);
                }

                if (barrackState.soldierCurrentStack >= barrackState.soldierMaxStack) return;

                    // Update the spawn timer
                    spawnTimer += Time.deltaTime;
                //if (regenTimeTX.text != null) regenTimeTX.text = Mathf.FloorToInt(spawnTimer+1).ToString() + "S";

                // นับถอยหลังเสร็จแล้ว
                if (spawnTimer >= spawnInterval)
                {
                    if (barrackState.soldierCurrentStack < barrackState.soldierMaxStack)
                    {
                        //CreateSoldier();
                        SpwanEntity();
                        spawnTimer = 0.0f; // Reset the timer after spawning
                    }
                    else
                    {
                        isSpawning = false; // Stop spawning if max stack is reached
                    }
                }
            }
        }

        int index = 0;
        public void SpwanEntity()
        {
            if (LevelStageSlot.Instance.isTutorial)
            {
                fortressTarget = null;
                CreateSoldier();
            }
            if (LevelStageSlot.Instance.isTutorial) return;

            fortressTarget = IsFortressEmptyType(barrackType);
            if (fortressTarget != null)
            {
                CreateSoldier();
            }
        }

        public void CreateSoldier()
        {
            GameObject prefab = barrackModelSO.barrackModel.prefabs[0];
            GameObject soldier = Instantiate(prefab, paths[0].transform.position, Quaternion.identity);
            soldier.GetComponent<EntityMovement>().barrackSlot = this;
            soldier.GetComponent<EntityMovement>().SetBaseStatus(upgradeCurrent,0);
            soldier.transform.SetParent(LevelStageSlot.Instance.heroSpwan.transform);
            entityPool.Add(soldier.GetComponent<EntityMovement>());

            barrackState.soldierCurrentStack++;
            //regenAmountTX.text = barrackState.soldierCurrentStack.ToString() + "/" + barrackState.soldierMaxStack.ToString();
            soldier.name = barrackType+ "_" + DataCenterManager.GenerateID();
            soldier.GetComponent<EntityMovement>().id = soldier.name;
            //prefab.name + index.ToString();

            #region Fortress Path
            //Set Path
            //Path จาก barrack ไป ประตูเมือง
            //ยืนอยู่หน้าฐานประตูเมือง
            soldier.GetComponent<PathController>().AddTransforms(paths);
            //soldier.GetComponent<PathController>().AddTransforms(levelStageManager.levelStageSlot.GetPathTransforms("Entity-Gate-Path"));

            //ต้องมี
            soldier.GetComponent<EntityMovement>().Init();
            soldier.GetComponent<EntityMovement>().InitStatus();
            soldier.GetComponent<EntityMovement>().SetRunPath(soldier.GetComponent<PathController>().GetPath());
            if (this.GetComponent<PathFilpCharacter>() != null) soldier.GetComponent<EntityMovement>().FlipCharacter(this.GetComponent<PathFilpCharacter>().pathFilpType);
            if (fortressTarget != null) soldier.GetComponent<EntityMovement>().GotoFortress(fortressTarget);
            index += 1;

            if (isGeneral)
            {
                soldier.GetComponent<EntityMovement>().health = PercentageCalculator(soldier.GetComponent<EntityMovement>().health);
            }
            #endregion

            #region healthFillView
            if (barrackType == BarrackType.SoldierSword || barrackType == BarrackType.SoldierGun)
            {
                GameObject healthFill = UiController.Instance.InstantiateUIView(LevelStageSlot.Instance.healthGO, LevelStageSlot.Instance.healthEntityGroup);
                healthFill.GetComponent<HealthFillView>().SetInit(soldier.transform, soldier.GetComponent<EntityMovement>().health, soldier.GetComponent<EntityMovement>().baseStatus.hp, "entity");
                soldier.GetComponent<EntityMovement>().healthFillView = healthFill.GetComponent<HealthFillView>();
            }
            #endregion

            #region Tutorial Event
            if (LevelStageSlot.Instance.isTutorial)
            {
                soldier.GetComponent<PathController>().AddTransforms(levelStageManager.levelStageSlot.GetPathTransforms("Entity-Gate-Path"));
                switch (TutorialStatge1.Instance.currentTutorial)
                {
                    case "Tutorial_1":
                        soldier.AddComponent<TutorialCharacter>().currentID = "Sword_Lv1_G1";
                        TutorialStatge1.Instance.entityMovement = soldier.GetComponent<EntityMovement>();
                        soldier.GetComponent<EntityMovement>().health = 10;
                        break;
                    case "Tutorial_3":
                        soldier.AddComponent<TutorialCharacter>().currentID = "Sword_Lv2_G1";
                        TutorialStatge1.Instance.entityMovement = soldier.GetComponent<EntityMovement>();
                        soldier.GetComponent<EntityMovement>().health = 20;
                        break;
                }
            }
            #endregion
        }

        #region หาป้อมให้ฮิโร่

        public FortressEmpty IsFortressEmptyType(BarrackType _barrackType)
        {
            FortressEmpty fortressEmpty = LevelStageSlot.Instance.mainBarrack.GetFortress(_barrackType);
            if (fortressEmpty != null) return fortressEmpty;
            else return null;
        }

        public FortressEmpty IsFortressEmpty(EntityMovement _soldier)
        {
            FortressEmpty fortressEmpty = LevelStageSlot.Instance.mainBarrack.GetFortress(_soldier.barrackSlot.barrackState.barrackType);
            if (fortressEmpty != null) return fortressEmpty;
            else return null;
        }

        public void FindFortressHero(GameObject soldier , FortressEmpty _fortressEmpty)
        {
            if (soldier.GetComponent<EntityMovement>().IsGeneral()) return;
            //Set Path
            //หาป้อมปราการที่สร้างและว่างอยู่
            FortressEmpty fortressEmpty = _fortressEmpty;
            //ถ้ามีป้อมวางอยู่
            if (fortressEmpty != null)
            {                //Set ตัวเองในจับจองที่พัก => จะต้องเช็คด้วยว่าตัวเองซ้ำไหม
                if (!fortressEmpty.fortressSlot.CheckEntityOnFortress(soldier.GetComponent<EntityMovement>()))
                {
                    //ลำดับของป้อม
                    //fortressEmpty.fortressSlot.isAdded = true;
                    //เช็คว่าเป็นป้อมสุดท้ายไหม จะได้รีเซ็ต
                    LevelStageSlot.Instance.mainBarrack.CheckIsLastFotress(fortressEmpty.fortressSlot);
                    soldier.GetComponent<EntityMovement>().fortressSlot = fortressEmpty.fortressSlot;
                    //Path จากประตูไปป้อมปราการ
                    soldier.GetComponent<PathController>().ClearPath();
                    soldier.GetComponent<PathController>().AddTransforms(fortressEmpty.fortressSlot.GetStagePath("Entity-FormGate"));
                    //Path จากป้อมปราการไปที่พัก
                    //soldier.GetComponent<PathController>().AddPath(fortressEmpty.state.pathSlot);

                    fortressEmpty.state.targetEntity = soldier.GetComponent<EntityMovement>();
                    soldier.GetComponent<EntityMovement>().SetRunPath(soldier.GetComponent<PathController>().GetPath());
                }
            }
        }

        public IEnumerator SetHeroFortressPool()
        {
            List<EntityMovement> entitiesToProcess = new List<EntityMovement>();

            // เก็บข้อมูลที่จะประมวลผลในลิสต์แยก
            foreach (var entity in entityPool)
            {
                if (entity != null)
                {
                    FortressEmpty fortressEmpty = IsFortressEmpty(entity);
                    if (fortressEmpty != null)
                    {
                        if (entity.GetComponent<EntityMovement>().actionEntity == ActionEntity.Defensive)
                        {
                            entitiesToProcess.Add(entity); // เก็บ entity ไว้ในลิสต์แยก
                        }
                    }
                }
            }

            // ประมวลผลลิสต์ที่เก็บไว้นอกลูป foreach
            foreach (var entity in entitiesToProcess)
            {
                FindFortressHero(entity.gameObject, IsFortressEmpty(entity));
                Debug.Log(entity.gameObject.name + ": SetHeroFortressPool");
                yield return new WaitForSeconds(0.25f);
            }
            /*
            foreach(var entity in entityPool)
            {
                if (entity != null)
                {
                    FortressEmpty fortressEmpty = IsFortressEmpty(entity);
                    if (fortressEmpty != null)
                    {
                        if (entity.GetComponent<EntityMovement>().actionEntity == ActionEntity.Defensive)
                        {
                            FindFortressHero(entity.gameObject, fortressEmpty);
                            Debug.Log(entity.gameObject.name + ": SetHeroFortressPool");
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                }
            }*/
        }
        #endregion

        //ฟังก์ชั่นนี้ใช้แค่ใน Tutorial
        public GameObject CreateGeneral(Vector3 _pos)
        {
            GameObject prefab = barrackModelSO.barrackModel.general;
            GameObject general = Instantiate(prefab, _pos, Quaternion.identity);
            general.GetComponent<EntityMovement>().barrackSlot = this;
            general.transform.SetParent(LevelStageSlot.Instance.heroSpwan.transform);
            general.name = prefab.name + index.ToString();
            this.general = general;

            //Status
            general.GetComponent<EntityMovement>().baseStatus = GameManager.Instance.LevelStageData().statusModelSO.general;

            //ต้องมี
            general.GetComponent<EntityMovement>().Init();
            general.GetComponent<EntityMovement>().InitStatus();
            general.GetComponent<EntityMovement>().FlipCharacter(PathFilpType.Right);
            general.GetComponent<EntityMovement>().SetRunPath(levelStageManager.levelStageSlot.GetPathTransforms("Entity-General-Path"));

            isGeneral = true;
            Transform sodlierParent = LevelStageSlot.Instance.heroSpwan.transform;
            if (LevelStageSlot.Instance.heroSpwan.transform.childCount > 0)
            {
                // วนลูปผ่านลูกทั้งหมดของ sodlierParent
                foreach (Transform child in sodlierParent)
                {
                    child.GetComponent<EntityMovement>().health = PercentageCalculator(child.GetComponent<EntityMovement>().health);
                }
            }
            return general;
        }

        public void OnSoldierDestroy(int _quantity)
        {
            barrackState.soldierCurrentStack -= _quantity;
            //regenAmountTX.text = barrackState.soldierCurrentStack.ToString() + "/" + barrackState.soldierMaxStack.ToString();
            isSpawning = true;
        }

        private float PercentageCalculator(float _hp)
        {
            float value = _hp;
            float increasePercentage = 200f;
            float increasedValue = CalculateIncreasedValue(value, increasePercentage);
            float CalculateIncreasedValue(float value, float percentage)
            {
                return value + (percentage / 100) * value;
            }

            return increasedValue;
        }
    }

}
