using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MadEventAction
{
    public GameObject handlerObject;
    public UnityEngine.Object manualClassObject;
    public int currentIndex;
    public string methodName;
    public string fullMethodName;
    public float delay;
    public bool isDelay;
    public bool isManualAssigned;

    public int paramInt;
    public float paramFloat;
    public bool paramBoolean;
    public string paramString;
    public Vector3 paramVector3;
    public Vector2 paramVector2;
    public Color paramColor;

    public UnityEngine.Object paramObject;
    public string customType = "";
    public GameObject paramGameObject;
    public Transform paramTransform;

    public string paramType;
    public bool isCustomParam = false;
}

[System.Serializable]
public class SuperEvent
{
    [SerializeField]
    public List<MadEventAction> handlerEvent;
    public bool showing_attr = false;

    public int count
    {
        get
        {
            return handlerEvent.FindAll( o => !o.isDelay ).Count;
        }
    }

    public void Invoke<T>(MonoBehaviour mono, T parameter)
    {
        if(mono.gameObject.activeInHierarchy)
        {
            mono.StartCoroutine(ThreadInvoker(parameter));
        }
    }

    public void Invoke(MonoBehaviour mono)
    {
        if (mono.gameObject.activeInHierarchy)
        {
            mono.StartCoroutine(ThreadInvoker(0));
        }
    }

    public void AddEventListener<T>(UnityAction<T> action)
    {
        MethodInfo info = action.GetMethodInfo();

        ParameterInfo[] param = info.GetParameters();
        if(param.Length > 1)
        {
            return;
        }

        MadEventAction mea = new MadEventAction();
        mea.isManualAssigned = true;

        var _type = info.DeclaringType;
        mea.methodName = _type.Name + "/" + info.Name + " (" + param[0].ParameterType.Name + ")";
        mea.fullMethodName = _type.FullName + "/" + info.Name + "(" + param[0].ParameterType + ")";
        mea.manualClassObject = action.Target as UnityEngine.Object;

        handlerEvent.Add(mea);
    }

    public void AddDelay(float second)
    {
        MadEventAction mea = new MadEventAction();
        mea.isDelay = true;
        mea.delay = second;
        mea.isManualAssigned = true;
        handlerEvent.Add(mea);
    }

