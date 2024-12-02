using System.Collections;
using System.Collections.Generic;
using TowerDefense_Prototype;
using Unity.VisualScripting;
using UnityEngine;
using static SpineSkinModelSO;

namespace TowerDefense
{
    public class EntityAttack : MonoBehaviour
    {
        private EntityMovement entity;
        private SpineAnimationController animController;
        private EntityAnimation anim;

        public void Init()
        {
            entity = this.GetComponent<EntityMovement>(); ;
            animController = this.GetComponent<SpineAnimationController>();
            anim = this.GetComponent<EntityAnimation>();
        }

        public void AttackEnemyDefensive()
        {
            entity.StopMoving();
            anim.AttackEnemyDefensive();
            //anim.AnimationComplete();
            anim.SpineEventAttack();
        }

        public void AttackEnemy()
        {
            entity.StopMoving();
            anim.AttackEnemy();
            //anim.AnimationComplete();
            anim.SpineEventAttack();
        }

        //โดนดาเมจ
        public void TakeDamage(float _damage)
        {
            float realHealth = entity.health * entity.healthMultiplier;
            if(entity.fortressSlot != null)
            {
                realHealth *= entity.fortressSlot.statusMultiplier;
            }
            if (realHealth <= 0) return;
            entity.health -= _damage;
            realHealth = entity.health * entity.healthMultiplier;
            if(entity.fortressSlot != null)
            {
                realHealth *= entity.fortressSlot.statusMultiplier;
            }
            if (entity.healthFillView != null) entity.healthFillView.hp_current = realHealth;
            if (realHealth <= 0)
            {
                entity.StopMoving();
                animController.DeadAnim();
                if (entity.healthFillView != null) Destroy(entity.healthFillView.gameObject);
                StartCoroutine(UiController.Instance.WaitForSecond(1, () => {
                    if (this.GetComponent<TutorialCharacter>() != null)
                    {
                        this.GetComponent<TutorialCharacter>().TutorialAction();
                        if (entity.targetEnemy != null && entity.targetEnemy.GetComponent<EnemyMovement>())
                        {
                            entity.targetEnemy.GetComponent<EnemyMovement>().isStopAttack = true;
                        }
                    }
                    Destroy(gameObject);
                }));
            }
        }

        public bool IsDie()
        {
            float realHealth = entity.health * entity.healthMultiplier;
            if(entity.fortressSlot != null)
            {
                realHealth *= entity.fortressSlot.statusMultiplier;
            }
            if (entity != null) return realHealth <= 0;
            else return true;
        }

        public void TakeDamageEnemy(Transform _targetEnemy)
        {
            if (_targetEnemy != null)
            {
                if (_targetEnemy.GetComponent<EnemyAttack>() != null)
                {
                    if (!_targetEnemy.GetComponent<EnemyAttack>().IsDie())
                    {
                        float realDmg = entity.baseStatus.dmg * entity.damageMultiplier;
                        float realAtkSpeed = entity.baseStatus.atkSpd * entity.atkSpeedMultiplier;
                        if(entity.fortressSlot != null)
                        {
                            realDmg *= entity.fortressSlot.statusMultiplier;
                            realAtkSpeed *= entity.fortressSlot.statusMultiplier;
                        }
                        _targetEnemy.GetComponent<EnemyAttack>().TakeDamage((int)realDmg);
                        animController.ChangeAnimation(SpineAnimationType.Attack, realAtkSpeed);
                        Debug.Log(name + ": hasTakenDamage");
                    }
                }
            }
        }

        public void TakeDamageRangeEnemy(List<Transform> _targetEnemys)
        {
            if (_targetEnemys.Count > 0)
            {
                _targetEnemys.ForEach(o => {
                    if (o.GetComponent<EnemyAttack>() != null)
                    {
                        if (!o.GetComponent<EnemyAttack>().IsDie())
                        {
                            float realDmg = entity.baseStatus.dmg * entity.damageMultiplier;
                            float realAtkSpeed = entity.baseStatus.atkSpd * entity.atkSpeedMultiplier;
                            if(entity.fortressSlot != null)
                            {
                                realDmg *= entity.fortressSlot.statusMultiplier;
                                realAtkSpeed *= entity.fortressSlot.statusMultiplier;
                            }
                            o.GetComponent<EnemyAttack>().TakeDamage((int)realDmg);
                            animController.ChangeAnimation(SpineAnimationType.Attack, realAtkSpeed);
                            Debug.Log(name + ": hasTakenDamage General Range");
                        }
                    }
                });
            }
        }
    }
}
