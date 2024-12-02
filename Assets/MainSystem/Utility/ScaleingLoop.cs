using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ScaleingLoop : MonoBehaviour
{
    public float minSize,maxSize;
    public AnimationCurve sizeWave;
    public float duration;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        transform.localScale = Vector3.one * Mathf.Lerp(minSize,maxSize,sizeWave.Evaluate(0));
    }

    // Update is called once per frame
    void Update()
    {
        float progress = timer / duration;
        transform.localScale = Vector3.one * Mathf.Lerp(minSize,maxSize,sizeWave.Evaluate(progress));
        timer += Time.deltaTime;
        if(timer>= duration)
        {
            timer = 0f;
        }
    }
}
