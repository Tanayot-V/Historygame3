using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine;
using TowerDefense_Prototype;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using static SpineSkinModelSO;

namespace TowerDefense
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Attack")]
        public EnemyAttack attack;

        [Header("SpineSkin")]
        public SpineSkinModelSO spineSkinModelSO;
        public SpineEntitySkinByPath spineSkin;
        private SpineAnimationController animController;
        private EnemyAnimation anim;
        private SortingGroup sortingGroup;

        [Header("References")]
        public EnemyType enemyType;
        public EntityComboType comboType;
        public EnemyStatus status;

        [Header("Current Status")]
        public HealthFillView healthFillView;
        public ActionEntity actionEntity;
        public float health;
        public float damage;
        public float atkSpeed;
        public float moveSpeed;

        [Header("Attributes baseCharacter")]
        public float baseTargetRange;
        private Rigidbody2D rb;
        private Transform targetPath;
        public int gold;
        public bool isStopAttack;
        private bool isMoving = true;

        [Header("Bullet")]
        public Transform fightingPoint;
        public GameObject bulletPerfab;

        [Header("Path")]
        public Transform[] path;
        private int pathIndex = 0;
        private bool isEndPath;
        public bool isTownShip;

        [Header("Attribute Attck")]
        [SerializeField] private GameObject attackRange;
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private float targetRange;
        //[SerializeField] private float bps = 1f; //Bullets per second
        [SerializeField] private Transform targetEnemy;

        public List<Transform> hitListsEnemy = new List<Transform>();

        private bool isCallOnDieEvent = false;
        public UnityEvent OnDieEvent;

        public void InitStart()
        {
            //SpineSkin
            if (spineSkin == null) spineSkin = this.transform.GetChild(0).GetComponent<SpineEntitySkinByPath>();
            spineSkin.ChangeSkinFormPath(spineSkinModelSO.paths);

            //SpineSkin Animation
            if (animController == null) animController = this.GetComponent<SpineAnimationController>();
            actionEntity = ActionEntity.Run;
            animController.SetSpineAnimation(spineSkinModelSO);
            animController.ChangeAnimation(SpineAnimationType.Run);
            anim = this.GetComponent<EnemyAnimation>();

            attack = this.GetComponent<EnemyAttack>();
            attack.Init();

            rb = this.GetComponent<Rigidbody2D>();
            targetPath = path[pathIndex];//GameManager.Instance.EnemySpwan().enemyPath[pathIndex];
            targetRange = baseTargetRange;

            sortingGroup = this.GetComponent<SortingGroup>();
            //path = GameManager.Instance.EnemySpwan().enemyPath;

            //Status
            if(enemyType != EnemyType.Betroyal) SetStatus();
        }

        public void SetStatus()
        {
            status = GameManager.Instance.LevelStageData().statusModelSO.GetEnemyStatus(enemyType,comboType);
            spineSkinModelSO = GameManager.Instance.LevelStageData().statusModelSO.GetEnemyStatusModel(enemyType).spineSkinModelSO;

            health = status.hp;
            damage = status.dmg;
            atkSpeed = status.atkSpd;
            moveSpeed = status.moveSpd;
            gold = status.gold;
        }

        private void Update()
        {
            if (sortingGroup == null) sortingGroup = this.GetComponent<SortingGroup>();
            if (isStopAttack && actionEntity != ActionEntity.Idle)
            {
                if (targetEnemy == null)
                {
                    anim.Idle();
                    StopMoving();
                }
                return;
            }

            if (isStopAttack) return;
            //sortingGroup.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
            if (attack.IsDie())
            {
                if(!isCallOnDieEvent)
                {
                    OnDieEvent?.Invoke();
                    isCallOnDieEvent = true;
                }
                return;
            }

            if (targetEnemy != null)
            {
                //Direction Enemy
                Vector3 _direction = targetEnemy.position - transform.position;
                if (_direction.x > 0) transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
                else if (_direction.x < 0) transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);

                if (actionEntity != ActionEntity.AttackEnemy)
                {
                    if (targetEnemy.GetComponent<EntityMovement>() != null)
                    {
                        //เข้าใกล้ศัตรู
                        if(ChechTargetIsInRange())
                        {
                            Debug.Log(name + "ChechTargetIsInRange");
                            attack.Attack();
                        }
                        else
                        {
                            if (actionEntity != ActionEntity.Run)
                            {
                                StartMoving();
                                actionEntity = ActionEntity.Run;
                                animController.ChangeAnimation(SpineAnimationType.Run);
                            }

                            
                            //Debug.Log(name + "!ChechTargetIsInRange");
                        }
                    }
                }

                if (actionEntity != ActionEntity.AttackTower)
                {
                    if (targetEnemy.GetComponent<TowerGateSlot>() != null)
                    {
                        /*
                        if (this.GetComponent<TutorialCharacter>() != null && this.GetComponent<TutorialCharacter>().currentID == "Tutorial_2_1")
                        {
                            isStopAttack = true;
                            return;
                        }
                        */
                        if (ChechTargetIsInRange())
                        {
                            AttackTower(targetEnemy.GetComponent<TowerGateSlot>());
                        }
                    }
                }
            }
            else
            {
                FindTargetEnemy();

                if (actionEntity == ActionEntity.Idle) return;
                if (actionEntity != ActionEntity.Run)
                {
                    StartMoving();
                    actionEntity = ActionEntity.Run;
                    animController.ChangeAnimation(SpineAnimationType.Run);
                }
            }

            if (targetEnemy != null) return;
            if (!isMoving) return;
            if (Vector2.Distance(targetPath.position, transform.position) <= 0.1f)
            {
                pathIndex++;

                if (pathIndex == path.Length)
                {
                    if (isTownShip)
                    {
                        anim.Idle();
                        StopMoving();
                        return;  
                    } 
                    if(!isEndPath)
                    {
                        targetPath = AroundBarrack.Instance.GetRandomPointAroundBarrack();
                        isEndPath = true;
                    }
                }
                else if (pathIndex >= path.Length)
                {
                    pathIndex = path.Length;
                }
                else
                {
                    actionEntity = ActionEntity.Run;
                    targetPath = path[pathIndex];
                    FlipCharacter();
                }
            }

            void FlipCharacter()
            {
                // คำนวณทิศทางของการเคลื่อนที่
                Vector2 direction = (targetPath.position - transform.position).normalized;

                // เช็คว่าทิศทางการเคลื่อนที่อยู่ทางซ้ายหรือขวา เพื่อหันหน้าตัวละคร
                if (direction.x > 0)
                {
                    transform.localScale = new Vector2(-transform.localScale.x, transform.localScale.y);
                }
                else if (direction.x < 0)
                {
                    transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
                }
            }

            //ตีป้อม
            void AttackTower(TowerGateSlot _tower)
            {
                //anim.ChangeAnimation("slashing");
                StopMoving();
                animController.ChangeAnimation(SpineAnimationType.Attack);
                anim.AnimationSlashingTower(_tower);
                actionEntity = ActionEntity.AttackTower;
            }
        }

        private void FixedUpdate()
        {
            if (targetEnemy != null)
            {
                Vector2 direction = (targetEnemy.position - transform.position).normalized;
                rb.velocity = direction * moveSpeed;
            }

            if (targetEnemy == null && targetPath != null)
            {
                Vector2 direction = (targetPath.position - transform.position).normalized;
                rb.velocity = direction * moveSpeed;
            }
        }

        public void UpdateSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
        }

        public void ResetSpeed()
        {
            moveSpeed = status.moveSpd;
        }

        public void ResetTargetRange()
        {
            targetRange = baseTargetRange;
        }

        public void UpdateTargetRange(float _range)
        {
            targetRange = _range;
        }

        public void StopMoving()
        {
            isMoving = false; // หยุดการเคลื่อนไหว
            UpdateSpeed(0); // หยุดการเคลื่อนไหวทันที
        }

        public void StartMoving()
        {
            isMoving = true;
            ResetSpeed();
        }

        public void OnDestroy()
        {
            GameManager.Instance.GoldCost().IncreaseGold(gold);
            if (this.GetComponent<SubEntityMovement>() != null)
            {
                this.GetComponent<SubEntityMovement>().subBarrackSlot.OnSoldierDestroy(1);
            }
            Debug.Log(name + ": OnDestroy");
        }

        private bool ChechTargetIsInRange()
        {
            return Vector2.Distance(targetEnemy.position, transform.position) <= targetRange;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(attackRange.transform.position, transform.forward, targetRange);
            //Handles.DrawWireCube(attackRange.transform.position, targetRange * transform.localScale);
        }
