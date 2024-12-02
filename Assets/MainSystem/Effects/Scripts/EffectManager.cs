using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager _Instance;
    public static EffectManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("[Script]: -- EffectManager Instance --");
                _Instance = obj.AddComponent<EffectManager>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }

    [SerializeField]private EffectModelSO effectModelSO;
    public void Init(EffectModelSO _effectModelSO)
    {
        effectModelSO = _effectModelSO;
    }

    void Update()
    {
        //Instantiate();
    }

    public void Instantiate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f; // กำหนดระยะห่างจากกล้อง
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Instantiate(effectModelSO.GetEffectModel("Click").prefab, worldPosition, Quaternion.identity);
        }
    }
}
