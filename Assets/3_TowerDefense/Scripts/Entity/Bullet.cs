using System.Collections;
using System.Collections.Generic;
using TowerDefense_Prototype;
using UnityEngine;

namespace TowerDefense
{
    public class Bullet : MonoBehaviour
    {
        [Header("Referencese")]
        public GameObject ownerBullet;
        [SerializeField] private Rigidbody2D rb;

        [Header("Attributes")]
        [SerializeField] private float bulletSpeed = 5f; //bulletSpeed

        private Transform target;

        public void SetTarget(Transform _target, GameObject _ownerBullet)
        {
            target = _target;
            ownerBullet = _ownerBullet;
        }

        public void Start()
        {
            StartCoroutine(CheckIfAlive());
        }

        private void FixedUpdate()
        {
            if (!target) return;

            Vector2 direction = (target.position - transform.position).normalized;
            direction.y += 0.5f;
            direction = direction.normalized;
            rb.velocity = direction * bulletSpeed;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            ownerBullet.GetComponent<EntityAttack>().TakeDamageEnemy(target);
            Debug.Log(other.collider.name +" :Bullet");
            //other.gameObject.GetComponent<Health>().TakeDamage(bulletDamage);
            Destroy(this.gameObject);
        }

        IEnumerator CheckIfAlive()
        {
            yield return new WaitForSeconds(1f);
            Destroy(this.gameObject);
        }
    }
}
