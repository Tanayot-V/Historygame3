using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine;
using UnityEditor;
using UnityEngine;
using System.Linq;
using static SpineSkinModelSO;
using UnityEngine.Rendering;
using static UnityEngine.EventSystems.EventTrigger;
using System;
using UnityEngine.Events;

namespace TowerDefense
{
    public class EntityMovement : MonoBehaviour
    {
        public string id;
        public EntityComboType comboType;
        public BarrackType barrackType;
        public BaseType baseType;
        private LevelStageManager levelStageManager;

        [Header("SpineSkin")]
        public SpineSkinModelSO spineSkinModelSO;
        public SpineEntitySkinByPath[] spineSkins;
        private SpineAnimationController animController;
        public EntityAnimation anim;

        [Header("Attack")]
        public EntityAttack attack;
        public BarrackSlot barrackSlot;
        public FortressSlot fortressSlot;

        [Header("References")]
        //=> Status
        public EntityStatus baseStatus;
        private float baseSize = 0.5f;
        public float baseTargetRange;
        public int deadQuantity;
        private Rigidbody2D rb;
        public ActionEntity actionEntity;
        public PathFilpType pathFilpType;

        [Header("Attributes baseCharacter")]
        public HealthFillView healthFillView;
        public float health;
        public float healthMultiplier = 1f;
        public float moveSpeed;
        public float speedMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float atkSpeedMultiplier = 1f;
        private bool isMoving = true;
        public bool isBackFilpChar; //หันหน้าสวนทาง
        public List<EntityMovement> comboEntityList = new List<EntityMovement>(); //ตัวเล็กในคอมโบ

        [Header("Bullet")]
        public Transform fightingPoint;
        public GameObject bulletPerfab;

        private SortingGroup sortingGroup;

        [Header("Path")]
        private int pathIndex = 0;
        public Transform[] path;
        [SerializeField] private Transform targetPath;

        [Header("Attribute Attack")]
        [SerializeField] private GameObject attackRange;
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private float targetRange;
        //[SerializeField] private float bps = 1f; //Bullets per second
        public Transform targetEnemy;

        public List<Transform> hitListsEntity = new List<Transform>();
        public bool isDemolitionFortress; //ถอนป้อมตอนออกไปสู้ ด่านรับ
        public bool isStopAttack;

        private bool isCallOnDieEvent = false;
        public UnityEvent OnDieEvent;

        public void Init()
        {
            levelStageManager = GameManager.Instance.LevelStageManager();
            baseType = levelStageManager.currentLevelStageModelSO.levelStageModel.baseType;
            if(barrackSlot != null)
                barrackType = barrackSlot.barrackType;
            else
                barrackType = BarrackType.SoldierSword;

            //SpineSkin Animation
            animController = this.GetComponent<SpineAnimationController>();
            animController.SetSpineAnimation(spineSkinModelSO);

            anim = this.GetComponent<EntityAnimation>();
            anim.Init();

            attack = this.GetComponent<EntityAttack>();
            attack.Init();

            rb = this.GetComponent<Rigidbody2D>();
            ResetTargetRange();
            
            sortingGroup = GetComponent<SortingGroup>();
            if (sortingGroup == null)
            {
                sortingGroup = gameObject.AddComponent<SortingGroup>();
            }
        }

        public void InitStatus()
        {
            moveSpeed = baseStatus.moveSpd;
            targetRange = baseTargetRange;
            health = (int)baseStatus.hp;
            spineSkins.ToList().ForEach(o => { o.ChangeSkinFormPath(spineSkinModelSO.paths); });
        }

        public void SetBaseStatus(BarrackUpgrade _barrackUpgrade, int _indexCombo)
        {
            baseStatus = _barrackUpgrade.entityStatus[_indexCombo];
            spineSkinModelSO = _barrackUpgrade.spineSkinModelSO;
        }

        public void InitPath()
        {
            pathIndex = 0;
            if (path.Length > 0) 
            {
                targetPath = path[pathIndex];
            }
        }

        private void Update()
        {
            if (sortingGroup == null) sortingGroup = this.GetComponent<SortingGroup>();
            sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);

            if (isStopAttack)
            {
                if (actionEntity != ActionEntity.Idle) anim.Idle();
            }
            if (isStopAttack) return;

            if (attack.IsDie())
            {
                if(!isCallOnDieEvent)
                {
                    OnDieEvent?.Invoke();
                    isCallOnDieEvent = true;
                }
                return;
            }

