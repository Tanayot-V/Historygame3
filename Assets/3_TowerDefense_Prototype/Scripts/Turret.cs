using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TowerDefense_Prototype
{
    public class Turret : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] Tower tower;

        [Header("References")]
        [SerializeField] private Transform turretRotationPoint;
        [SerializeField] private LayerMask enemyMask;
        [SerializeField] private GameObject bulletPerfab;
        [SerializeField] private Transform firingPoint;

        [Header("Attribute")]
        [SerializeField] private float targetingRange = 5f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float bps = 1f; //Bullets per second

        private Transform target;
        private float timeUntilFire;

        private void Update()
        {
            if (target == null)
            {
                FindTarget();
                return;
            }

            RotatTowersTarget();

            if (!ChechTargetIsInRange())
            {
                target = null;
            }
            else
            {
                timeUntilFire += Time.deltaTime;
                if (timeUntilFire >= 1f / bps)
                {
                    Shoot();
                    timeUntilFire = 0f;
                }
            }
        }

        private void Shoot()
        {
            GameObject bulletObj = Instantiate(bulletPerfab, firingPoint.position, Quaternion.identity);
            Bullet bulletSpript = bulletObj.GetComponent<Bullet>();
            bulletSpript.SetTarget(target);
        }

        private void FindTarget()
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, targetingRange, (Vector2)transform.position, 0f, enemyMask);

            if (hits.Length > 0)
            {
                if (hits[0].collider.GetComponent<EnemyMovement>().GetTowerType() == tower.towerType
                    || hits[0].collider.GetComponent<EnemyMovement>().GetTowerType() == TowerType.generic)
                {
                    target = hits[0].transform;
                }
            }
        }

        private void RotatTowersTarget()
        {
            float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x)
                * Mathf.Rad2Deg - 90f;

            Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        private bool ChechTargetIsInRange()
        {
            return Vector2.Distance(target.position, transform.position) <= targetingRange;
        }
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, transform.forward, targetingRange);
        }
#endif
    }
}
