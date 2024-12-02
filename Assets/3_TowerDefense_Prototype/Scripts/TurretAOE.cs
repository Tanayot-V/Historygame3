using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class TurretAOE : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LayerMask enemyMask;

        [Header("Attributes")]
        [SerializeField] private float targetingRange = 5f;
        [SerializeField] private float aps = 4f; // attacks per second
        [SerializeField] private int damageAmount = 10;

        private float timeUntilFire;
        [SerializeField] float timeUntilFireCooldown = 5f;//Cooldown ในการ AOE รอบถัดไป

        private void Update()
        {
            timeUntilFire += Time.deltaTime;

            if (timeUntilFire >= timeUntilFireCooldown / aps)
            {
                Debug.Log("AOE Attack");
                AOEAttack();
                timeUntilFire = 0f;
            }
        }

        // Tower Defense branch
        private void AOEAttack()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, targetingRange, enemyMask);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    Collider2D hit = hits[i];

                    Health eh = hit.transform.GetComponent<Health>();
                    if (eh != null)
                    {
                        eh.TakeDamage(damageAmount);
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, targetingRange);
        }
    }
}
