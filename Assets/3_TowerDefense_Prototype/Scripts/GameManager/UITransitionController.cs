using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense_Prototype
{
    public class UITransitionController : MonoBehaviour
    {
        [SerializeField] float[] duration;
        public GameObject[] goObjs;

        void Start()
        {
            duration = UITransition.Instance.InitializeDurations();
        }
    }
}
