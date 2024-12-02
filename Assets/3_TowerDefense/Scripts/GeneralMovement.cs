using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using TowerDefense_Prototype;
using UnityEngine;

namespace TowerDefense
{
    public class GeneralMovement : MonoBehaviour
    {
        [SerializeField] private float targetingRange = 3.25f;
        [SerializeField] private LayerMask enemyMask;

        public void AOEAttack()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetingRange, enemyMask);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Collider2D hit = hits[i];

                    var eh = hit.transform.GetComponent<EnemyAttack>();
                    if (eh != null)
                    {
                        if (!eh.GetComponent<EnemyAttack>().IsDie())
                        {
                            eh.TakeDamage(this.GetComponent<EntityMovement>().baseStatus.dmg);
                            Debug.Log(eh.name + " : General Attacked.");
                        }
                    }
                }
            }

        }
    }
}
