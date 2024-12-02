using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class SubBarrackSlot : MonoBehaviour
    {
        public bool isBetRoyal;
        public SubBarrackSlotManager subBarrackManager;
        public BarrackSlot referenceBarrack;
        public BarrackType barrackType;
        public Transform[] paths;
        public FortressEmpty fortressTarget;
        public List<EntityMovement> entityPool = new List<EntityMovement>();

        [Header("Spwan")]
        public int maxStack;
        public BarrackState barrackState;
        public BarrackUpgrade barrackUpgrade;

        [Header("Spawning")]
        public bool isStopSpawning = false;
        public bool isSpawning = false;
        public float spawnInterval = 3f; // Interval between spawning soldiers
        private float spawnTimer = 0.0f; // Timer to track time between spawns

        public void SetSubFormMain()
        {
            barrackUpgrade = referenceBarrack.upgradeCurrent;
            subBarrackManager.towerSR.sprite = barrackUpgrade.sprite;

            barrackState.level = referenceBarrack.barrackState.level;
            barrackState.barrackType = referenceBarrack.barrackState.barrackType;
            barrackState.soldierMaxStack = maxStack;

            spawnInterval = barrackUpgrade.respawnTime;//barrackModelSO.barrackModel.regenTime;
            isSpawning = true;
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
                //if (regenTimeTX.text != null) regenTimeTX.text = Mathf.FloorToInt(spawnTimer + 1).ToString() + "S";

                // นับถอยหลังเสร็จแล้ว
                if (spawnTimer >= spawnInterval)
                {
                    if (barrackState.soldierCurrentStack < barrackState.soldierMaxStack)
                    {
                        if (!isBetRoyal) SpwanEntity();
                        else CreateBetroyal();

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
            fortressTarget = IsFortressEmptyType(barrackType);
            if (fortressTarget != null)
            {
                Debug.Log("CreateSoldier");
                CreateSoldier();
            }
        }

        #region Betrayal
        public void StartBetroyal()
        {
            isBetRoyal = true;
            //ClearSolider
            if (entityPool.Count > 0)
            {
                entityPool.ForEach(o => {
                    if(o != null) Destroy(o.gameObject);
                });
            }
        }

        public void CreateBetroyal()
        {
            SubBetrayalSlot betrayalSlot = this.GetComponent<SubBetrayalSlot>();

            GameObject betrayal = Instantiate(betrayalSlot.prefab, paths[0].transform.position, Quaternion.identity);
            EnemyMovement enemy = betrayal.GetComponent<EnemyMovement>();
            enemy.gameObject.AddComponent<SubEntityMovement>().subBarrackSlot = this;
            enemy.path = betrayalSlot.paths;

            //Set Status EnemyRef.
            enemy.status.id = barrackUpgrade.entityStatus[0].id;
            enemy.status.hp = barrackUpgrade.entityStatus[0].hp;
            enemy.status.dmg = barrackUpgrade.entityStatus[0].dmg;
            enemy.status.atkSpd = barrackUpgrade.entityStatus[0].atkSpd;
            enemy.status.moveSpd = barrackUpgrade.entityStatus[0].moveSpd;

            //Set Status จะเหมือนป้อมอันเดิมก่อนจะทรยศ [0] => ตัวเดียว
            enemy.health = enemy.status.hp;
            enemy.damage = enemy.status.dmg;
            enemy.atkSpeed = enemy.status.atkSpd;
            enemy.moveSpeed = barrackUpgrade.entityStatus[0].moveSpd;
            if(barrackUpgrade.spineSkinModelSOBetrayal != null) enemy.spineSkinModelSO = barrackUpgrade.spineSkinModelSOBetrayal;

            enemy.InitStart();
            betrayal.transform.SetParent(LevelStageSlot.Instance.enemySpwan.transform);
            barrackState.soldierCurrentStack++;
        }
        #endregion

        public void CreateSoldier()
        {
            GameObject prefab = referenceBarrack.barrackModelSO.barrackModel.prefabs[0];
            GameObject soldier = Instantiate(prefab, paths[0].transform.position, Quaternion.identity);
            soldier.GetComponent<EntityMovement>().barrackSlot = referenceBarrack;
            soldier.GetComponent<EntityMovement>().SetBaseStatus(barrackUpgrade, 0);
            soldier.AddComponent<SubEntityMovement>().subBarrackSlot = this;
            soldier.transform.SetParent(LevelStageSlot.Instance.heroSpwan.transform);
            entityPool.Add(soldier.GetComponent<EntityMovement>());

            barrackState.soldierCurrentStack++;
            soldier.name = barrackType + "_" + DataCenterManager.GenerateID();
            soldier.GetComponent<EntityMovement>().id = soldier.name;

            #region Fortress Path
            //Set Path
            //Path จาก barrack ไป ประตูเมือง
            //ยืนอยู่หน้าฐานประตูเมือง
            soldier.GetComponent<PathController>().AddTransforms(paths);

            //ต้องมี
            soldier.GetComponent<EntityMovement>().Init();
            soldier.GetComponent<EntityMovement>().InitStatus();
            soldier.GetComponent<EntityMovement>().SetRunPath(soldier.GetComponent<PathController>().GetPath());
            if (this.GetComponent<PathFilpCharacter>() != null) soldier.GetComponent<EntityMovement>().FlipCharacter(this.GetComponent<PathFilpCharacter>().pathFilpType);
            if (fortressTarget != null) soldier.GetComponent<EntityMovement>().GotoFortress(fortressTarget);
            index += 1;
            #endregion
        }

        public void OnSoldierDestroy(int _quantity)
        {
            barrackState.soldierCurrentStack -= _quantity;
            //regenAmountTX.text = barrackState.soldierCurrentStack.ToString() + "/" + barrackState.soldierMaxStack.ToString();
            isSpawning = true;
        }

        public IEnumerator SetHeroFortressPool()
        {
            foreach (var entity in entityPool)
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
            }
        }

        public FortressEmpty IsFortressEmptyType(BarrackType _barrackType)
        {
            FortressEmpty fortressEmpty = subBarrackManager.GetFortress(_barrackType);
            if (fortressEmpty != null) return fortressEmpty;
            else return null;
        }

        public FortressEmpty IsFortressEmpty(EntityMovement _soldier)
        {
            FortressEmpty fortressEmpty = subBarrackManager.GetFortress(_soldier.barrackSlot.barrackState.barrackType);
            if (fortressEmpty != null) return fortressEmpty;
            else return null;
        }

        public void FindFortressHero(GameObject soldier, FortressEmpty _fortressEmpty)
        {
            if (soldier.GetComponent<EntityMovement>().IsGeneral()) return;
            //Set Path
            //หาป้อมปราการที่สร้างและว่างอยู่
            FortressEmpty fortressEmpty = _fortressEmpty;
            //ถ้ามีป้อมวางอยู่
            if (fortressEmpty != null)
            {
                //Set ตัวเองในจับจองที่พัก => จะต้องเช็คด้วยว่าตัวเองซ้ำไหม
                if (!fortressEmpty.fortressSlot.CheckEntityOnFortress(soldier.GetComponent<EntityMovement>()))
                {
                    //ลำดับของป้อม
                    fortressEmpty.fortressSlot.isAdded = true;
                    //เช็คว่าเป็นป้อมสุดท้ายไหม จะได้รีเซ็ต
                    subBarrackManager.CheckIsLastFotress(fortressEmpty.fortressSlot);
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
    }
}
