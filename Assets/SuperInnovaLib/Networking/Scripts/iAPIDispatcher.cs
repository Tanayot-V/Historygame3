using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class iAPIDispatcher : MonoBehaviour
{
    public int id;

    private static Dictionary<int, iAPIDispatcher> allDispatchers = new Dictionary<int, iAPIDispatcher>();

    public static iAPIDispatcher Create()
    {
        GameObject obj = new GameObject();
        int id = GenerateID();
        obj.name = "Dispatcher: " + id;
        iAPIDispatcher apid = obj.AddComponent<iAPIDispatcher>();
        apid.id = id;
        allDispatchers[id] = apid;

        return apid;
    }

    public static iAPIDispatcher Current()
    {
        if (allDispatchers.Count == 0)
        {
            return Create();
        }

        foreach(var kv in allDispatchers)
        {
            return kv.Value;
        }

        return Create();
    }


    public static void Dispose(int key)
    {
        if (allDispatchers.ContainsKey(key))
        {
            DestroyImmediate(allDispatchers[key].gameObject);
            allDispatchers.Remove(key);
        }
    }


    public static void DisposeAll()
    {
        foreach(var kv in allDispatchers)
        {
            DestroyImmediate(kv.Value.gameObject);
        }
        allDispatchers.Clear();
    }


    private static int GenerateID()
    {
        Clean();
        int count = 0;
        while(allDispatchers.Any( o => o.Value.id == count))
        {
            count++;
        }

        return count;
    }

    private static void Clean()
    {
        List<int> emptyKey = new List<int>();
        foreach(var kv in allDispatchers)
        {
            if (kv.Value == null)
            {
                emptyKey.Add(kv.Key);
            }
        }

        emptyKey.ForEach( o => allDispatchers.Remove(o));
    }
}
