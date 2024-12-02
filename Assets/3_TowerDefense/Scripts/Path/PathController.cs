using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TowerDefense;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private Transform[] path;
    private List<Transform> allPaths = new List<Transform>();
    public List<Transform> backupPaths = new List<Transform>();

    public Transform[] GetPath()
    {
        var distinctPaths = allPaths.Distinct().ToList();
        path = distinctPaths.ToArray();
        return path;
    }

    public void ClearPath()
    {
        allPaths.Clear();
    }

    public void AddTransforms(Transform[] _path)
    {
        for (int i = 0; i < _path.Length; i++)
        {
            if (!allPaths.Contains(_path[i]))
            {
                allPaths.Add(_path[i]);
            }
        }
    }

    //แอดแบบมี Child
    public void AddPaths(GameObject _path)
    {
        Transform parentTransform = _path.transform;

        for (int i = 0; i < parentTransform.childCount; i++)
        {
            allPaths.Add(parentTransform.GetChild(i));
        }
    }

    //แอดแบบเดี่ยวๆ
    public void AddPath(Transform _path)
    {
        if(!allPaths.Contains(_path)) allPaths.Add(_path);
    }

    //ตัด path ที่มี PathAction Type นี้ออก
    public void PathRemoveAction(ActionEntity _pathAction)
    {
        if (allPaths.Count > 0)
        {
            if (allPaths.Count > 0)
            {
                List<Transform> pathsToRemove = new List<Transform>();

                allPaths.ForEach(o =>
                {
                    var pathComponent = o.GetComponent<PathAction>();
                    if (pathComponent != null && pathComponent.pathAction == _pathAction)
                    {
                        pathsToRemove.Add(o);
                    }
                });

                // ลบ GameObject ที่ต้องการลบหลังจากการวนลูปเสร็จ
                pathsToRemove.ForEach(o => allPaths.Remove(o));
            }
        }
    }
}