            if (actionEntity == ActionEntity.Run && targetPath != null)
            {
                //Direction Enemy
                Vector3 _direction = targetPath.position - transform.position;
                if (_direction.x > 0) transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                else if (_direction.x < 0) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            if (barrackType == BarrackType.Merchant || barrackType == BarrackType.Monk) targetEnemy = null;
            else _UpdateEnemy();

            //ด่านบุก
            if (baseType == BaseType.Offensive)
            {
                UpdateOffensive();
                return;
            }
            //ด่านตั้งรับที่ฐาน
            else
            {
                UpdateDefensive();
                return;
            }

            void _UpdateEnemy()
            {
                //Debug.Log("Entity Action :" + name + actionEntity);
                if (targetEnemy != null)
                {
                    Debug.Log($"{name} : found Target {targetEnemy.name}!");
                    //Direction Enemy
                    Vector3 _direction = targetEnemy.position - transform.position;
                    if (_direction.x > 0) transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                    else if (_direction.x < 0) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);

                    if (barrackType == BarrackType.Merchant || barrackType == BarrackType.Monk) return;

                        //วิ่งตีมอนระหว่างทาง
                    if (actionEntity != ActionEntity.AttackEnemy && actionEntity != ActionEntity.InFotress)
                    {
                        if (ChechTargetIsInRange())
                        {
                            if (targetEnemy.GetComponent<EnemyMovement>() != null)
                            {
                                //Debug.Log(name + ": Attack enemy");
                                attack.AttackEnemy();
                                Debug.Log(name + ": try to targetEnemy AttackDefensive");
                            }
                        }
                    }
                    
                        /*
                        //ตีมอนหน้าประตูตัวเอง
                        if (!isMoving && actionEntity == ActionEntity.Defensive && actionEntity != ActionEntity.AttackDefensive && fortressSlot == null)
                        {
                            Debug.Log(name + ": AttackDefensive_1");
                            if (targetEnemy.GetComponent<EnemyMovement>() != null)
                            {
                                attack.AttackEnemyDefensive();
                                actionEntity = ActionEntity.AttackDefensive;
                                Debug.Log(name + ": AttackDefensive_2");
                            }
                        }

                        /*
                        else
                        {
                            if (actionEntity == ActionEntity.AttackTower || actionEntity == ActionEntity.InFotress) return;

                            StartMoving();
                            actionEntity = ActionEntity.Run;
                            anim.Run();

                            Vector2 direction = (targetEnemy.position - transform.position).normalized;
                            rb.velocity = direction * moveSpeed;
                            Debug.Log(name + ": Attack enemy move");
                        }*/

                     //ถ้ายืนอยู่ในป้อมแล้วศัตรูมาโจมตี Attack on fotress
                     if (actionEntity == ActionEntity.InFotress)
                    {
                        /*
                        Debug.Log(name + "ยืนในป้อมแต่มีศัตรูมาตี!!!");
                        if (fortressSlot != null && fortressSlot.targetEnemy != null)
                        {
                            fortressSlot.TakeAttackFortress(fortressSlot.GetStagePath("Entity-AttackGateTower"));
                        }
                        */
                        switch (baseType)
                        {
                            case BaseType.Offensive:
                                if (fortressSlot != null && fortressSlot.targetEnemy != null)
                                {
                                    fortressSlot.TakeAttackFortress(fortressSlot.GetStagePath("Entity-AttackGateTower"));
                                }
                                break;
                            case BaseType.Defensive:
                                if (fortressSlot != null && fortressSlot.targetEnemy != null)
                                {
                                    fortressSlot.TakeAttackFortress(new Transform[] { fortressSlot.GetFightEnemyPoint() });
                                }
                                break;
                        }

                    }

                    //ตี Tower
                    if (actionEntity != ActionEntity.AttackTower)
                    {
                        if (targetEnemy.GetComponent<TowerGateSlot>() != null)
                        {
                            if (ChechTargetIsInRange())
                            {
                                //ตีฐานป้อม
                                StopMoving();
                                anim.AnimationSlashingTower(targetEnemy.GetComponent<TowerGateSlot>());
                                anim.AttackTower();
                                Debug.Log(name + ":Attackitn Tower.");

                                #region Tutorial
                                if (LevelStageSlot.Instance.isTutorial)
                                {
                                    if (TutorialStatge1.Instance.IsCurrentTutorialID("Tutorial_4"))
                                    {
                                        if (this.GetComponent<TutorialCharacter>().currentID == "Sword_Lv2_G1")
                                        {
                                            StartCoroutine(UiController.Instance.WaitForSecond(2, () =>
                                            {
                                                this.GetComponent<TutorialCharacter>().TutorialAction();
                                                anim.Idle();
                                                isStopAttack = true;
                                                GameManager.Instance.LevelStageUI().towerFightUI.SetActive(false);
                                                TutorialUI.Instance.arrowHpEnemyUI.SetActive(true);
                                            }));
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                else
                {
                    FindTargetEnemy();

                    #region แก้บัคตีแล้วลื่น ห้ามเอาออก
                    if (actionEntity == ActionEntity.Defensive ||actionEntity == ActionEntity.InFotress || actionEntity == ActionEntity.FontFortress) return;
                    if (actionEntity != ActionEntity.Run && actionEntity != ActionEntity.InFotress)
                    {
                        StartMoving();
                        actionEntity = ActionEntity.Run;
                        anim.Run();
                    }
                    #endregion
                }
            }

            void UpdateOffensive()
            {
                if (targetPath == null)
                {
                    StopMoving();
                    anim.Idle();
                    return;
                }
                else
                {
                    //อยู่ Path ปัจจุบัน
                    if (Vector2.Distance(targetPath.position, transform.position) <= 0.1f)
                    {
                        _BackupPath();
                        pathIndex++;
                        //Debug.Log("pathAction OnFotress:_1" + name);

                        if (targetPath.GetComponent<PathFilpCharacter>() == null)
                        {
                            Vector3 _pathDirection = targetPath.position - transform.position;
                            if (_pathDirection.x > 0) transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                            else if (_pathDirection.x < 0) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                            Debug.Log(name + ": _pathDirection = " + _pathDirection);
                        }
                        else PathFlipEntity();

                        //อยู่ Path สุดท้าย
                        if (pathIndex >= path.Length)
                        {
                            pathIndex = path.Length - 1;
                            //Debug.Log("pathAction OnFotress:_2" + name + actionEntity);

                            if (targetPath.GetComponent<PathAction>() != null)
                            {
                                Debug.Log("pathAction OnFotress_0:" + name + actionEntity);

                                //จบเกม
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.EndGame)
                                {
                                   
                                }

                                /*
                                //อยู่หน้าประตูแล้วหาป้อมที่ว่าง
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.Defensive && actionEntity != ActionEntity.Defensive)
                                {
                                    actionEntity = ActionEntity.Defensive;
                                    FortressEmpty fortressEmpty = barrackSlot.IsFortressEmpty(this);
                                    //ไปป้อมที่ว่างอยู่
                                    if (fortressEmpty != null)
                                    {
                                        barrackSlot.FindFortressHero(this.gameObject, fortressEmpty);
                                        Debug.Log("ไปป้อมที่ว่างอยู่:" + name + actionEntity);
                                    }
                                    // เดินไปที่ประตูศัตรูเลย
                                    else
                                    {
                                        this.GetComponent<PathController>().ClearPath();
                                        this.GetComponent<PathController>().AddTransforms(levelStageManager.levelStageSlot.GetPathTransforms("Entity-EnemyGate-Path"));
                                        Debug.Log("วิ่งไปที่ประตู:" + name + actionEntity);
                                    }
                                    path = this.GetComponent<PathController>().GetPath();
                                    InitPath();
                                    anim.Run();
                                    return;
                                }*/

                                //หน้าป้อม ไม่มีศัตรูให้เข้าป้อม => เข้าป้อมบวกคอมโบ
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.FontFortress && actionEntity != ActionEntity.FontFortress)
                                {
                                    //Debug.Log(name + ":ก่อนเข้าป้อม");
                                    if (fortressSlot != null && fortressSlot.targetEnemy == null)
                                    {
                                        actionEntity = ActionEntity.FontFortress;
                                        SetRunPath(fortressSlot.GetComponent<FortressSlot>().GetStagePath("Entity-Fortress"));
                                        //Debug.Log(name + ":เข้าป้อม");
                                    }
                                    if (fortressSlot != null && fortressSlot.targetEnemy != null)
                                    {
                                        if (actionEntity == ActionEntity.InFotress) return;
                                        //Debug.Log(name + ":วิ่งยิกยักๆ ก่อนเข้าป้อม");
                                    }
                                    return;
                                }

                                //อยู่ในป้อมปราการ 
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.InFotress && actionEntity != ActionEntity.InFotress)
                                {
                                    Debug.Log("pathAction OnFotress_1:" + name + actionEntity);
                                    if (fortressSlot != null && fortressSlot.targetEnemy == null)
                                    {
                                        _OnFortress();
                                        actionEntity = ActionEntity.InFotress;
                                        Debug.Log("pathAction OnFotress_2:" + name + actionEntity);
                                        return;
                                    }
                                    if (fortressSlot != null && fortressSlot.targetEnemy != null)
                                    {
                                        Debug.Log(name + ":ในป้อมมีคัตรู !!");
                                        _RunFotress();
                                        //attack.AttackEnemy();
                                        //actionEntity = ActionEntity.InFotress;
                                        return;
                                    }
                                }
                            }
                        }

                        //ไป Path ถัดไป
                        else
                        {
                            targetPath = path[pathIndex];
                        }
                    }
                }
            }

            void UpdateDefensive()
            {
                if (targetPath == null)
                {
                    if (actionEntity != ActionEntity.Idle)
                    {
                        StopMoving();
                        anim.Idle();
                    }
                    return;
                }
                else
                {
                    //อยู่ Path ปัจจุบัน
                    if (Vector2.Distance(targetPath.position, transform.position) <= 0.1f)
                    {
                        _BackupPath();
                        pathIndex++;
                        //Debug.Log(name + targetPath.name);

                        if (targetPath.GetComponent<PathFilpCharacter>() == null)
                        {
                            Vector3 _pathDirection = targetPath.position - transform.position;
                            if (_pathDirection.x > 0) transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                            else if (_pathDirection.x < 0) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                            //Debug.Log(name + ": _pathDirection = " + _pathDirection);
                        }
                        else PathFlipEntity();

                        //อยู่ Path สุดท้าย
                        if (pathIndex >= path.Length)
                        {
                            pathIndex = path.Length - 1;
                            if(IsGeneral())
                            {
                                if (pathIndex >= path.Length - 1)
                                {
                                    pathIndex = 0;
                                }
                                else
                                {
                                    // ไปยัง path ถัดไป
                                    pathIndex++;
                                }
                            }

                            if (targetPath.GetComponent<PathAction>() != null)
                            {
                                /*
                                if (LevelStageSlot.Instance.isTutorial)
                                {
                                    //ยืนป้องกันตีมอนอยู่หน้าประตู
                                    if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.Defensive && actionEntity != ActionEntity.Defensive
                                        && actionEntity != ActionEntity.AttackDefensive && actionEntity != ActionEntity.InFotress && targetEnemy == null)
                                    {
                                        StopMoving();
                                        anim.Defensive();
                                        isBackFilpChar = false;
                                        PathFlipEntity();
                                        Debug.Log(name + ": ป้องกันประตู " + targetPath.name);
                                        //if (IsGeneral()) return;
                                    }
                                }*/

                                //หน้าป้อม ไม่มีศัตรูให้เข้าป้อม => เข้าป้อมบวกคอมโบ
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.FontFortress && actionEntity != ActionEntity.FontFortress)
                                {
                                    //Debug.Log(name + ":ก่อนเข้าป้อม");
                                    //เข้าป้อมแล้วป้อมโดนถอนไปแล้ว
                                    if (fortressSlot != null && fortressSlot.fortressType == BarrackType.Empty && !fortressSlot.CheckEntityOnFortress(this) && isDemolitionFortress)
                                    {
                                        StartCoroutine(NothingFortress());
                                        isDemolitionFortress = false;
                                        Debug.Log(name + ":เข้าป้อมแล้วป้อมไม่อยู่แล้ว");
                                    }

                                    //เข้าป้อมแบบไม่มีศัตรู
                                    if (fortressSlot != null && fortressSlot.targetEnemy == null)
                                    {
                                        actionEntity = ActionEntity.FontFortress;
                                        path = fortressSlot.GetComponent<FortressSlot>().GetStagePath("Entity-Fortress");
                                        this.InitPath();
                                        anim.Run();
                                        Debug.Log(name + ":เข้าป้อม");
                                    }
                                    if (fortressSlot != null && fortressSlot.targetEnemy != null)
                                    {
                                        if (targetEnemy == null) targetEnemy = fortressSlot.targetEnemy;
                                        _RunFotress();
                                        Debug.Log(name + ":วิ่งยิกยักๆ ก่อนเข้าป้อม");
                                    }
                                    return;
                                }

                                //อยู่ในป้อมปราการ 
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.InFotress && actionEntity != ActionEntity.InFotress)
                                {
                                    Debug.Log("pathAction OnFotress_1:" + name + actionEntity);
                                    if (fortressSlot != null && fortressSlot.targetEnemy == null)
                                    {
                                        //คอมโบสองและสามกลับเข้าป้อม
                                        if (comboType == EntityComboType.Combo2 || comboType == EntityComboType.Combo3)
                                        {
                                            StopMoving();
                                            anim.InFotress();
                                            UpdateScale(fortressSlot.entity_scaleInFortress);
                                            PathFlipEntity();
                                            UpdateTargetRange(fortressSlot.entity_rangeInFortress);
                                            Debug.Log("pathAction OnFotress_1:" + name + actionEntity);
                                        }
                                        //คอมโบเดียวเข้าไปบวกคอมโบในป้อม
                                        else
                                        {
                                            _OnFortress();
                                            PathFlipEntity();
                                            Debug.Log(name + "คอมโบเดียวเข้าไปบวกคอมโบในป้อม");
                                        }

                                        actionEntity = ActionEntity.InFotress;
                                    }

                                    //if (fortressSlot != null && fortressSlot.targetEnemy != null && targetEnemy == null)
                                    if (fortressSlot != null && fortressSlot.targetEnemy != null)
                                    {
                                        if (barrackType == BarrackType.Merchant || barrackType == BarrackType.Monk) targetEnemy = null;
                                        if (targetEnemy == null) targetEnemy = fortressSlot.targetEnemy;
                                        _RunFotress();
                                        //Debug.Log(name + "อยู่ในป้อมปราการ ตัวเองไม่มีศัตรู แต่ป้อมมีศัตรู ");
                                    }
                                }

                                //ยืนป้องกันตีมอนอยู่หน้าประตู
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.Defensive && actionEntity != ActionEntity.Defensive
                                    && actionEntity != ActionEntity.AttackDefensive && actionEntity != ActionEntity.InFotress && targetEnemy == null)
                                {
                                    StopMoving();
                                    anim.Defensive();
                                    isBackFilpChar = false;
                                    PathFlipEntity();
                                    //Debug.Log(name + ": ป้องกันประตู " + targetPath.name);
                                    //if (IsGeneral()) return;
                                }

                                //กลับเมืองแล้วซ่อนตัว
                                if (targetPath.GetComponent<PathAction>().pathAction == ActionEntity.InBarrack && fortressSlot == null)
                                {
                                    Destroy(this.gameObject);
                                }
                            }
                        }
                        //ไป Path ถัดไป
                        else
                        {
                            targetPath = path[pathIndex];
                        }
                    }
                }
            }

            //เดินเข้าป้อมไปคอมโบ
            void _OnFortress()
            {
                //ถ้าไม่ได้มาร์กในป้อมต้องเดินออก
                if (baseType == BaseType.Offensive && !fortressSlot.CheckEntityOnFortress(this))
                {
                    _RunFotress();
                    Debug.Log("CheckEntityOnFortress: Offensive");
                    return;
                }

                Debug.Log(name + "CheckEntityOnFortress:" + fortressSlot.CheckEntityOnFortress(this));
                //ถ้าไม่ได้มาร์กในป้อมต้องเดินออก
                if (baseType == BaseType.Defensive && !fortressSlot.CheckEntityOnFortress(this))
                {
                    StartCoroutine(NothingFortress());
                    Debug.Log("CheckEntityOnFortress: Defensive");
                    return;
                }

                switch (baseType)
                {
                    case BaseType.Offensive:
                        Debug.Log("OnFortress Offensive");
                        fortressSlot.OffensiveBarrack(this);
                        break;
                    case BaseType.Defensive:
                        Debug.Log("OnFortress Defensive");
                        fortressSlot.DefensiveBarrack(this);
                        break;
                }
                return;
            }

            //อยู่หน้าป้อมศัตรูมาโจมตี แล้วทำอะไรต่อ
            void _RunFotress()
            {
                if (baseType == BaseType.Offensive)
                {
                    Debug.Log("RunFotress");
                    actionEntity = ActionEntity.Run;
                    if (fortressSlot != null)
                    {
                        path = fortressSlot.GetComponent<FortressSlot>().GetStagePath("Entity-AttackGateTower");
                        fortressSlot.SetQuantity();
                    }
                    InitPath();
                    anim.Run();
                    StartMoving();
                    fortressSlot.ResetEntityFortress(this);
                    ResetTargetRange();
                }
                if (baseType == BaseType.Defensive)
                {
                    Debug.Log("RunFotress");
                    actionEntity = ActionEntity.Run;
                    if (fortressSlot != null)
                    {
                        path = new Transform[] { fortressSlot.GetFightEnemyPoint() };
                        //path = fortressSlot.GetComponent<FortressSlot>().GetStagePath("Entity-FontFortress");
                        fortressSlot.SetQuantity();
                    }
                    InitPath();
                    anim.Run();
                    StartMoving();
                    ResetTargetRange();
                }
            }

            void _BackupPath()
            {
                if (targetPath != null && !(pathIndex >= path.Length))
                {
                    if (!this.GetComponent<PathController>().backupPaths.Contains(targetPath))
                    {
                        this.GetComponent<PathController>().backupPaths.Add(targetPath);
                    }
                    /*
                    if (targetPath.GetComponent<PathAction>() != null)
                    {
                        if (targetPath.GetComponent<PathAction>().pathAction != ActionEntity.InBarrack)
                        {
                            if (!this.GetComponent<PathController>().backupPaths.Contains(targetPath))
                            {
                                this.GetComponent<PathController>().backupPaths.Add(targetPath);
                            }
                        }
                    }
                    else
                    {
                        if (!this.GetComponent<PathController>().backupPaths.Contains(targetPath))
                        {
                            this.GetComponent<PathController>().backupPaths.Add(targetPath);
                        }
                    }
                    */
                }
            }
        }

