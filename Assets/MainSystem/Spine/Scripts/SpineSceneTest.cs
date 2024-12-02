using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class SpineSceneTest : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] SpineSkinData spineSkinData;
    [SerializeField] SpineEntitySkin spineEntitySkin;
    [SerializeField] string[] heroLists;

    [Header("UI")]
    [SerializeField] TMPro.TextMeshProUGUI displayeNameTX;
    [SerializeField] GameObject parent_Hero;
    [SerializeField] GameObject parent_Animation;
    [SerializeField] GameObject buttonPrefab;

    public void Start()
    {
        spineSkinData.InitData();

        int index = 0;
        heroLists.ToList().ForEach(o => {
            GameObject hero = Instantiate(buttonPrefab);
            hero.transform.SetParent(parent_Hero.transform);
            hero.transform.localScale = Vector3.one;
            hero.name = "HERO" + (index+1);
            hero.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = hero.name;
            SpineSkinModelSO modelSO = spineSkinData.spineSkinDic[o];
            displayeNameTX.text = modelSO.displayName;

            hero.GetComponent<Button>().onClick.AddListener(() => {
                spineEntitySkin.ChangeSkin(modelSO);
                displayeNameTX.text = modelSO.displayName;
            });
            if (index == 0) hero.GetComponent<Button>().onClick.Invoke();

            index += 1;
        });

      
    }
}
