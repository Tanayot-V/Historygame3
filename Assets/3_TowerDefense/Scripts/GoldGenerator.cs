using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class GoldGenerator : MonoBehaviour
    {
        private int goldPerSecond = 1; // จำนวนเงินที่เจนในแต่ละรอบ
        private float timer = 0f; // ตัวจับเวลา
        private int totalGold = 0; // จำนวนเงินทั้งหมด
        private bool isStart;
        public EntityComboType type;

        public void StartGoldGeneration(EntityComboType _type)
        {
            isStart = true;
            type = _type;
        }

        void Update()
        {
            if (GameManager.Instance.isStopStartgame) return;
            if (!isStart) return;
            switch (type)
            {
                case EntityComboType.Combo1:
                    goldPerSecond = 0;
                    break;
                case EntityComboType.Combo2:
                    goldPerSecond = 0;
                    break;
                case EntityComboType.Combo3:
                    goldPerSecond = 1;
                    break;
                case EntityComboType.Combo4:
                    goldPerSecond = 1;
                    break;
                case EntityComboType.Combo5:
                    goldPerSecond = 2;
                    break;
                case EntityComboType.Combo6:
                    goldPerSecond = 6;
                    break;
                case EntityComboType.Combo7:
                    goldPerSecond = 7;
                    break;
                default:
                    goldPerSecond = 0; // ค่าเริ่มต้นหากไม่มีในกรณีที่ระบุ
                    break;
            }

            // เพิ่มค่าของตัวจับเวลา
            timer += Time.deltaTime;

            // ตรวจสอบว่าผ่านไป 1 วินาทีแล้วหรือยัง
            if (timer >= 1f)
            {
                // สร้างเงินและรีเซ็ตตัวจับเวลา
                totalGold += goldPerSecond;
                timer = 0f;

                GoldCost.Instance.IncreaseGold(goldPerSecond);
                // แสดงผลใน Debug
                Debug.Log("Gold generated! Total gold: " + totalGold);

                if (goldPerSecond > 0)
                {
                    GameObject gold = GoldCost.Instance.CreateGoldUI(goldPerSecond);
                    gold.GetComponent<WorldToUIPos>().targetTransform = this.transform;
                    gold.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "+" + goldPerSecond;
                }
            }
        }
    }
}
