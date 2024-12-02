using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToUIPos : MonoBehaviour
{
    public Transform targetTransform;
    public float offset = 200;
    private float offsetmultiplyer;
    private static float startCamSize = -1f;
    private float calOffset;
    private Camera mCam;

    void Start()
    {
        mCam = Camera.main;
        if(startCamSize == -1f) startCamSize = mCam.orthographicSize;
    }

    void Update()
    {
        SetImagePositionToTransformWorld();
    }

    void SetImagePositionToTransformWorld()
    {
        if (targetTransform != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                this.GetComponent<RectTransform>().parent as RectTransform,
                screenPosition,
                Camera.main,
                out Vector2 localPoint);

            offsetmultiplyer = startCamSize / mCam.orthographicSize;
            calOffset = offset * offsetmultiplyer;

            localPoint.y += calOffset;
            this.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }
}
