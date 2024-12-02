using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseClass
{
    public int baseA;
    public int baseB;

    public virtual void Print()
    {
        Debug.Log("Base");
    }
}


public class ChildA: BaseClass
{
    public int childA;

    public override void Print()
    {
        Debug.Log("Child A");
    }
}


public class ChildB: BaseClass
{
    public string childB;

    public override void Print()
    {
        Debug.Log("Child B");
    }
}


public class JsonTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        JsonConvert.DefaultSettings = () => settings;

        string json = JsonConvert.SerializeObject( new ChildA() 
        {
            baseA = 1,
            baseB = 2,
            childA = 10
        });

        BaseClass baseClass = JsonConvert.DeserializeObject<BaseClass>(json);
        baseClass.Print();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
