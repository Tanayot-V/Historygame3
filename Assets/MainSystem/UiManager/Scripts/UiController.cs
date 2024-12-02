using System.Collections;
using System.Collections.Generic;
using TowerDefense;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiController : MonoBehaviour
{
    private static UiController _Instance;
    public static UiController Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("[Script]: -- UiController Instance --");
                _Instance = obj.AddComponent<UiController>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }

    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (RaycastResult r in results)
        {
            if (r.gameObject.GetComponent<RectTransform>() != null)
                return true;
        }
        return false;
    }

    public void DestorySlot(GameObject tf)
    {
        if (tf == null)
        {
            Debug.LogWarning("DestorySlot is null");
            return;
        }

        if (tf.transform.childCount > 0)
        {
            for (int i = 0; i < tf.transform.childCount; i++)
            {
                Destroy(tf.transform.GetChild(i).gameObject);
            }
        }
    }

    public void HideChild(GameObject tf)
    {
        if (tf == null)
        {
            Debug.LogWarning("DestorySlot is null");
            return;
        }

        if (tf.transform.childCount > 0)
        {
            for (int i = 0; i < tf.transform.childCount; i++)
            {
                tf.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator WaitForSecond(float _seconds, System.Action callback = null)
    {
        yield return new WaitForSeconds(_seconds);

        callback?.Invoke();
    }

    public IEnumerator WaitForSecondRealtime(float seconds, System.Action callback)
    {
        yield return new WaitForSecondsRealtime(seconds);
        callback?.Invoke();
    }

    public Color SetColorWithHex(string _hexColor)
    {
        Color color;

        if (ColorUtility.TryParseHtmlString(_hexColor, out color))
        {
            return color;
        }
        else
        {
            Debug.LogError("Unable to parse hex string to Color.");
        }
        return color;
    }

    public GameObject InstantiateUIView(GameObject _prefab,GameObject _parent = null)
    {
        GameObject prefab = Instantiate(_prefab);
        if(_parent != null) prefab.transform.SetParent(_parent.transform);
        prefab.GetComponent<RectTransform>().localScale = Vector3.one;
        return prefab;
    }
}
