using System.Collections;
using System.Collections.Generic;
using TowerDefense_Prototype;
using Unity.VisualScripting;
using UnityEngine;
using static SpineSkinModelSO;
using static UnityEngine.EventSystems.EventTrigger;

namespace TowerDefense
{
    public class EnemyAttack : MonoBehaviour
    {
        private EnemyMovement enemy;
        private SpineAnimationController animController;
        private EnemyAnimation anim;

        public void Init()
        {
            enemy = this.GetComponent<EnemyMovement>(); ;
            animController = this.GetComponent<SpineAnimationController>();
            anim = this.GetComponent<EnemyAnimation>();
        }

        public void Attack()
        {
            enemy.actionEntity = ActionEntity.AttackEnemy;
            enemy.StopMoving();

            animController.ChangeAnimation(SpineAnimationType.Attack, enemy.atkSpeed);
            anim.SpineEventAttack();
            //anim.AnimationComplete();
        }

        //โดนดาเมจ
        public void TakeDamage(float _damage)
        {
            Debug.Log("_damage:" + _damage);
            enemy.health -= _damage;
            if (enemy.healthFillView != null) enemy.healthFillView.hp_current = enemy.health;
            if (enemy.health <= 0)
            {
                enemy.StopMoving();
                animController.DeadAnim();
                if (enemy.healthFillView != null) Destroy(enemy.healthFillView.gameObject);
                StartCoroutine(UiController.Instance.WaitForSecond(1f, () => {
                    if (this.GetComponent<TutorialCharacter>() != null) this.GetComponent<TutorialCharacter>().TutorialAction();
                    Destroy(gameObject);
                }));
            }
        }

        public bool IsDie()
        {
            if (enemy != null) return enemy.health <= 0;
            return true;
        }

        //โจมตีใส่ศัตรู
        public void TakeDamageEnemy(Transform _targetEnemy)
        {
            Debug.Log(name + ": TakeDamageEnemy " + enemy.damage);
            if (_targetEnemy != null)
            {
                if (_targetEnemy.GetComponent<EntityAttack>() != null)
                {
                    if (!_targetEnemy.GetComponent<EntityAttack>().IsDie())
                    {
                        _targetEnemy.GetComponent<EntityAttack>().TakeDamage(enemy.damage);
                        animController.ChangeAnimation(SpineAnimationType.Attack, enemy.status.atkSpd);
                        Debug.Log(name + ": Enemy hasTakenDamage");
                    }
                }
            }
        }
    }
}
