using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class LevelStageLevel0 : MonoBehaviour
    {
        [SerializeField] GameObject startGO;

        public void StartGameButton(string _name)
        {
            DataCenterManager.Instance.LoadSceneByName(_name);
        }
    }
}
