using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AroundBarrack : Singletons<AroundBarrack>
{
    public List<Transform> pointAroundTheBarrack;
    
    public Transform GetPointAroundBarrack()
    {
        int minChildCount = int.MaxValue;
        int minIndex = -1;
        for (int i = 0; i < pointAroundTheBarrack.Count; i++)
        {
            if(pointAroundTheBarrack[i].childCount < minChildCount)
            {
                minChildCount = pointAroundTheBarrack[i].childCount;
                minIndex = i;
            }
        }
        return pointAroundTheBarrack[minIndex];
    }

    public Transform GetRandomPointAroundBarrack()
    {
        int r = Random.Range(0,pointAroundTheBarrack.Count);
        return pointAroundTheBarrack[r];
    }
}
