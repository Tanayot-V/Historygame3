using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class HealthFillView : MonoBehaviour
    {
        public Image imgFill;
        public float hp_current;
        public float hp_Max;

        public void SetInit(Transform _transform,float _hp_current,float _hp_Max,string _id)
        {
            this.GetComponent<WorldToUIPos>().offset = 70;
            this.GetComponent<WorldToUIPos>().targetTransform = _transform;
            hp_current = _hp_current;
            hp_Max = _hp_Max;

            switch (_id)
            {
                case "entity":
                    imgFill.color = UiController.Instance.SetColorWithHex("#51C50E");
                    break;
                case "enemy":
                    imgFill.color = UiController.Instance.SetColorWithHex("#C52A0E");
                    break;
            }
        }

        void Update()
        {
            imgFill.fillAmount = hp_current / hp_Max;
        }

        public void InstantiateSoldier(EntityMovement soldier)
        {
            if (soldier.barrackType == BarrackType.SoldierSword || soldier.barrackType == BarrackType.SoldierGun)
            {
                GameObject healthFill = UiController.Instance.InstantiateUIView(LevelStageSlot.Instance.healthGO, LevelStageSlot.Instance.healthEntityGroup);
                healthFill.GetComponent<HealthFillView>().SetInit(soldier.transform, soldier.health, soldier.baseStatus.hp, "entity");
                soldier.healthFillView = healthFill.GetComponent<HealthFillView>();
            }
        }
    }
}