        //หาป้อมให้ผู้บ่าวแบบมีป้อมมาให้
        public void GotoFortress(FortressEmpty _fortressEmpty)
        {
            if (!IsSubEntity())
            {
                if (IsGeneral()) return;
                //ไปป้อมที่ว่างอยู่
                if (_fortressEmpty != null)
                {
                    barrackSlot.FindFortressHero(this.gameObject, _fortressEmpty);
                    SetRunPath(this.GetComponent<PathController>().GetPath());
                    Debug.Log(name + ": หาป้อมเจอแล้ว เดินไปป้อม");
                }
            }
            else
            {
                //ไปป้อมที่ว่างอยู่
                if (_fortressEmpty != null)
                {
                    this.GetComponent<SubEntityMovement>().subBarrackSlot.FindFortressHero(this.gameObject, _fortressEmpty);
                    SetRunPath(this.GetComponent<PathController>().GetPath());
                    Debug.Log(name + ": หาป้อมเจอแล้ว เดินไปป้อม");
                }
            }
        }

        //หาป้อมให้ผู้บ่าวแบบไม่มีป้อมมาให้
        public void GotoFortress()
        {
            if (!IsSubEntity())
            {
                if (IsGeneral()) return;
                //ไปป้อมที่ว่างอยู่
                FortressEmpty fortressEmpty = barrackSlot.IsFortressEmpty(this);
                if (fortressEmpty != null)
                {
                    barrackSlot.FindFortressHero(this.gameObject, fortressEmpty);
                    path = this.GetComponent<PathController>().GetPath();
                    InitPath();
                    anim.Run();
                    Debug.Log(name + ": หาป้อมเจอแล้ว เดินไปป้อม");
                }
            }
            else
            {
                //ไปป้อมที่ว่างอยู่
                FortressEmpty fortressEmpty = this.GetComponent<SubEntityMovement>().subBarrackSlot.IsFortressEmpty(this);
                if (fortressEmpty != null)
                {
                    this.GetComponent<SubEntityMovement>().subBarrackSlot.FindFortressHero(this.gameObject, fortressEmpty);
                    path = this.GetComponent<PathController>().GetPath();
                    InitPath();
                    anim.Run();
                    Debug.Log(name + ": หาป้อมเจอแล้ว เดินไปป้อม");
                }
            }
        }