#endif
        public void FindTargetEnemy()
        {
            hitListsEnemy.Clear();
            targetEnemy = null;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(attackRange.transform.position, targetRange, (Vector2)transform.position, 0f, enemyMask);
            /*
            Vector2 boxSize = new Vector2(targetRange* transform.lossyScale.x, targetRange * transform.lossyScale.y);
            // ตรวจจับ Collider ทั้งหมดภายในสี่เหลี่ยม
            Collider2D[] hits = Physics2D.OverlapBoxAll(
                attackRange.transform.position, // ตำแหน่งศูนย์กลางของสี่เหลี่ยม
                boxSize,                        // ขนาดของสี่เหลี่ยม (กว้าง, สูง)
                0f,                             // มุมหมุนของสี่เหลี่ยม (ถ้าต้องการให้หมุน)
                enemyMask                       // เลเยอร์ของเป้าหมายที่ต้องการตรวจจับ
            );*/


            if (hits.Length > 0)
            {
                hits.ToList().ForEach(o =>
                {
                     if (o.transform != null && o.transform.gameObject != null)
                    {
                        //ถ้าเป็น Tower จะไม่สามารถแอดได้
                        hitListsEnemy.Add(o.transform);
                    }
                });

                hitListsEnemy.RemoveAll(item => item == null || item.gameObject == null);

                if (hitListsEnemy.Count > 0)
                {
                    targetEnemy = hitListsEnemy[0];
                    foreach (var o in hitListsEnemy) // วนลูปผ่านลิสต์ชั่วคราวเพื่อเช็คข้อมูล
                    {
                        if (o != null && o.transform.gameObject != null)
                        {
                            // ตรวจสอบว่าคอมโพเนนต์ยังคงมีอยู่และไม่ถูกทำลาย
                            var enemyMovement = o.gameObject.GetComponent<EntityMovement>();
                            var enemyAttack = o.gameObject.GetComponent<EntityAttack>();

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

        public void OnAnimationComplete(TrackEntry trackEntry)
        {
            if (targetEnemy != null)
            {
                Debug.Log("_damage2: OnAnimationComplete");
                attack.TakeDamageEnemy(targetEnemy);
            }
            else
            {
                if (actionEntity == ActionEntity.AttackTower) return;
                animController.ChangeAnimation(SpineAnimationType.Run);
                StartMoving();
            }
        }
    }
}
