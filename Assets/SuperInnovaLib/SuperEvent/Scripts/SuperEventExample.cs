using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Animal
{
    CAT,
    DOG,
    FISH
}

public class SuperEventExample : MonoBehaviour
{
    public UnityEvent unityEvent;
    public SuperEvent superEvent;

    private void Start()
    {
        superEvent.Invoke(this);
    } 


    [SECustomProperty]
    public void ClickEnum(Animal animal)
    {
        Debug.Log(animal);
    }


    public void Play(ScriptableObject  obj)
    {

    }
}