        public void PathFlipEntity()
        {
            //Vector2 direction = (targetPath.position - transform.position).normalized;
            if (targetPath == null) return;
            if (targetPath.GetComponent<PathFilpCharacter>() != null)
            {
                FlipCharacter(targetPath.GetComponent<PathFilpCharacter>().pathFilpType);
            }
        }

        public void FlipCharacter(PathFilpType _pathFilpType)
        {
            if (targetEnemy != null) return;
            switch (_pathFilpType)
            {
                case PathFilpType.Right:
                    if (!isBackFilpChar)
                    {
                        pathFilpType = PathFilpType.Right;
                        transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                    }
                    else
                    {
                        pathFilpType = PathFilpType.Left ;
                        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                    }
                    break;
                case PathFilpType.Left:
                    if (!isBackFilpChar)
                    {
                        pathFilpType = PathFilpType.Left;
                        transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
                    }
                    else
                    {
                        pathFilpType = PathFilpType.Right;
                        transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                    }
                    break;
            }
        }

        public void SetRunPath(Transform[] _paths)
        {
            path = _paths;
            this.InitPath();
            anim.Run();
        }

        //ถ้าไม่มีป้อมของตัวเองต้องเดินออก
        public IEnumerator NothingFortress()
        {
            if (this.comboType == EntityComboType.Combo1)
            {
                Debug.Log("NothingFortress_Combo1_1");
                if (fortressSlot != null) fortressSlot = null;
                this.GetComponent<PathController>().ClearPath();
                this.GetComponent<PathController>().backupPaths.Reverse();
                this.GetComponent<PathController>().AddTransforms(this.GetComponent<PathController>().backupPaths.ToArray());
                this.GetComponent<PathController>().PathRemoveAction(ActionEntity.InFotress);
                path = this.GetComponent<PathController>().GetPath();
                this.GetComponent<PathController>().ClearPath();
                InitPath();
                //isBackFilpChar = true;
                PathFlipEntity();
                anim.Run();
            }
            else
            {
                Debug.Log("NothingFortress_2");
                foreach (var spine in spineSkins)
                {
                    spine.gameObject.SetActive(false);
                }

                //ถอนป้อมแบบหลายตัว
                foreach (var o in comboEntityList.ToList())
                {
                    if (o != null)
                    {
                        if (o.actionEntity == ActionEntity.InFotress)
                        {
                            Debug.Log("กดถอนป้อมแบบด่านรับ: " + o.name);
                            o.transform.position = this.transform.position;
                            o.gameObject.SetActive(true);

                            o.Init();
                            o.path = fortressSlot.GetStagePath("Entity-BackBarrack");
                            o.InitPath();
                            o.path.ToList().RemoveAt(0); //ไม่ต้องเดินเข้าป้อม
                            o.PathFlipEntity();
                            o.anim.Run();
                            o.ResetScale();
                            //if (o.baseType == BaseType.Defensive) o.isBackFilpChar = true;
                            o.fortressSlot = null;

                            // หน่วงเวลา 0.5 วินาที
                            yield return new WaitForSeconds(0.25f);
                        }
                    }
                }
                this.GetComponent<EntityMovement>().deadQuantity = 0;
                Destroy(this.gameObject);
            }
        }

