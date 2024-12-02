using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class PopupUI : MonoBehaviour
    {
        [SerializeField] string popupID;
        [SerializeField] TMPro.TextMeshProUGUI desTX;

        private void SetPopupUI(string _id,string _des)
        {
            popupID = _id;
            //desTX.text = _des;
        }

        public void Show(string _id,string _des)
        {
            SetPopupUI(_id,_des);
            this.gameObject.SetActive(true);
        }

        public void ButtonNext()
        {
            switch (popupID)
            {
                case "Level4_Betroyal":
                    //LevelStageLevel4.Instance.Betroyal();
                    break;
            }
            this.gameObject.SetActive(false);
        }
    }
}
