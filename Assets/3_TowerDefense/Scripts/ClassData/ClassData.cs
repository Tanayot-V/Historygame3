using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class ClassData : MonoBehaviour
    {
        
    }

    #region Enemy
    [System.Serializable]
    public class EnemyStatusModel
    {
        public string id;
        public EnemyType enemyType;
        public SpineSkinModelSO spineSkinModelSO;
        public EnemyStatus[] status;
    }

    [System.Serializable]
    public class EnemyStatus
    {
        public string id;
        public EntityComboType comboType;
        public float hp;
        public float dmg;
        public float moveSpd;
        public float atkSpd;
        public int gold;
    }

    public enum EnemyType
    {
        Sword,
        Shield,
        Gun,
        General,
        Betroyal,
        Marchant,
        Monk,
        Cavalry
    }
    #endregion
}