        //ตั้งรับอยู่ที่ป้อม
        public void OnDefensive()
        {
            StopMoving();
            anim.Defensive();
            UpdateTargetRange(fortressSlot.entity_rangeInFortress);
        }

        private void FixedUpdate()
        {
            if (targetEnemy != null)
            {
                Vector2 direction = (targetEnemy.position - transform.position).normalized;
                if(fortressSlot != null)
                {
                    direction *= fortressSlot.statusMultiplier;
                }
                rb.velocity = direction * moveSpeed * speedMultiplier;
            }

            if (targetPath != null)
            {
                Vector2 direction = (targetPath.position - transform.position).normalized;
                if(fortressSlot != null)
                {
                    direction *= fortressSlot.statusMultiplier;
                }
                rb.velocity = direction * moveSpeed * speedMultiplier;
            }
        }

        public void UpdateSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
        }

        public void ResetSpeed()
        {
            moveSpeed = baseStatus.moveSpd;
        }

        public void UpdateScale(float _scale)
        {
            transform.localScale = new Vector3(_scale, _scale, _scale);
        }

        public void ResetScale()
        {
            transform.localScale = new Vector3(baseSize, baseSize, baseSize);
        }

        public void ResetTargetRange()
        {
            targetRange = baseTargetRange;
        }

