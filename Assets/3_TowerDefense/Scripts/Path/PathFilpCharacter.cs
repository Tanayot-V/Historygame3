using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum PathFilpType
    {
        Right,
        Left
    }
    
    public class PathFilpCharacter : MonoBehaviour
    {
        public PathFilpType pathFilpType;
    }
}
