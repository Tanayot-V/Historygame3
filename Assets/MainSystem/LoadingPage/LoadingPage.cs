using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingPage : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject loadingParentObj;

    [Header("Prefebs")]
    public GameObject start_Prefab_Loading;
    [SerializeField] GameObject mini_Prefab_Loading;
    [SerializeField] GameObject game2_Prefab_Loading;
    [SerializeField] GameObject big_Prefab_Loading;

    public void ShowMiniLoading(float _seconds)
    {
        mini_Prefab_Loading.SetActive(true);
        loadingParentObj.SetActive(true);
        StartCoroutine(UiController.Instance.WaitForSecond(_seconds, () => {
            loadingParentObj.SetActive(false);
            mini_Prefab_Loading.SetActive(false);
        }));
    }

    public void ShowStartLoading(float _seconds, System.Action callback = null)
    {
        GameObject prefab = CeateLoading(start_Prefab_Loading);
        prefab.SetActive(true);
        loadingParentObj.SetActive(true);
        StartCoroutine(UiController.Instance.WaitForSecond(_seconds, () => {
            prefab.GetComponent<CanvasGroupTransition>().FadeOut(() => {
                loadingParentObj.SetActive(false);
                Destroy(prefab.gameObject);
                if(callback != null) callback.Invoke();
            });
        }));
    }

    public void ShowBigLoading(float _seconds)
    {
        GameObject prefab = CeateLoading(big_Prefab_Loading);
        prefab.SetActive(true);
        loadingParentObj.SetActive(true);
        StartCoroutine(UiController.Instance.WaitForSecond(_seconds, () => {
            prefab.GetComponent<CanvasGroupTransition>().FadeOut(() => {
                loadingParentObj.SetActive(false);
                Destroy(prefab.gameObject);
            });
        }));
    }

    public void ShowGame2Loading(float _seconds)
    {
        GameObject prefab = CeateLoading(game2_Prefab_Loading);
        prefab.SetActive(true);
        loadingParentObj.SetActive(true);
        StartCoroutine(UiController.Instance.WaitForSecond(_seconds, () => {
            prefab.GetComponent<CanvasGroupTransition>().FadeOut(() => {
                loadingParentObj.SetActive(false);
                Destroy(prefab.gameObject);
            });
        }));
    }

    GameObject CeateLoading(GameObject _prefab)
    {
        UiController.Instance.HideChild(loadingParentObj);
        GameObject prefab = Instantiate(_prefab, Vector3.zero, Quaternion.identity);
        prefab.transform.SetParent(loadingParentObj.transform);
        RectTransform rectTransform = prefab.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
        return prefab;
    }

    public void StartGameButton(string _name)
    {
        //DataCenterManager.Instance.LoadSceneByName(_name);
        SceneManager.LoadScene(_name);
    }

    public void StartGameButtonIndex(int _index)
    {
        SceneManager.LoadScene(_index);
    }
}
