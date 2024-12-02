using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense
{
    public class VolumeSetting : Singletons<VolumeSetting>
    {
        public Image volumeImg;
        public void Start()
        {
            volumeImg.gameObject.SetActive(false);
        }
        public void SetVolume(float _volume)
        {
            if (SoundManager.Instance.volume == 0)
            {
                SoundManager.Instance.SetVolume(1);
                volumeImg.gameObject.SetActive(false);
            }
            else if (SoundManager.Instance.volume == 1)
            {
                SoundManager.Instance.SetVolume(0);
                volumeImg.gameObject.SetActive(true);
            }
        }
    }
}
