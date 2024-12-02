using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using static SpineSkinModelSO;
using static UnityEngine.EventSystems.EventTrigger;

namespace TowerDefense
{
    public class EnemyAnimation : MonoBehaviour
    {
        private SpineAnimationController controller;
        private EnemyMovement enemy;
        void Start()
        {
            controller = this.GetComponent<SpineAnimationController>();
            enemy = this.GetComponent<EnemyMovement>();
        }

        public void Idle()
        {
            enemy.actionEntity = ActionEntity.Idle;
            controller.ChangeAnimation(SpineAnimationType.Idle);
            enemy.StopMoving();
        }


        public void Defensive()
        {
            enemy.actionEntity = ActionEntity.Defensive;
            controller.ChangeAnimation(SpineAnimationType.Defensive);
        }
        /*
        public void AnimationComplete()
        {
            if (controller.GetCurrentAnimationName(controller.currentAnimType.animationName))
            //if (GetCurrentAnimationName())
            {
                controller.skeletonAnimations[0].state.Complete -= this.GetComponent<EnemyMovement>().OnAnimationComplete;
                controller.skeletonAnimations[0].state.Complete += this.GetComponent<EnemyMovement>().OnAnimationComplete;
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
                enemy.OnAnimationComplete(trackEntry);
                Debug.Log($"[Spine] Event 'Enemy attak' triggered on Track {trackEntry.TrackIndex} at Time: {e.Time}");
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

        public void AnimationSlashingTower(TowerGateSlot _tower)
        {
            controller.skeletonAnimations[0].AnimationState.Event -= _OnSpineEvent;
            controller.skeletonAnimations[0].AnimationState.Event += _OnSpineEvent;

            void _OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
            {
                // ตรวจสอบว่า event ที่ถูกทริกเกอร์คือ 'step' หรือไม่
                if (e.Data.Name == "attak" || e.Data.Name == "attack")
                {
                    // แสดงข้อความใน Console
                    Debug.Log("Animation completed one loop: " + trackEntry.Animation.Name + "ตีป้อม");
                    //GameManager.Instance.LevelStageManager().levelStageSlot.TakeDamageTowerEntity(1);
                    _tower.TakeDamageTower((int)enemy.damage);
                    this.GetComponent<EnemyMovement>().FindTargetEnemy();
                }
            }
        }
    }
}
