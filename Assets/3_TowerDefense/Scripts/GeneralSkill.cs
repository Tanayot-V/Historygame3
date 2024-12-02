using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class GeneralSkill : MonoBehaviour
    {
        public string id;
        [SerializeField] private Image fillImage;
        [SerializeField] private TMPro.TextMeshProUGUI timeTX;
        [SerializeField] private TMPro.TextMeshProUGUI amountTX;
        [SerializeField] private float cooldownDuration;
        [SerializeField] private int currentAmount;
        [SerializeField] private int maxAmount;

        private bool isCooldownActive = false;

        [Header("General")]
        [SerializeField] BarrackSlot generalBarrack;
        [SerializeField] private GameObject postGeneral;
        [SerializeField] private float cooldownGeneralDuration = 3;
        [SerializeField] private Image fillPostGeneral;

        public void Start()
        {
            amountTX.text = currentAmount + "/" + maxAmount;
            //StartCooldown();
        }
        public void StartCooldown()
        {
            if (!isCooldownActive)
            {
                StartCoroutine(CooldownRoutine());
            }
        }

        private IEnumerator CooldownRoutine()
        {
            isCooldownActive = true;

            float elapsedTime = 0f;

            while (elapsedTime < cooldownDuration)
            {
                elapsedTime += Time.deltaTime;

                // คำนวณ Fill Amount
                float fillAmount = 1 - (elapsedTime / cooldownDuration);
                fillImage.fillAmount = fillAmount;

                float timeRemaining = cooldownDuration - elapsedTime;
                timeTX.text = Mathf.Ceil(timeRemaining).ToString();
                
                yield return null; 
            }

            fillImage.fillAmount = 0f;
            timeTX.text = string.Empty;
            isCooldownActive = false;
        }

        public bool IsMoreThanItemCurrent()
        {
            return currentAmount >= 1;
        }

        public void CellGeneralButton()
        {
            if (isCooldownActive) return;
            if (generalBarrack.isGeneral) return;
            if (!IsMoreThanItemCurrent()) return;
            if (GameManager.Instance.isStopStartgame) return;

            currentAmount -= 1;
            if (currentAmount <= 0) currentAmount = 0;
            amountTX.text = currentAmount + "/" + maxAmount;

            postGeneral.SetActive(true);
            StartCooldown();
            StartCoroutine(WaitGeneral());
        }

        public IEnumerator WaitGeneral()
        {
            float elapsedTime = 0f;
            while (elapsedTime < cooldownGeneralDuration)
            {
                elapsedTime += Time.deltaTime;

                // คำนวณ Fill Amount
                float fillAmount = 1 - (elapsedTime / cooldownGeneralDuration);
                fillPostGeneral.fillAmount = fillAmount;
                yield return null;
            }
            fillPostGeneral.fillAmount = 0f;
            postGeneral.SetActive(false);
            LevelStageLevel4.Instance.General();
        }
    }
}
