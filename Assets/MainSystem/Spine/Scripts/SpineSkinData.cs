using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpineSkinData : MonoBehaviour
{
    public SpineSkinAllModelSO spineSkinAllModelSO;
    public Dictionary<string, SpineSkinModelSO> spineSkinDic = new Dictionary<string, SpineSkinModelSO>();

    public void InitData()
    {
        spineSkinDic = spineSkinAllModelSO.spineSkinModelSOs.ToDictionary(skin => skin.id, skin => skin);
    }
}
