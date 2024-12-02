using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        public List<WalkPath> walkPaths;
        public int currentwalkPathIndex;

        public Color debugLineColor;

        public GameObject incomingIcon;

        public void ShowIncomingIcon(bool isShow = true)
        {
            incomingIcon.SetActive(isShow);
        }

        public WalkPath GetRandomWalkPath()
        {
            int r = Random.Range(0,walkPaths.Count);
            currentwalkPathIndex = r;
            return walkPaths[r];
        }

        public WalkPath GetContinueWalkPath()
        {
            return walkPaths[currentwalkPathIndex];
        }

        public WalkPath GetExceptPreviousWalkPath()
        {
            if(walkPaths.Count < 2) return walkPaths[0];
            int r = Random.Range(0,walkPaths.Count);
            while (r == currentwalkPathIndex)
            {
                r = Random.Range(0,walkPaths.Count);
            }
            currentwalkPathIndex = r;
            return walkPaths[r];
        }
        public void OnDrawGizmosSelected()
        {
            for (int i = 0; i < walkPaths.Count; i++)
            {
                for (int j = 0; j + 1 < walkPaths[i].waypoints.Count; j++)
                {
                    Gizmos.color = debugLineColor;
                    Gizmos.DrawLine(walkPaths[i].waypoints[j].position, walkPaths[i].waypoints[j+1].position);
                }
            }
        } 
    }

    [System.Serializable] public class WalkPath
    {
        public List<Transform> waypoints;
    }  
}