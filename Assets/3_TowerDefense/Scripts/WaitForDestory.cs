using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForDestory : MonoBehaviour
{
    [SerializeField] float time;
    void Start()
    {
        StartCoroutine(
        UiController.Instance.WaitForSecond(time , ()=> {
            Destroy(this.gameObject);
        }));
    }
}