        public void UpdateTargetRange(float _range)
        {
            targetRange = _range;
        }

        public void Buff(float hpM, float dmgM, float atkSM, float speedM)
        {
            healthMultiplier = hpM;
            damageMultiplier = dmgM;
            atkSpeedMultiplier = atkSM;
            speedMultiplier = speedM;
        }

        public void BackToNormal()
        {
            healthMultiplier = 1f;
            damageMultiplier = 1f;
            atkSpeedMultiplier = 1f;
            speedMultiplier = 1f;
        }

        //ตรวจสอบวงโจมตี ว่าศัครูอยู่ในวงไหม
        private bool ChechTargetIsInRange()
        {
            return Vector2.Distance(targetEnemy.position, transform.position) <= targetRange;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.cyan;
            //Handles.DrawWireCube(attackRange.transform.position, targetRange * transform.localScale);
            Handles.DrawWireDisc(attackRange.transform.position, transform.forward, targetRange);
        }
#endif
        public void FindTargetEnemy()
        {
            if (barrackType == BarrackType.Monk || barrackType == BarrackType.Merchant) targetEnemy = null;
            if (barrackType == BarrackType.Monk || barrackType == BarrackType.Merchant) return;
            hitListsEntity.Clear();
            targetEnemy = null;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(attackRange.transform.position, targetRange, (Vector2)transform.position, 0f, enemyMask);
            /*
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                attackRange.transform.position, 
                targetRange * (Vector2)transform.localScale, 
                0f,                             
                enemyMask                       
            );

            float scaledTargetRange = targetRange * transform.lossyScale.x; // คำนวณ targetRange โดยคูณกับสเกล (เลือก axis ที่เหมาะสม)
            RaycastHit2D[] hits = Physics2D.CircleCastAll(
                attackRange.transform.position,
                scaledTargetRange, // ใช้ range ที่คำนวณแล้ว
                Vector2.zero, // ทิศทางการยิงเป็น Vector2.zero
                0f,
                enemyMask
            );*/

            if (hits.Length > 0)
            {
                hits.ToList().ForEach(o =>
                {
                    if (o.transform != null && o.transform.gameObject != null)
                    {
                        hitListsEntity.Add(o.transform);
                    }
                });

                hitListsEntity.RemoveAll(item => item == null || item.gameObject == null);

                if (hitListsEntity.Count > 0)
                {
                    targetEnemy = hitListsEntity[0];
                    foreach (var o in hitListsEntity) // วนลูปผ่านลิสต์ชั่วคราวเพื่อเช็คข้อมูล
                    {
                        if (o != null && o.gameObject != null)
                        {
                            // ตรวจสอบว่าคอมโพเนนต์ยังคงมีอยู่และไม่ถูกทำลาย
                            var enemyMovement = o.gameObject.GetComponent<EnemyMovement>();
                            var enemyAttack = o.gameObject.GetComponent<EnemyAttack>();

                            if (enemyMovement != null && enemyAttack != null)
                            {
                                if (!enemyAttack.IsDie())
                                {
                                    targetEnemy = o; // ตั้งเป้าหมายใหม่
                                }
                            }
                        }
                    }
                }
            }
        }

