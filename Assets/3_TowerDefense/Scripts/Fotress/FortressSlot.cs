using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;

namespace TowerDefense
{
    public enum FortressUpgradeType
    {
        None,
        Acttack,
        Defense
    }

    [System.Serializable]
    public class FortressState
    {
        public EntityMovement targetEntity;
        public ActionEntity entityAction;
    }

    public class FortressSlot : MonoBehaviour
    {
        [Header("Model")]
        public string id;
        [SerializeField] private FortressSprites fortressSprites;
        [SerializeField] private TMPro.TextMeshProUGUI quantityTX;
        public float entity_scaleInFortress;
        public float entity_rangeInFortress;
        public StagePath[] stagePath;

        [Header("Upgread")]
        public int soldierMax;
        public FortressUpgradeType upgradeType;
        public float statusMultiplier = 1f;

        [Header("State")]
        public BarrackType fortressType;
        public BarrackSlot barrackSlot;
        public List<FortressState> targetEntitys;
        [SerializeField] private List<EntityMovement> entitiesInFortress = new List<EntityMovement>();
        [SerializeField] private GameObject comboEntity;
        private Vector3 fontPos;
        private Transform fightEnemy;
        //มาร์กลำดับป้อม
        public bool isAdded;

        [Header("TakeAttackFortress")]
        public GameObject attackRange;
        [SerializeField] private float targetRange;
        public Transform targetEnemy;
        public List<Transform> hitListsFortress = new List<Transform>();
        [SerializeField] private LayerMask enemyMask;
        GoldGenerator goldGenerator;

        [Header("Monk Effect")]
        public float monkRadius;
        public LayerMask fortressMask;
        private List<FortressSlot> fortressinbuff;


        public void Start()
        {
            statusMultiplier = 1f;
            fightEnemy = GetStagePath("Entity-FontFortress")[0];
            fontPos = fightEnemy.position;
            fortressinbuff = new List<FortressSlot>();
        }

        public void SetDefault()
        {
            if(fortressType == BarrackType.Monk)
            {
                Debug.Log("Reset Monk fortress!!");
                for (int i = 0; i < fortressinbuff.Count; i++)
                {
                    fortressinbuff[i].statusMultiplier -= 0.5f;
                }
                fortressinbuff.Clear();
            }
            fortressType = BarrackType.Empty;
            soldierMax = 3;
            targetEntitys.Clear();
            for (int i = 0; i < soldierMax; i++) targetEntitys.Add(new FortressState());

            entitiesInFortress.Clear();
            SetQuantity();
            SetSprite();

            targetEnemy = null;
            hitListsFortress.Clear();

            LevelStageSlot.Instance.mainBarrack.fortressSlotConstruct.Remove(this);

            if (comboEntity != null)
            {
                Destroy(comboEntity);
            }
        }

        #region UpSize
        public void UpSizeFortress(int _size)
        {
            if (_size > 7) _size = 7;
            int subtract = _size - soldierMax;
            soldierMax = _size;
            for (int i = 0; i < subtract; i++) targetEntitys.Add(new FortressState());
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.cyan;
            Handles.DrawWireCube(attackRange.transform.position, targetRange * transform.localScale);
            //Handles.DrawWireDisc(attackRange.transform.position, transform.forward, targetRange);
        }
#endif
        public void FindTargetEnemy()
        {
            if (fortressType == BarrackType.Empty) return;
            if (fortressType == BarrackType.Merchant || fortressType == BarrackType.Monk) targetEnemy = null;
            if (fortressType == BarrackType.Merchant || fortressType == BarrackType.Monk) return;

            hitListsFortress.Clear();
            targetEnemy = null;
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                attackRange.transform.position,
                targetRange * (Vector2)transform.localScale,
                0f,
                enemyMask
            );

            if (hits.Length > 0)
            {
                hits.ToList().ForEach(o => {
                    if (o.transform != null && o.transform.gameObject != null)
                    {
                        hitListsFortress.Add(o.transform);
                        if (o.transform.gameObject.GetComponent<EnemyMovement>() != null)
                        {
                            targetEnemy = o.transform;
                            return;
                        }
                    }
                });
                targetEnemy = hits[0].transform;
            }
        }

        public void Update()
        {
            if (fortressType != BarrackType.Empty)
            {
                FindTargetEnemy();

                if (entitiesInFortress.Count > 0)
                {
                    // ลบ Entity ที่ถูก Destroy หรือเป็น null
                    entitiesInFortress.RemoveAll(entity => entity == null || entity.gameObject == null);
                    SetQuantity();
                }

                if (targetEnemy == null && fightEnemy.position != fontPos) fightEnemy.position = fontPos;
            }

        }

