using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class UITransitionController : MonoBehaviour
    {
        [SerializeField] float[] duration;
        public GameObject[] goObjs;

        void Start()
        {
            duration = UITransition.Instance.InitializeDurations();
        }

        public void FortressOptionUIShow(GameObject _obj)
        {
            if (_obj.activeSelf) return;
            _obj.SetActive(true);
            Time.timeScale = 0;
            /*
            UITransition.Instance.SlideOneY(_obj, new Vector2(0, -1500), new Vector2(0, 0), duration[13],()=> {
                Time.timeScale = 0;
            });*/
        }

        public void FortressOptionUIHide(GameObject _obj)
        {
            Time.timeScale = 1;
            _obj.SetActive(false);
            /*
            UITransition.Instance.SlideOneY(_obj, new Vector2(0, 0), new Vector2(-1500, -1500), duration[13],()=> {
            });
            */
        }
    }
}