        public void StopMoving()
        {
            isMoving = false; // หยุดการเคลื่อนไหว
            moveSpeed = 0; // หยุดการเคลื่อนไหวทันที
        }

        public void StartMoving()
        {
            isMoving = true; 
            moveSpeed = baseStatus.moveSpd; 
        }

        public void Shoot()
        {
            GameObject bulletObj = Instantiate(bulletPerfab, fightingPoint.position, Quaternion.identity);
            Bullet bulletSpript = bulletObj.GetComponent<Bullet>();
            bulletSpript.SetTarget(targetEnemy,this.gameObject);
        }

        //หลังจากตีศัตรูเสร็จแล้ว
        public void OnAnimationComplete(TrackEntry trackEntry)
        {
            if (targetEnemy != null)
            {
                Debug.Log("_damage2: OnAnimationComplete");
                if (IsGeneral()) this.GetComponent<GeneralMovement>().AOEAttack(); //attack.TakeDamageRangeEnemy(this.GetComponent<GaneralMovement>().targetRangeEnemy);
                if (IsGeneral()) return;

                attack.TakeDamageEnemy(targetEnemy);
                /*
                if (barrackType == BarrackType.SoldierSword) attack.TakeDamageEnemy(targetEnemy);
                if (barrackType == BarrackType.SoldierGun) Shoot();
                */
            }
            else
            {
                if (isDemolitionFortress)
                {
                    StartCoroutine(NothingFortress());
                    isDemolitionFortress = false;
                    Debug.Log("isDemolitionFortress:" + isDemolitionFortress);
                    return;
                }

                if (actionEntity == ActionEntity.AttackTower) return;
                if (actionEntity == ActionEntity.InFotress) return;
                /*
                if (actionEntity == ActionEntity.AttackDefensive || actionEntity == ActionEntity.Defensive)
                {
                    anim.Defensive();
                    actionEntity = ActionEntity.Defensive;
                    if (barrackSlot.IsFortressEmpty(this) != null) GotoFortress();
                    Debug.Log(name + ": back to Defensive");
                    return;
                }
                */
                animController.ChangeAnimation(SpineAnimationType.Run);
                StartMoving();
                //Debug.Log("OnAnimationComplete");
            }
        }