    private IEnumerator ThreadInvoker<T>(T parameter)
    {
        foreach(var evt in handlerEvent)
        {
            if(evt.isDelay)
            {
                yield return new WaitForSeconds(evt.delay);
            }
            else
            {
                if(!evt.methodName.Contains("/")) continue;

                if(!evt.methodName.StartsWith("MadEvent.Extra"))
                {
                    MethodInfo methodInfo = GetMethodInfo(evt.fullMethodName);
                    object classObject = evt.isManualAssigned ? evt.manualClassObject : GetClassObject(evt);

                    if (methodInfo != null)
                    {
                        if (methodInfo.GetParameters().Length > 0)
                        {
                            if (!evt.isCustomParam)
                            {
                                try
                                {
                                    methodInfo.Invoke(classObject, new object[] { parameter });
                                }
                                catch (ArgumentException)
                                {
                                    methodInfo.Invoke(classObject, new object[1]);
                                }
                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                            else
                            {
                                Type paramType = GetTypeSelf(evt.paramType);
                                if (paramType == typeof(int))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramInt });
                                }
                                else if (paramType == typeof(float))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramFloat });
                                }
                                else if (paramType == typeof(bool))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramBoolean });
                                }
                                else if (paramType == typeof(string))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramString });
                                }
                                else if (paramType == typeof(GameObject))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramGameObject });
                                }
                                else if (paramType == typeof(Transform))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramTransform });
                                }
                                else if (paramType == typeof(Vector3))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramVector3 });
                                }
                                else if (paramType == typeof(Vector2))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramVector2 });
                                }
                                else if (paramType == typeof(Color))
                                {
                                    methodInfo.Invoke(classObject, new object[] { evt.paramColor });
                                }
                                else if (paramType == null)
                                {
                                    Type customType = GetTypeSelf(evt.customType);
                                    if (customType != null)
                                    {
                                        if (evt.paramObject != null)
                                        {
                                            methodInfo.Invoke(classObject, new object[] { evt.paramObject });
                                        }
                                        else if (customType.IsEnum)
                                        {
                                            methodInfo.Invoke(classObject, new object[] { Enum.Parse(customType, evt.paramString) });
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            methodInfo.Invoke(classObject, new object[] { });
                        }
                    }
                }
                else
                {
                    string methodName = evt.methodName.Split('/')[1];

                    if(methodName.StartsWith("SetActive"))
                    {
                        evt.handlerObject.SetActive(evt.paramBoolean);
                    }
                    else if(methodName.StartsWith("SetPosition(" + typeof(Transform).GetTypeInfo() + ")"))
                    {
                        if(evt.paramTransform != null)
                        {
                            evt.handlerObject.transform.position = evt.paramTransform.position;
                        }
                    }
                    else if(methodName.StartsWith("SetPosition(" + typeof(Vector3).GetTypeInfo() + ")"))
                    {
                        evt.handlerObject.transform.position = evt.paramVector3;
                    }
                    else if(methodName.StartsWith("SetRotation(" + typeof(Transform).GetTypeInfo() + ")"))
                    {
                        if(evt.paramTransform != null)
                        {
                            evt.handlerObject.transform.rotation = evt.paramTransform.rotation;
                        }
                    }
                    else if(methodName.StartsWith("SetPositionAndRotation(" + typeof(Transform).GetTypeInfo() + ")"))
                    {
                        if(evt.paramTransform != null)
                        {
                            evt.handlerObject.transform.position = evt.paramTransform.position;
                            evt.handlerObject.transform.rotation = evt.paramTransform.rotation;
                        }
                    }
                    else if(methodName.StartsWith("Translate(" + typeof(Vector3).GetTypeInfo() + ")"))
                    {
                        evt.handlerObject.transform.Translate(evt.paramVector3);
                    }
                    else if(methodName.StartsWith("Destroy"))
                    {
                        GameObject.Destroy(evt.handlerObject);
                    }
                    else if(methodName.StartsWith("Debug(" + typeof(string) + ")"))
                    {
                        Debug.Log(evt.paramString);
                    }
                }
            }
        }
    }

    private MethodInfo GetMethodInfo(string methodName)
    {
        string[] methodSplit = methodName.Split('/');
        string className = methodSplit[0];
        string[] actualMethodName = methodSplit[1].Replace(")", "").Split('(');

        Type type = GetTypeSelf(className);
        MethodInfo info = null;
        if(actualMethodName[1] == "")
        {
           info = type.GetMethod(actualMethodName[0], new Type[0]); 
        }
        else
        {
           info = type.GetMethod(actualMethodName[0], new Type[] { GetTypeSelf(actualMethodName[1]) }); 
        }

        return info;
    }

    private Component GetClassObject(MadEventAction eventAction)
    {
        string methodName = eventAction.methodName;
        if (!methodName.Contains("/")) return null;
        if (eventAction.handlerObject == null) return null;

        string[] methodSplit = methodName.Split('/');
        return eventAction.handlerObject.GetComponent(methodSplit[0]);
    }

    public Type GetTypeSelf( string TypeName )
    {
        var type = Type.GetType( TypeName);
        if( type != null ) return type;

        type = Type.GetType(TypeName + ", Assembly-CSharp");
        if( type != null ) return type;

        type = Type.GetType("UnityEngine." + TypeName + ", UnityEngine");
        if( type != null ) return type;

        type = Type.GetType("UnityEngine.UI." + TypeName + ", UnityEngine.UI");
        if( type != null ) return type;

        return Type.GetType(TypeName + ", UnityEngine");
    }
}
