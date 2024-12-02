using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum ClickType
    {
        Fortress,
        Barrack,
        TownEnemy,
        TownEntity
    }

    public class ObjectClick : MonoBehaviour
    {
        public ClickType clickType;
    }
}
