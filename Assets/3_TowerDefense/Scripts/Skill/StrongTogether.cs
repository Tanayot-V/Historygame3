using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TowerDefense
{
    public class StrongTogether : MonoBehaviour
    {
        public int state = 2;
        public Image activeFillImg;
        public Image cooldownFillImg;
        public TMP_Text cooldownText;
        public ParticleSystem activeEffect;

        public float cooldownTime = 30f;
        private float cooldownTimer;
        public float effectTime = 10f;
        private float effectTimer;

        public float hpMultiplier;
        public float attackMultiplier;
        public float attackSpeedMultiplier;
        public float speedMovementMultiplier;

        public void Start()
        {
            state = 2;
            cooldownTimer = 30f;
        }

        public void OnSkillBtnClick()
        {
            if(state != 0) return;
            state = 1;
            effectTimer = effectTime;
            StartBuff();
        }

        void Update()
        {
            if (GameManager.Instance.isStopStartgame) return;
            if(state == 1)
            {
                cooldownText.text = effectTimer.ToString("F0");
                effectTimer -= Time.deltaTime;
                activeFillImg.fillAmount = effectTimer / effectTime;
                if(effectTimer <= 0f)
                {
                    EndBuff();
                    state = 2;
                    activeFillImg.fillAmount = 0;
                    cooldownTimer = cooldownTime;
                }
            }
            else if(state == 2)
            {
                cooldownText.text = cooldownTimer.ToString("F0");
                cooldownTimer -= Time.deltaTime;
                cooldownFillImg.fillAmount = cooldownTimer / cooldownTime;
                if(cooldownTimer <= 0f)
                {
                    state = 0;
                    cooldownFillImg.fillAmount = 0f;
                    cooldownText.text = "";
                }
            }
        }

        public void StartBuff()
        {
            activeEffect.Play();
            Transform sodlierParent = LevelStageSlot.Instance.heroSpwan.transform;
            if (LevelStageSlot.Instance.heroSpwan.transform.childCount > 0)
            {
                // วนลูปผ่านลูกทั้งหมดของ sodlierParent
                foreach (Transform child in sodlierParent)
                {
                    child.GetComponent<EntityMovement>().Buff(hpMultiplier,attackMultiplier,attackSpeedMultiplier,speedMovementMultiplier);
                }
            }
        }

        public void EndBuff()
        {
            activeEffect.Stop();
            Transform sodlierParent = LevelStageSlot.Instance.heroSpwan.transform;
            if (LevelStageSlot.Instance.heroSpwan.transform.childCount > 0)
            {
                // วนลูปผ่านลูกทั้งหมดของ sodlierParent
                foreach (Transform child in sodlierParent)
                {
                    child.GetComponent<EntityMovement>().BackToNormal();
                }
            }
        }
    }
}