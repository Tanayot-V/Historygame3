using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UITransition : MonoBehaviour
{
    private static UITransition _Instance;
    public static UITransition Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("[Script]: -- UIAnimation Instance --");
                _Instance = obj.AddComponent<UITransition>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }
    Tween myTween;

    public float[] InitializeDurations()
    {
        float[] duration;
        duration = new float[18];
        for (int i = 0; i < duration.Length; i++)
        {
            duration[i] = 1f - i * 0.05f;
        }
        return duration;
    }

    //ค่อยๆ Scale ทีละอัน _ZoomIn
    public void ScaleList_ZoomIn(List<GameObject> _goList, float _duration = 1f, System.Action callback = null)
    {
        if (_goList == null)
        {
            Debug.Log(_goList + "Gameobject null");
            return;
        }

        _goList.ForEach(o => {
            o.SetActive(true);
            Vector3 scale = o.GetComponent<RectTransform>().localScale;
            o.GetComponent<RectTransform>().localScale = Vector3.zero;
            o.GetComponent<RectTransform>().DOScale(scale, _duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
            {
                if (callback != null) callback();
            });
        });
    }

    public void ScaleOneSet(GameObject _go, Vector3 _startScale, Vector3 _targetScale, float _duration = 1f, System.Action callback = null)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        _go.SetActive(true);
        Vector3 scale = _go.GetComponent<RectTransform>().localScale;
        _go.GetComponent<RectTransform>().localScale = _startScale;
        _go.GetComponent<RectTransform>().DOScale(_targetScale, _duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
        {
            if (callback != null) callback();
        });
    }

    public void ScaleOneIn(GameObject _go, float _duration = 1f, System.Action callback = null)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        _go.SetActive(true);
        Vector3 scale = _go.GetComponent<RectTransform>().localScale;
        _go.GetComponent<RectTransform>().localScale = Vector3.zero;
        _go.GetComponent<RectTransform>().DOScale(scale, _duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
        {
            if (callback != null) callback();
        });
    }

    //ค่อยๆ Scale ทีละอัน _ZoomIn
    public void ScaleList_ZoomOut(List<GameObject> _goList, float _duration = 1f, System.Action callback = null)
    {
        if (_goList == null)
        {
            Debug.Log(_goList + "Gameobject null");
            return;
        }

        _goList.ForEach(o => {
            o.SetActive(true);
            Vector3 scale = o.GetComponent<RectTransform>().localScale;
            o.GetComponent<RectTransform>().localScale = scale;
            o.GetComponent<RectTransform>().DOScale(0, _duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
            {
                if (callback != null) callback();
            });
        });
    }

    public void ScaleOneOut(GameObject _go, float _duration = 1f, System.Action callback = null)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        _go.SetActive(true);
        Vector3 scale = _go.GetComponent<RectTransform>().localScale;
        _go.GetComponent<RectTransform>().localScale = scale;
        _go.GetComponent<RectTransform>().DOScale(0, _duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
        {
            if (callback != null) callback();
        });
    }
    public void ScaleList(List<GameObject> _goList, Vector3 _startScale, Vector3 _targetScale, float _duration = 1f, System.Action callback = null)
    {
        if (_goList == null)
        {
            Debug.Log(_goList + "Gameobject null");
            return;
        }

        _goList.ForEach(o => {
            o.SetActive(true);
            o.GetComponent<RectTransform>().localScale = _startScale;
            o.GetComponent<RectTransform>().localScale = Vector3.zero;
            o.GetComponent<RectTransform>().DOScale(_targetScale, _duration).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
            {
                if (callback != null) callback();
            });
        });
    }

    public void SlideList(List<GameObject> _goList, Vector2 _startPos, Vector2 _targetPos, float _duration = 1f, System.Action callback = null)
    {
        if (_goList == null)
        {
            Debug.Log(_goList + "Gameobject null");
            return;
        }

        _goList.ForEach(o => {
            o.SetActive(true);
            Vector3 originalPos = o.GetComponent<RectTransform>().localPosition;
            o.GetComponent<RectTransform>().localPosition = _startPos;
            o.GetComponent<RectTransform>().DOAnchorPos(_targetPos, _duration).SetUpdate(true).OnComplete(() => {
                if (callback != null) callback();
            });
        });
    }

    public void SlideOne(GameObject _go, Vector2 _startPos, Vector2 _targetPos, float _duration = 1f, System.Action callback = null)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        if (myTween != null)
        {
            if (DOTween.IsTweening(myTween)) return;
        }

        GameObject o = _go;
        Vector2 originalPosition = _go.GetComponent<RectTransform>().localPosition;
        o.GetComponent<RectTransform>().localPosition = _startPos;
        myTween = o.GetComponent<RectTransform>().DOAnchorPos(_targetPos, _duration).SetUpdate(true).OnComplete(() => {
            if (callback != null) callback();
        });
    }

    public void SlideOneY(GameObject _go, Vector2 _startPos, Vector2 _targetPos, float _duration = 1f, System.Action callback = null)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        GameObject o = _go;
        o.GetComponent<RectTransform>().localPosition = _startPos;
        o.GetComponent<RectTransform>().DOAnchorPosY(_targetPos.y, _duration).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
        {
            callback?.Invoke();
        });
    }

    public void SlideListY(List<GameObject> _goList, Vector2 _startPos, Vector2 _targetPos, float _duration = 1f, System.Action callback = null)
    {
        if (_goList == null)
        {
            Debug.Log(_goList + "Gameobject null");
            return;
        }

        _goList.ForEach(o => {
            o.SetActive(true);
            Vector3 pos = o.GetComponent<RectTransform>().localPosition;
            o.GetComponent<RectTransform>().localPosition = _startPos;
            o.GetComponent<RectTransform>().DOAnchorPosY(_targetPos.y, _duration).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
            {
                if (callback != null) callback();
            });
        });
    }

    public void CanvasGroupAlpha(GameObject _go, float _alphaStart = 0, float _alphaTarget = 1, float _duration = 1f, System.Action callback = null)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        if (myTween != null)
        {
            if (DOTween.IsTweening(myTween)) return;
        }

        if (_go.GetComponent<CanvasGroup>() == null) _go.AddComponent<CanvasGroup>();
        _go.GetComponent<CanvasGroup>().alpha = _alphaStart;

        // ใช้ DOTween เพื่อทำแอนิเมชันเปลี่ยนค่า Alpha จาก 0 เป็น 1 ในระยะเวลาที่กำหนด
        _go.GetComponent<CanvasGroup>().DOFade(_alphaTarget, _duration).SetUpdate(true).OnComplete(() =>
        {
            // เมื่อแอนิเมชันเสร็จสิ้น ทำงานอื่นๆ ที่คุณต้องการ
            if (callback != null) callback();
        });
    }

    public void CanvasGroupAlphaList(List<GameObject> _goList, float _alphaStart = 0, float _alphaTarget = 1, float _duration = 1f, System.Action callback = null)
    {
        if (_goList == null)
        {
            Debug.Log(_goList + "Gameobject null");
            return;
        }

        _goList.ForEach(o => {
            if (o.GetComponent<CanvasGroup>() == null) o.AddComponent<CanvasGroup>();
            o.GetComponent<CanvasGroup>().alpha = _alphaStart;
            // ใช้ DOTween เพื่อทำแอนิเมชันเปลี่ยนค่า Alpha จาก 0 เป็น 1 ในระยะเวลาที่กำหนด
            o.GetComponent<CanvasGroup>().DOFade(_alphaTarget, _duration).SetUpdate(true).OnComplete(() =>
            {
                // เมื่อแอนิเมชันเสร็จสิ้น ทำงานอื่นๆ ที่คุณต้องการ
                if (callback != null) callback();
            });

        });
    }

    public void TweenRotation(GameObject _go, float _duration)
    {
        if (_go == null)
        {
            Debug.Log(_go + "Gameobject null");
            return;
        }

        Vector3 rotation = _go.GetComponent<RectTransform>().rotation.eulerAngles;
        rotation.z = 100f;
        _go.GetComponent<RectTransform>().rotation = Quaternion.Euler(rotation);
        _go.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 0), _duration).SetUpdate(true);
        if (_go.GetComponent<CanvasGroup>() != null)
        {
            _go.GetComponent<CanvasGroup>().alpha = 0;
            _go.GetComponent<CanvasGroup>().DOFade(1, _duration);
        }
    }
}
