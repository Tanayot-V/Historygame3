using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    [System.Serializable]
    public class BarrackState
    {
        public int level;
        public BarrackType barrackType;
        public int soldierCurrentStack;
        public int soldierMaxStack; //จาก model

        public void SetBarrackType(BarrackType _barrackType)
        {
            barrackType = _barrackType;
        }
    }
}