        public void Demolition()
        {
            if (!IsCanDemolition()) return;
            #region Defensive Attack
            if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Defensive)
            {
                //ถ้าแบบอยู่ในป้อมแล้ว
                if (entitiesInFortress.Count == 1)
                {
                    if (entitiesInFortress[0].actionEntity == ActionEntity.AttackEnemy)
                    {
                        if (!entitiesInFortress[0].isDemolitionFortress)
                        {
                            if (!entitiesInFortress[0].GetComponent<EntityMovement>().isDemolitionFortress)
                            {
                                entitiesInFortress[0].GetComponent<EntityMovement>().isDemolitionFortress = true;
                                _SetDemolition();
                                entitiesInFortress.Clear();
                                return;
                            }
                        }
                    }
                }
                else if (entitiesInFortress.Count >= 2)
                {
                    if (comboEntity != null)
                    {
                        if (comboEntity.GetComponent<EntityMovement>().actionEntity == ActionEntity.AttackEnemy)
                        {
                            if (!comboEntity.GetComponent<EntityMovement>().isDemolitionFortress)
                            {
                                comboEntity.GetComponent<EntityMovement>().isDemolitionFortress = true;
                                _SetDemolition();
                                comboEntity = null;
                                return;
                            }
                        }
                    }
                }
            }
            #endregion

            if (fortressType == BarrackType.Empty)
            {
                GameManager.Instance.LevelStageUI().selectFortessUI.Show(this);
            }
            else
            {
                if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Offensive)
                {
                    //ถอนป้อมแบบรุก
                    StartCoroutine(_Demolition(GetStagePath("Entity-AttackGateTower")));
                }

