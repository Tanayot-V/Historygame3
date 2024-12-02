using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletons<T> : MonoBehaviour where T : MonoBehaviour {

    public static T Instance{
        get{
            if(instance == null){
                instance = (T)FindObjectOfType(typeof(T));
                if(instance == null){
                    instance = CreateInstance();
                }
            }
            return instance;
        }
    }

    private static T instance;

    public static bool InstanceExist{
        get{
            if (instance == null) {

                instance = (T)FindObjectOfType(typeof(T));
            }
            return instance != null;
            
        }
    }

    public static T CreateInstance(){
        GameObject parentUI = GameObject.FindGameObjectWithTag("UI");
        return parentUI.AddComponent<T>();
    }

    // Use this for initialization
    protected virtual void Awake () {
        if(instance != null){
            Destroy(gameObject);

        }

    }


}
