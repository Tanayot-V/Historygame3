using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

namespace TowerDefense
{
    public class CannonSkill : MonoBehaviour
    {
        public float skillDamage = 10f;
        public float skillRange = 2f;
        public LayerMask whatIsEnemy;
        public GameObject bombFxPrefab;

        public bool isActiveSkill = false;

        public float cooldownTime;
        private float cooldownTimer;

        public Image skillImg;
        public Image cooldownFillImg;
        public TMP_Text cooldownText;

        public Sprite cancelingSprite;
        private Sprite originalSprite;
        public bool onTargetingState = false;

        private Vector3 debugLastPosition;

        public void Start()
        {
            originalSprite = skillImg.sprite;
            cooldownTimer = cooldownTime;
        }

        public void OmCannonBtnClick()
        {
            if (!isActiveSkill) return;
            if (onTargetingState)
            {
                onTargetingState = false;
                skillImg.sprite = originalSprite;
            }
            else
            {
                onTargetingState = true;
                skillImg.sprite = cancelingSprite;
            }
        }

        void Update()
        {
            if (GameManager.Instance.isStopStartgame) return;
            if (Input.GetMouseButtonDown(0))
            {
                if (UiController.IsPointerOverUIObject() || !onTargetingState || !isActiveSkill) return;
                Vector3 mousePosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0;
                KaBoommmm(worldPosition);
                SoundManager.Instance.PlayAudioSource("Bomb");
            }
            if (Input.touchCount > 0)
            {
                if (UiController.IsPointerOverUIObject() || !onTargetingState || !isActiveSkill) return;
                Vector3 mousePosition = Input.GetTouch(0).position;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
                worldPosition.z = 0;
                KaBoommmm(worldPosition);
            }

            if (cooldownTimer > 0f)
            {
                cooldownFillImg.fillAmount = cooldownTimer / cooldownTime;
                cooldownText.text = cooldownTimer.ToString("F0");
                cooldownTimer -= Time.deltaTime;
            }
            else
            {
                cooldownFillImg.fillAmount = 0f;
                cooldownText.text = "";
                isActiveSkill = true;
            }
        }

        private void KaBoommmm(Vector3 position)
        {
            debugLastPosition = position;
            Instantiate(bombFxPrefab, position, quaternion.identity);

            Collider2D[] allEnemiesInArea = Physics2D.OverlapCircleAll(position, skillRange, whatIsEnemy);
            foreach (Collider2D enemyCol in allEnemiesInArea)
            {
                enemyCol.gameObject.SendMessage("TakeDamage", skillDamage, SendMessageOptions.DontRequireReceiver);
            }

            isActiveSkill = false;
            cooldownTimer = cooldownTime;
            cooldownText.text = cooldownTimer.ToString("F0");

            onTargetingState = false;
            skillImg.sprite = originalSprite;
        }

        void OnDrawGizmos()
        {
            if (cooldownTimer > 0)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(debugLastPosition, skillRange);
            }
        }
    }
}
