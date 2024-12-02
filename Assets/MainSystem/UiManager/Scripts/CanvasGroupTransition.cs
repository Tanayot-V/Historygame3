using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using Unity.VisualScripting;
using UnityEngine;

public class CanvasGroupTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration = 1.0f;

    private void Start() {}

    private CanvasGroup CanvasGroup()
    {
        if (canvasGroup == null)
        {
            canvasGroup = this.GetComponent<CanvasGroup>();
        }
        return canvasGroup;
    }

    public void FadeIn(System.Action callback = null)
    {
        CanvasGroup().alpha = 0;
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1, duration , callback));
    }

    public void FadeOut(System.Action callback = null)
    {
        CanvasGroup().alpha = 1;
        StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, duration , callback));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float lerpTime = 1.0f , System.Action callback = null)
    {
        float _timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - _timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - _timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }

        cg.interactable = end == 1;
        cg.blocksRaycasts = end == 1;
        if (callback != null) callback.Invoke();

    }
}
