using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System.Linq;


[CreateAssetMenu(fileName = "SpineSkinModelSO", menuName = "Main System/SpineSkinModelSO")]
public class SpineSkinModelSO : ScriptableObject
{
    public string id;
    public string displayName;
    public GameObject prefab;
    public string[] paths;
    public SpineAnimation[] animations;

    [System.Serializable]
    public struct SpineAnimation
    {
        public SpineAnimationType type;
        public string animationName;
    }

    public enum SpineAnimationType
    {
        Idle,
        Run,
        Attack,
        Dead,
        Hit,
        Defensive
    }

    public bool IsSpineID(string _id)
    {
        return id == _id;
    }

    #region SpineAnimation
    public string GetSpineAnimationPath(SpineAnimationType _type)
    {
        string path = string.Empty;
        animations.ToList().ForEach(o => {
            if (_type == o.type)
            {
                path = o.animationName;
            }
        });

        if (path == string.Empty)
        {
            path = animations[0].animationName;
            Debug.Log("Path animation is not founds.");
        }
        return path;
    }

    public SpineAnimation GetSpineAnimationType(string _name)
    {
        SpineAnimation _type = animations[0];
        animations.ToList().ForEach(o => {
            if (_name == o.animationName)
            {
                _type = o;
            }
        });
        return _type;
    }
    #endregion
}