                if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Defensive)
                {
                    //ถอนป้อมแบบตั้งรับ แบบตัวละครอยู่ในป้อมเท่านั้น
                    //StartCoroutine(_Demolition(GetStagePath("Entity-BackBarrack")));
                    StartCoroutine(_Demolition(null));
                }

                if(fortressType == BarrackType.Merchant &&  goldGenerator != null)
                {
                    Destroy(goldGenerator);
                }
            }

            IEnumerator _Demolition(Transform[] _path)
            {
                 Debug.Log("_Demolition");
                if (entitiesInFortress.Count > 0)
                {
                    fortressType = BarrackType.Empty;
                    quantityTX.gameObject.SetActive(false);
                    SetSprite();

                    //กดถอนป้อมแบบด่านบุก
                    if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Offensive)
                    {
                        TakeAttackFortress(GetStagePath("Entity-AttackGateTower"));
                    }

                    //กดถอนป้อมแบบด่านรับ
                    if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Defensive)
                    {
                        if (comboEntity != null)
                        {
                            comboEntity.GetComponent<EntityMovement>().deadQuantity = 0;
                            Destroy(comboEntity);
                        }

                        foreach (var o in targetEntitys.ToList())
                        {
                            if (o.targetEntity != null)
                            {
                                //รื้อถอนแล้วมีตัวละครที่อยู่ในป้อม
                                if (o.targetEntity.actionEntity == ActionEntity.InFotress)
                                {
                                    Debug.Log("กดถอนป้อมแบบด่านรับ: " + o.targetEntity.name);
                                    o.targetEntity.gameObject.SetActive(true);
                                    if(o.targetEntity.GetComponent<EntityMovement>().healthFillView != null) o.targetEntity.GetComponent<EntityMovement>().healthFillView.gameObject.SetActive(true);

                                    //_SetEntity(o.entity, _path);
                                    o.targetEntity.ResetScale();
                                    if (o.targetEntity.baseType == BaseType.Defensive) o.targetEntity.isBackFilpChar = true;
                                    Debug.Log(o.targetEntity.name + "Demolition_2");
                                    //o.entity = null;

                                    // หน่วงเวลา 0.5 วินาที
                                    //yield return new WaitForSeconds(0.25f);
                                }
                            }
                        }
                    }

                    /*
                    void _SetEntity(EntityMovement _entity , Transform[] _path)
                    {
                        EntityMovement movement = _entity.GetComponent<EntityMovement>();
                        EntityAnimation animation = _entity.GetComponent<EntityAnimation>();

                        movement.Init();
                        movement.path = _path;
                        movement.InitPath();
                        movement.PathFlipEntity();
                        animation.Run();
                        movement.ResetScale();
                    }*/
                }

                //SetPath
                foreach (var o in targetEntitys.ToList())
                {
                    if (o.targetEntity != null)
                    {
                        Debug.Log(name + ": Defensive: ถอยกลับ_0");
                        //รื้อถอนแล้วมีตัวละครกำลังเดินมา
                        //if (o.entity.actionEntity != ActionEntity.InFotress)
                        if (o.targetEntity.actionEntity != ActionEntity.InFotress || o.targetEntity.actionEntity == ActionEntity.InFotress)
                        {
                            Debug.Log(name + ": Defensive: ถอยกลับ_1");
                            PathController pathController = o.targetEntity.GetComponent<PathController>();
                            EntityMovement movement = o.targetEntity.GetComponent<EntityMovement>();

                            o.targetEntity.gameObject.SetActive(true);
                            movement.Init();
                            pathController.ClearPath();

                            GameObject tempObject = new GameObject("SecondaryPath");
                            tempObject.transform.position = o.targetEntity.transform.position; // กำหนดตำแหน่งให้มัน
                            pathController.AddTransforms(new Transform[] { tempObject.transform }); // ส่งอาร์เรย์ที่มี Transform ของมันเข้าไป
                            Debug.Log("Position:" + tempObject.transform.position);

                            //คัดลอก pathFilpType ของpathป้อม
                            Transform fortressPath = GetStagePath("Entity-Fortress")[0];
                            tempObject.AddComponent<PathFilpCharacter>().pathFilpType = fortressPath.GetComponent<PathFilpCharacter>().pathFilpType;
                            tempObject.transform.SetParent(o.targetEntity.transform);

                            ///หลักการคือ Posตัวเอง + Path หน้าประตูไปหน้าป้อม (Entity-FormGate) หักกับPathที่เดินไปแล้ว +  Path หน้าประตูไปตีประตูศัตรู(Entity-AttackGateTower)
                            if (o.targetEntity.baseType == BaseType.Offensive)
                            {
                                List<Transform> backupPaths = new List<Transform>(); // backup ตัวเก่าของฮิโร่
                                pathController.backupPaths.ForEach(o => { backupPaths.Add(o); });
                                List<Transform> _backupPaths = new List<Transform>(); // backup ที่จะใส่ตัวใหม่ เอาไว้หักกับ Path ที่เดินไปแล้ว
                                Transform[] entityPaths = GetStagePath("Entity-FormGate");

                                foreach (Transform entityPath in entityPaths)
                                {
                                    bool isPathInBackup = false;
                                    foreach (Transform _backupPath in backupPaths)
                                    {
                                        if (_backupPath == entityPath)
                                        {
                                            isPathInBackup = true;
                                            break;
                                        }
                                    }
                                    if (!isPathInBackup)
                                    {
                                        _backupPaths.Add(entityPath);
                                        Debug.Log("Added missing path: " + entityPath.name);
                                    }
                                }

                                pathController.AddPath(tempObject.transform);
                                pathController.AddTransforms(_backupPaths.ToArray());
                                pathController.AddTransforms(GetStagePath("Entity-AttackGateTower"));
                                Debug.Log("Demolition_วิ่งอยู่_Offensive");
                            }

                            ///หลักการคือ Pos ตัวเอง + Path backup reverse กลับประตูตัวเอง Gate
                            if (o.targetEntity.baseType == BaseType.Defensive)
                            {
                                var itemsToRemove = new List<Transform>();
                                pathController.backupPaths.ToList().ForEach(o =>
                                {
                                    if (o.GetComponent<PathAction>() != null)
                                    {
                                        if (o.GetComponent<PathAction>().pathAction == ActionEntity.InFotress)
                                        {
                                            itemsToRemove.Add(o);
                                        }
                                    }
                                });

                                // ลบรายการที่ตรงตามเงื่อนไขออกจาก backupPaths
                                foreach (var item in itemsToRemove)
                                {
                                    pathController.backupPaths.Remove(item);
                                }

                                pathController.backupPaths.Reverse();
                                pathController.AddPath(tempObject.transform);
                                pathController.AddTransforms(pathController.backupPaths.ToArray());

                                movement.isBackFilpChar = true;
                                Debug.Log(name + ": Defensive: ถอยกลับ_2");
                            }

                            movement.path = pathController.GetPath();

                            o.targetEntity.InitPath();
                            //movement.PathFlipEntity();
                            o.targetEntity.GetComponent<EntityAnimation>().Run();
                            pathController.ClearPath();

                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                }

                _SetDemolition();
            }

            void _SetDemolition()
            {
                targetEntitys.ToList().ForEach(o => {
                    if (o.targetEntity != null)
                    {
                        o.targetEntity.deadQuantity = 1;
                        o.targetEntity.fortressSlot = null;
                        o.targetEntity = null;
                    }
                });

                entitiesInFortress.Clear();
                SetQuantity();

                fortressType = BarrackType.Empty;
                SetSprite();
                isAdded = false;

                //ถอนออกจากลิสหลัก
                if (LevelStageSlot.Instance.mainBarrack.fortressSlotConstruct.Contains(this))
                {
                    LevelStageSlot.Instance.mainBarrack.fortressSlotConstruct.Remove(this);
                }
                Debug.Log("_SetDemolition");

                //Refund
                int cost = barrackSlot.barrackModelSO.barrackModel.costFortress;
                int refund = cost / 2;
                GameManager.Instance.GoldCost().IncreaseGold(refund);
                Debug.Log("Refund Fortress:" + refund);
            }
        }

        //สามารถถอนป้อมได้ไหม
        public bool IsCanDemolition()
        {
            bool isCanDemolition = false;

            //ป้อมมีศัตรูมาตี
            if (targetEnemy == null) isCanDemolition = true;
            return isCanDemolition;
        }

        //ป้อมโดนโจมตี ปล้อยฮีโร่ออกมาโจมตี
        public void TakeAttackFortress(Transform[] _paths)
        {
            if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Offensive)
            {
                if (entitiesInFortress.Count == 1)
                {
                    foreach (var o in targetEntitys.ToList())
                    {
                        if (o.targetEntity != null)
                        {
                            if (o.targetEntity.actionEntity == ActionEntity.InFotress)
                            {
                                o.targetEntity.gameObject.SetActive(true);
                                _SetEntity(o.targetEntity, _paths);
                                o.targetEntity.PathFlipEntity();
                                o.targetEntity = null;
                            }
                        }
                    }
                    entitiesInFortress.Clear();
                    SetQuantity();
                    return;
                }

                if (entitiesInFortress.Count == 2)
                {
                    if (comboEntity != null)
                    {
                        entitiesInFortress.ForEach(o =>
                        {
                            if (o != null)
                            {
                                if (o.actionEntity == ActionEntity.InFotress)
                                {
                                    o.deadQuantity = 0;
                                    Destroy(o.gameObject);
                                }
                            }
                        });
                        //Entity-AttackBarrack
                        _SetEntity(comboEntity.GetComponent<EntityMovement>(), _paths);
                        comboEntity.GetComponent<EntityMovement>().PathFlipEntity();
                        entitiesInFortress.Clear();
                        SetQuantity();
                    }
                }
            }
            if (GameManager.Instance.LevelStageManager().currentLevelStageModelSO.levelStageModel.baseType == BaseType.Defensive)
            {
                if (entitiesInFortress.Count == 1)
                {
                    foreach (var o in targetEntitys.ToList())
                    {
                        if (o.targetEntity != null)
                        {
                            if (o.targetEntity.actionEntity == ActionEntity.InFotress)
                            {
                                o.targetEntity.gameObject.SetActive(true);
                                _SetEntity(o.targetEntity, _paths);
                                o.targetEntity.PathFlipEntity();
                            }
                        }
                    }
                    SetQuantity();
                    return;
                }

                if (entitiesInFortress.Count >= 2)
                {
                    if (comboEntity != null)
                    {
                        //Entity-AttackBarrack
                        _SetEntity(comboEntity.GetComponent<EntityMovement>(), _paths);
                        comboEntity.GetComponent<EntityMovement>().PathFlipEntity();
                        SetQuantity();
                    }
                }
            }

            void _SetEntity(EntityMovement _entity, Transform[] _path)
            {
                EntityMovement movement = _entity.GetComponent<EntityMovement>();
                EntityAnimation animation = _entity.GetComponent<EntityAnimation>();

                //movement.Init();
                movement.path = _path;
                movement.InitPath();
                movement.PathFlipEntity();
                animation.Run();
                movement.ResetScale();
                movement.ResetTargetRange();
            }
        }

        public int GetCurrentSolier()
        {
            return entitiesInFortress.Count();
        }

        public void ResetDestroy(bool isDestroy = true)
        {
            targetEntitys.ToList().ForEach(o =>
            {
                if (o.targetEntity != null)
                {
                    o.targetEntity.deadQuantity = 0;
                    if (isDestroy)
                    {
                        if (o.targetEntity.gameObject != null) Destroy(o.targetEntity.gameObject);
                        o.targetEntity = null;
                    }
                    else
                    {
                        if (o.targetEntity.gameObject != null) o.targetEntity.gameObject.SetActive(false);
                    }
                }
            });
            SetQuantity();
        }

        public void SetQuantity()
        {
            if (entitiesInFortress.Count > 0)
            {
                // ลบ Entity ที่ถูก Destroy หรือเป็น null
                entitiesInFortress.RemoveAll(entity => entity == null || entity.gameObject == null);
            }

            if (quantityTX == null)
            {
                GameObject quantityTXGO = UiController.Instance.InstantiateUIView(
                    FortessOptionsUI.Instance.fortress_Quantity_prefab ,
                    FortessOptionsUI.Instance.fortress_Quantity_parent);

                quantityTXGO.GetComponent<WorldToUIPos>().targetTransform = this.transform;
                quantityTX = quantityTXGO.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>();
            }

            int currentSolier = entitiesInFortress.Count();//GetCurrentSolier();
            soldierMax = targetEntitys.Count;

            if (currentSolier <= 0)
            {
                entitiesInFortress.Clear();
                if (quantityTX != null) Destroy(quantityTX.transform.parent.gameObject);
                //quantityTX.gameObject.SetActive(false);
            }
            else
            {
                if (quantityTX != null)
                {
                    quantityTX.text = currentSolier + "/" + soldierMax;
                }
                //quantityTX.gameObject.SetActive(true);
            }
        }

        public void SetSprite()
        {
            if (fortressSprites == null)
            {
                GameObject fortressSP = Instantiate(FortessOptionsUI.Instance.fortressSpriteGO,transform.position,Quaternion.identity);
                fortressSP.transform.SetParent(this.transform);
                fortressSP.transform.localPosition = Vector3.zero;
                fortressSP.transform.localScale = Vector3.one;
                fortressSprites = fortressSP.GetComponent<FortressSprites>();
            }

            if (fortressType != BarrackType.Empty)
            {
                if(fortressSprites != null) fortressSprites.SetSpriteRenderer(LevelStageSlot.Instance.stageModelSO.GetBarrackModel(fortressType).fortressFullSP);
            }
            else
            {
                if (fortressSprites != null) fortressSprites.SetSpriteRendererNull();
            }
        }

        public void UpgreadeEffect(FortressUpgradeType _type)
        {
            statusMultiplier += 0.5f;
            if (fortressSprites == null)
            {
                GameObject fortressSP = Instantiate(FortessOptionsUI.Instance.fortressSpriteGO, transform.position, Quaternion.identity);
                fortressSP.transform.SetParent(this.transform);
                fortressSP.transform.localPosition = Vector3.zero;
                fortressSP.transform.localScale = Vector3.one;
                fortressSprites = fortressSP.GetComponent<FortressSprites>();
            }

            if (fortressSprites != null) fortressSprites.SetSpriteRenderer(barrackSlot.barrackModelSO.GetFortressUpgrade(_type).fortressFullSP);
        }

        public void SetFortress(BarrackType _barrackType)
        {
            soldierMax = 3;
            targetEntitys.Clear();
            for (int i = 0; i < soldierMax; i++) targetEntitys.Add(new FortressState());

            fortressType = _barrackType;
            //mainBarrack
            if (!IsSubFortress())
            {
                LevelStageSlot.Instance.mainBarrack.AddFortressConstruction(this);
            }
            //subBarrack
            else
            {
                this.GetComponent<SubFortressSlot>().subBarrackSlotManager.AddFortressConstruction(this);
            }

            SetSprite();
            if (_barrackType == BarrackType.Empty) barrackSlot = null;
            else barrackSlot = LevelStageSlot.Instance.mainBarrack.barrackSlotsDic[_barrackType];
            isAdded = false;
        }

        public void ResetEntityFortress(EntityMovement _entity)
        {
            targetEntitys.ToList().ForEach(o => {
                if (o.targetEntity == _entity)
                {
                    _entity.fortressSlot = null;
                    o.targetEntity = null;
                }
            });
        }

        private GameObject CreateSoldier(EntityMovement _entity,int _indexEntity)
        {
            Vector3 pos = GetStagePath("Entity-Fortress")[0].position;
            GameObject prefab = _entity.GetComponent<EntityMovement>().barrackSlot.barrackModelSO.barrackModel.prefabs[_indexEntity];
            GameObject soldier = Instantiate(prefab, pos, Quaternion.identity);
            soldier.GetComponent<EntityMovement>().baseType = _entity.GetComponent<EntityMovement>().baseType;
            soldier.GetComponent<EntityMovement>().barrackSlot = _entity.GetComponent<EntityMovement>().barrackSlot;
            soldier.GetComponent<EntityMovement>().fortressSlot = this;
            soldier.GetComponent<EntityMovement>().SetBaseStatus(soldier.GetComponent<EntityMovement>().barrackSlot.upgradeCurrent, _indexEntity);
            soldier.transform.SetParent(LevelStageSlot.Instance.heroSpwan.transform);
            soldier.name = DataCenterManager.GenerateID();
            soldier.GetComponent<EntityMovement>().id = soldier.name;

            if (_entity.IsSubEntity())
            {
                soldier.AddComponent<SubEntityMovement>().subBarrackSlot = _entity.GetComponent<SubEntityMovement>().subBarrackSlot;
            }

            //if (_entity.GetComponent<EntityMovement>().healthFillView != null) _entity.GetComponent<EntityMovement>().healthFillView.gameObject.SetActive(false);
            return soldier;
        }

        #region Combo
        //ออกมาโจมตี
        public void OffensiveBarrack(EntityMovement _entity)
        {
            if (!CheckEntityOnFortress(_entity)) return;

            // ตรวจสอบว่า entity นี้ยังไม่มีในลิสต์หรือไม่
            if (!entitiesInFortress.Contains(_entity))
            {
                // ถ้ายังไม่มีอยู่ในลิสต์ ให้แอดเข้าไป
                entitiesInFortress.Add(_entity);
            }

            if (entitiesInFortress.Count() > 0) entitiesInFortress.RemoveAll(entity => entity == null);

            // จำนวน Entity ที่อยู่ในป้อม
            int entityCount = entitiesInFortress.Count;

            if (entityCount == 1)
            {
                // เมื่อมีแค่ 1 Entity
                //ตัวเดียว
                _entity.StopMoving();
                _entity.anim.InFotress();
                _entity.UpdateScale(entity_scaleInFortress);
                _entity.PathFlipEntity();
                _entity.UpdateTargetRange(entity_rangeInFortress);
                SetQuantity();
                Debug.Log("Single :" + GetCurrentSolier() + _entity.name);
            }
            else if (entityCount == 2)
            {
                _entity.StopMoving();
                GameObject soldier = CreateSoldier(_entity,1);
                EntityMovement movement = soldier.GetComponent<EntityMovement>();
                EntityAnimation animation = soldier.GetComponent<EntityAnimation>();
                comboEntity = soldier;

                //Path ออกไปพร้อมสู้
                movement.Init();
                movement.InitStatus();
                movement.path = GetStagePath("Entity-Fortress");
                movement.InitPath();
                movement.StopMoving();
                movement.UpdateScale(entity_scaleInFortress);
                movement.UpdateTargetRange(entity_rangeInFortress);
                movement.PathFlipEntity();

                entitiesInFortress.ForEach(o => {
                    o.gameObject.SetActive(false);
                });

                animation.InFotress();
                SetQuantity();
                Debug.Log("IsTwo :" + GetCurrentSolier() + _entity.name);
            }
            else if (entityCount == 3)
            {
                GameObject soldier = CreateSoldier(_entity,2);
                EntityMovement movement = soldier.GetComponent<EntityMovement>();
                EntityAnimation animation = soldier.GetComponent<EntityAnimation>();

                comboEntity.GetComponent<EntityMovement>().deadQuantity = 0;
                if (comboEntity != null) Destroy(comboEntity);
                comboEntity = soldier;

                //Set path ออกไปสู้ copy pathCombo2 มาใช้
                movement.Init();
                movement.InitStatus();
                movement.path = GetStagePath("Entity-AttackGateTower");
                movement.InitPath();
                soldier.GetComponent<EntityAnimation>().Run();
                movement.ResetScale();
                movement.ResetTargetRange();
                movement.PathFlipEntity();

                ResetDestroy(true);
                comboEntity = null;
                entitiesInFortress.Clear();
                SetQuantity();

                Debug.Log("IsThree :" + GetCurrentSolier() + _entity.name);
            }
            else
            {
                // เมื่อไม่มีหรือมากกว่า 3 Entity ให้ยืนเฉยๆ หรือทำการอื่นๆ
                foreach (EntityMovement entity in entitiesInFortress)
                {
                    entity.StopMoving();
                }
                Debug.Log("Entities in fortress standing idle.");
            }
        }

        //อยู่เฝ้าป้อม
        public void DefensiveBarrack(EntityMovement _entity)
        {
            if (!CheckEntityOnFortress(_entity)) return;
            if (!entitiesInFortress.Contains(_entity))
            {
                if(_entity.comboType == EntityComboType.Combo1) entitiesInFortress.Add(_entity);
            }

            if (entitiesInFortress.Count() > 0) entitiesInFortress.RemoveAll(entity => entity == null);

            // จำนวน Entity ที่อยู่ในป้อม
            int entityCount = entitiesInFortress.Count;

            if (entityCount == 1)
            {
                // เมื่อมีแค่ 1 Entity
                //ตัวเดียว
                _entity.comboEntityList.Add(_entity);
                _entity.StopMoving();
                _entity.anim.InFotress();
                _entity.UpdateScale(entity_scaleInFortress);
                _entity.PathFlipEntity();
                _entity.UpdateTargetRange(entity_rangeInFortress);
                SetQuantity();
                Debug.Log("Single :" + GetCurrentSolier() + _entity.name);
            }
            else if (entityCount > 1)
            {
                GameObject backupCombo = null;
                if (comboEntity != null) backupCombo = comboEntity;
                GameObject soldier = CreateSoldier(_entity, entityCount-1);
                EntityMovement movement = soldier.GetComponent<EntityMovement>();
                EntityAnimation animation = soldier.GetComponent<EntityAnimation>();
                comboEntity = soldier;
                entitiesInFortress.ForEach(o => {
                    comboEntity.GetComponent<EntityMovement>().comboEntityList.Add(o);
                    o.gameObject.SetActive(false);
                });

                movement.Init();
                movement.InitStatus();
                movement.path = GetStagePath("Entity-Fortress");
                movement.InitPath();
                movement.StopMoving();
                movement.UpdateScale(entity_scaleInFortress);
                movement.UpdateTargetRange(entity_rangeInFortress);
                movement.PathFlipEntity();

                entitiesInFortress.ForEach(o => {
                    o.gameObject.SetActive(false);
                    if(o.GetComponent<EntityMovement>().healthFillView != null) o.GetComponent<EntityMovement>().healthFillView.gameObject.SetActive(false);
                });

                animation.InFotress();
                SetQuantity();
                //ResetDestroy(false); //Set Active false

                if (backupCombo != null)
                {
                    backupCombo.GetComponent<EntityMovement>().deadQuantity = 0;
                    Destroy(backupCombo);
                }

                #region healthFillView
                if (fortressType == BarrackType.SoldierSword || fortressType == BarrackType.SoldierGun)
                {
                    GameObject healthFill = UiController.Instance.InstantiateUIView(LevelStageSlot.Instance.healthGO, LevelStageSlot.Instance.healthEntityGroup);
                    healthFill.GetComponent<HealthFillView>().SetInit(comboEntity.transform, comboEntity.GetComponent<EntityMovement>().health, comboEntity.GetComponent<EntityMovement>().baseStatus.hp, "entity");
                    comboEntity.GetComponent<EntityMovement>().healthFillView = healthFill.GetComponent<HealthFillView>();
                }
                #endregion
            }

            if (fortressType == BarrackType.Merchant)
            {
                if (goldGenerator == null)
                {
                    goldGenerator = fortressSprites.GetComponent<GoldGenerator>();
                }
                if(entityCount >= 2) goldGenerator.StartGoldGeneration(comboEntity.GetComponent<EntityMovement>().comboType);
            }

            if(fortressType == BarrackType.Monk)
            {
                if(entityCount >= 3)
                {
                    fortressinbuff.Clear();
                    Collider2D[] allFortressinrange = Physics2D.OverlapCircleAll(transform.position,monkRadius,fortressMask);
                    for (int i = 0; i < allFortressinrange.Length; i++)
                    {
                        FortressSlot temp = allFortressinrange[i].GetComponent<FortressSlot>();
                        fortressinbuff.Add(temp);
                        temp.statusMultiplier += 0.5f;
                    }
                }
            }



            /*
            else if (entityCount == 2)
            {
                _entity.StopMoving();
                GameObject soldier = CreateSoldier(_entity, 1);
                EntityMovement movement = soldier.GetComponent<EntityMovement>();
                EntityAnimation animation = soldier.GetComponent<EntityAnimation>();
                comboEntity = soldier;
                entitiesInFortress.ForEach(o => {
                    comboEntity.GetComponent<EntityMovement>().comboEntityList.Add(o);
                });

                //Path ออกไปพร้อมสู้
                movement.Init();
                movement.InitStatus();
                movement.path = GetStagePath("Entity-Fortress");
                movement.InitPath();
                movement.StopMoving();
                movement.UpdateScale(entity_scaleInFortress);
                movement.UpdateTargetRange(entity_rangeInFortress);
                movement.PathFlipEntity();

                entitiesInFortress.ForEach(o => {
                    o.gameObject.SetActive(false);
                });

                animation.InFotress();
                SetQuantity();
                Debug.Log("IsTwo :" + GetCurrentSolier() + _entity.name);
            }

            else if (entityCount == 3)
            {
                GameObject soldier = CreateSoldier(_entity, 2);
                EntityMovement movement = soldier.GetComponent<EntityMovement>();
                EntityAnimation animation = soldier.GetComponent<EntityAnimation>();

                if (comboEntity != null) comboEntity.GetComponent<EntityMovement>().deadQuantity = 0;
                if (comboEntity != null) Destroy(comboEntity);
                comboEntity = soldier;
                entitiesInFortress.ForEach(o => {
                    comboEntity.GetComponent<EntityMovement>().comboEntityList.Add(o);
                });
                //Set path ออกไปสู้ copy pathCombo2 มาใช้
                movement.Init();
                movement.InitStatus();
                movement.path = GetStagePath("Entity-Fortress");
                movement.InitPath();
                movement.StopMoving();
                animation.InFotress();
                movement.UpdateScale(entity_scaleInFortress);
                movement.UpdateTargetRange(entity_rangeInFortress);
                movement.PathFlipEntity();

                SetQuantity();
                Debug.Log("IsThree :" + GetCurrentSolier() + _entity.name);
                ResetDestroy(false); //Set Active false

                if (LevelStageSlot.Instance.isTutorial)
                {
                    if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_7_1"))
                    {
                        movement.AddComponent<TutorialCharacter>().currentID = "Sword_lv2_G3";
                        movement.GetComponent<TutorialCharacter>().TutorialAction();
                        TutorialStatge1.Instance.entityMovement = movement;
                        movement.health = 100;
                    }
                }
            }
            else
            {
                // เมื่อไม่มีหรือมากกว่า 3 Entity ให้ยืนเฉยๆ หรือทำการอื่นๆ
                foreach (EntityMovement entity in entitiesInFortress)
                {
                    entity.StopMoving();
                }
                Debug.Log("Entities in fortress standing idle.");
            }*/
        }

        public void OffensiveBarrackClick(EntityMovement movement)
        {
            //Set path ออกไปสู้ copy pathCombo2 มาใช้
            movement.Init();
            movement.InitStatus();
            movement.SetRunPath(GetStagePath("Entity-AttackGateTower"));
            movement.ResetScale();
            movement.ResetTargetRange();
            movement.PathFlipEntity();
            movement.baseType = BaseType.Offensive;
            ResetDestroy(true);
            comboEntity = null;
            entitiesInFortress.Clear();
            SetQuantity();
        }
        #endregion

        #region Paths
        private Dictionary<string, Transform[]> stagePathDic = new Dictionary<string, Transform[]>();
        public Transform[] GetStagePath(string id)
        {
            if (stagePathDic.ContainsKey(id))
            {
                return stagePathDic[id];
            }
            else
            {
                return stagePathDic[id] = stagePath.ToList().Find(o => o.id == id).paths;
            }
        }
        #endregion

        //เช็คว่าป้อมยังจะเอาตัวฮิโร่อยู่ไหม
        public bool CheckEntityOnFortress(EntityMovement _entity)
        {
            bool ishave = false;
            if (_entity.comboType == EntityComboType.Combo1)
            {
                targetEntitys.ToList().ForEach(o =>
                {
                    if (o.targetEntity != null && _entity.id == o.targetEntity.id)
                    {
                        ishave = true;
                    }
                });
            }
            else
            {
                if (comboEntity != null && comboEntity.GetComponent<EntityMovement>().id == _entity.id) ishave = true;
            }
            return ishave; 
        }

        //เช็คว่าป้อมยังจะเอาตัวฮิโร่อยู่ไหม
        public bool IsFullEntityOnFortress()
        {
            bool isOnFortress = false;
            if (entitiesInFortress.Count() >= 3) isOnFortress = false;
            else
            {
                targetEntitys.ToList().ForEach(o =>
                {
                    if (o.targetEntity.actionEntity == ActionEntity.InFotress)
                    {
                        isOnFortress = true;
                    }
                });
            }
            return isOnFortress;
        }

        public bool IsSubFortress()
        {
            if (this.GetComponent<SubFortressSlot>() != null) return true;
            else return false;
        }

        //Font หน้าป้อมเพื่อมาร์กจุดตี Enemy
        public Transform GetFightEnemyPoint()
        {
            if (targetEnemy != null)
            {
                if (fortressType == BarrackType.Merchant || fortressType == BarrackType.Monk)
                {
                    fightEnemy.transform.position = fontPos;
                }
                else fightEnemy.transform.position = targetEnemy.transform.position;
            }
            return fightEnemy.transform;
        }
    }
}
