using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TowerDefense_Prototype
{
    public class Menu : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI currencyUI;
        [SerializeField] GameObject menu;

        private bool isMenuOpen = true;

        public void ToggleMenu()
        {
            if (isMenuOpen)
            {
                menu.SetActive(false);
                isMenuOpen = false;
            }
            else
            {
                menu.SetActive(true);
                isMenuOpen = true;
            }
        }

        private void OnGUI()
        {
            currencyUI.text = LevelManager.main.currency.ToString();
        }

        public void SetSelected()
        {

        }
    }
}
