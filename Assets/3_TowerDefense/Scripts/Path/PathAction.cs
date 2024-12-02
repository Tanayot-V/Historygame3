using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum ActionEntity
    {
        Run,
        InFotress,
        AttackEnemy,
        AttackTower,
        BackBarrack,
        Idle,
        Defensive,
        FontFortress,
        AttackDefensive, //ตีหน้าป้อม ป้องกันป้อมตัวเอง
        InBarrack,
        EndGame,
        TownShip
    }
    public class PathAction : MonoBehaviour
    {
        public ActionEntity pathAction;
    }
}