        public void OnDestroy()
        {
            //เช็คก่อนว่าจำนวนถูกไหม
            if (barrackSlot != null)
            {
                if (IsGeneral())
                {
                    barrackSlot.isGeneral = false;
                    return;
                }

                bool iscomboDead = comboType == EntityComboType.Combo3 || comboType == EntityComboType.Combo5 || comboType == EntityComboType.Combo7;
                //ตายแล้วป้อมตัวเองพัง
                if (iscomboDead && attack.IsDie())
                {
                    if (fortressSlot != null)
                    {
                        fortressSlot.SetDefault();
                    }
                }

                //ถ้ายังยืดถือกับป้อม ด่านรับแบบออกมาโจมตีแล้วตาย => ตายแล้วรีเซตป้อม
                if (baseType == BaseType.Defensive)
                {
                    if (comboType != EntityComboType.Combo1 && attack.IsDie())
                    {
                        comboEntityList.ToList().ForEach(o => {
                            if (o != null)
                            {
                                o.deadQuantity = 0;
                                Destroy(o.gameObject);
                            }
                        });
                    }
                }

                if (healthFillView != null) Destroy(healthFillView.gameObject);
                barrackSlot.OnSoldierDestroy(deadQuantity);

                /*
                //ถ้ายังยืดถือกับป้อม ด่านรับแบบออกมาโจมตีแล้วตาย => ตายแล้วรีเซตป้อม
                if (baseType == BaseType.Defensive)
                {
                    if (comboType != EntityComboType.Combo1 && attack.IsDie())
                    {
                        comboEntityList.ToList().ForEach(o => {
                            if (o != null)
                            {
                                o.deadQuantity = 0;
                                Destroy(o.gameObject);
                            }
                        });
                    }
                }
                if (fortressSlot != null) fortressSlot.isAdded = false;
                if (!IsSubEntity()) barrackSlot.OnSoldierDestroy(deadQuantity);
                else this.GetComponent<SubEntityMovement>().subBarrackSlot.OnSoldierDestroy(deadQuantity);
                */
                Debug.Log(name + ": OnDestroy : "+ deadQuantity);
            }
        }

        public bool IsSubEntity()
        {
            if (this.GetComponent<SubEntityMovement>() != null) return true;
            else return false;
        }

        public bool IsGeneral()
        {
            if (spineSkinModelSO.id == "Sword_General") return true;
            else return false;
        }    
    }
}
