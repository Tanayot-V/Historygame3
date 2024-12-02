using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class Plot : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Color hoverColor;

        private GameObject tower;
        private Color startColor;

        private void Start()
        {
            sr = this.GetComponent<SpriteRenderer>();
            startColor = sr.color;
        }
        private void OnMouseEnter()
        {
            sr.color = hoverColor;
        }
        private void OnMouseExit()
        {
            sr.color = startColor;
        }
        private void OnMouseDown()
        {
            if (UiController.IsPointerOverUIObject()) return;
            if (tower != null) return;
            
            Tower towerToBuild = BuildManager.main.GetSlectedTower();
            if (towerToBuild.cost > LevelManager.main.currency)
            {
                Debug.Log("You can't afford this tower");
                return;
            }

            LevelManager.main.SpendCurrency(towerToBuild.cost);
            tower = Instantiate(towerToBuild.prefab, transform.position,Quaternion.identity);
        }
    }
}
