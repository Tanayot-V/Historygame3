using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillAmountAnimation : MonoBehaviour
{
    public Image fillImage; // อ้างอิงถึง UI Image ที่ต้องการปรับค่า fillAmount
    public float time = 2;

    public void Start()
    {
        StartCoroutine(FillOverTime(time));
    }

    private IEnumerator FillOverTime(float duration)
    {
        fillImage.fillAmount = 0;
        float elapsedTime = 0f;
        float startFill = fillImage.fillAmount;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fillImage.fillAmount = Mathf.Lerp(startFill, 1f, elapsedTime / duration);
            yield return null;
        }

        fillImage.fillAmount = 1f; // ตั้งค่าให้เต็ม 100% เมื่อครบเวลา
    }
}
