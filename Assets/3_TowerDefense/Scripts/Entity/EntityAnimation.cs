using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static SpineSkinModelSO;

namespace TowerDefense
{
    public class EntityAnimation : MonoBehaviour
    {
        private SpineAnimationController controller;
        private EntityMovement entity;

        public void Init()
        {
            controller = this.GetComponent<SpineAnimationController>();
            entity = this.GetComponent<EntityMovement>();
        }

        public void Idle()
        {
            entity.actionEntity = ActionEntity.Idle;
            controller.ChangeAnimation(SpineAnimationType.Idle);
            entity.StopMoving();
        }

        public void Run(float _timeScale = 1f)
        {
            if (entity == null) Init();
            entity.actionEntity = ActionEntity.Run;
            controller.ChangeAnimation(SpineAnimationType.Run, _timeScale);
            entity.StartMoving();
        }

        public void AttackEnemyDefensive()
        {
            entity.actionEntity = ActionEntity.AttackDefensive;
            controller.ChangeAnimation(SpineAnimationType.Attack);
        }

        public void AttackEnemy()
        {
            entity.actionEntity = ActionEntity.AttackEnemy;
            controller.ChangeAnimation(SpineAnimationType.Attack);
        }

        public void AttackTower()
        {
            entity.actionEntity = ActionEntity.AttackTower;
            controller.ChangeAnimation(SpineAnimationType.Attack);
        }

        public void Defensive()
        {
            entity.actionEntity = ActionEntity.Defensive;
            controller.ChangeAnimation(SpineAnimationType.Defensive);
        }

        public void InFotress()
        {
            if (entity.actionEntity == ActionEntity.InFotress) return;
            entity.actionEntity = ActionEntity.InFotress;
            controller.ChangeAnimation(SpineAnimationType.Defensive);
        }
        /*
        public void AnimationComplete()
        {
            if (controller.GetCurrentAnimationName(controller.currentAnimType.animationName))
            //if (GetCurrentAnimationName())
            {
                Debug.Log("_damage1: OnAnimationComplete");
                controller.skeletonAnimations[0].state.Complete -= OnAnimationComplete;
                controller.skeletonAnimations[0].state.Complete += entity.OnAnimationComplete;
                //if (entity.barrackType == BarrackType.SoldierGun) entity.Shoot();
                //Invoke("DealDamage", 0.5f);

                void OnAnimationComplete(TrackEntry trackEntry)
                {
                    controller.skeletonAnimations[0].state.Complete -= OnAnimationComplete;
                }                
            }
        }
        */
        public void SpineEventAttack()
        {
            controller.skeletonAnimations[0].AnimationState.Event -= OnSpineEvent;
            controller.skeletonAnimations[0].AnimationState.Event += OnSpineEvent;
        }

        void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
            // ตรวจสอบว่า event ที่ถูกทริกเกอร์คือ 'step' หรือไม่
            if (e.Data.Name == "attak" || e.Data.Name == "attack")
            {
                entity.OnAnimationComplete(trackEntry);
                Debug.Log($"[Spine] Event 'attak' triggered on Track {trackEntry.TrackIndex} at Time: {e.Time}");
            }
        }

        public void AnimationSlashingTower(TowerGateSlot _tower)
        {
            controller.skeletonAnimations[0].AnimationState.Event -= _OnSpineEvent;
            controller.skeletonAnimations[0].AnimationState.Event += _OnSpineEvent;

            void _OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
            {
                // ตรวจสอบว่า event ที่ถูกทริกเกอร์คือ 'step' หรือไม่
                if (e.Data.Name == "attak" || e.Data.Name == "attack")
                {
                    _tower.TakeDamageTower((int)entity.baseStatus.dmg);
                    //AttackTower();
                    entity.FindTargetEnemy();
                    Debug.Log($"[Spine] Event 'attak' triggered on Track {trackEntry.TrackIndex} at Time: {e.Time}");
                }
            }
        }

        void OnDestroy()
        {
            // ยกเลิกการสมัครเมื่อ GameObject นี้ถูกทำลาย เพื่อป้องกันบัค
            if (controller.skeletonAnimations[0] != null)
            {
                controller.skeletonAnimations[0].AnimationState.Event -= OnSpineEvent;
                Debug.Log("skeletonAnimations OnSpineEvent");
            }
        }
    }
}
