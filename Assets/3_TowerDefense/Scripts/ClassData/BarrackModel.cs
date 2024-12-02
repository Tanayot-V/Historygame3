using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefense
{
    public enum BarrackType
    {
        Empty,
        SoldierGun,
        SoldierSword,
        Merchant,
        Monk
    }

    [System.Serializable]
    public class FortressUpgrade
    {
        public string id;
        public string fortressName;
        public FortressUpgradeType upgradeType;
        public Sprite fortressUISP;
        public Sprite[] fortressFullSP;
        public int goldcost;
    }

    [System.Serializable]
    public class BarrackModel
    {
        public string barrackID;
        public string displayName;
        public BarrackType barrackType;
        public Sprite characterFullSP;

        [Header("Fortress")]
        public string fortressName;
        public Sprite fortressUISP;
        public Sprite[] fortressFullSP;
        public int costFortress;
        public FortressUpgrade[] fortressUpgrades;
        //price upsize, price effects

        [Header("Characters")]
        public GameObject general;
        public Sprite sprite_FullChar;
        public GameObject[] prefabs;
    }

    [System.Serializable]
    public class BarrackUpgrade
    {
        public string id;
        public int level;
        public BarrackType barrackType;
        public SpineSkinModelSO spineSkinModelSO;
        public SpineSkinModelSO spineSkinModelSOBetrayal;
        public Sprite sprite;
        public Sprite characterSP;
        public EntityStatus[] entityStatus; //baseStatus
        public float respawnTime;
        public int costUpgrade;
    }

    [System.Serializable]
    public class EntityStatus
    {
        public string id;
        public EntityComboType entityCombo;
        public float hp;
        public float dmg;
        public float moveSpd;
        public float atkSpd;
        public float genCost;

        public EntityStatus(string _id, EntityComboType _entityCombo, float _hp, float _dmg, float _moveSpd, float _atkSpd)
        {
            id = _id;
            entityCombo = _entityCombo;
            hp = _hp;
            dmg = _dmg;
            moveSpd = _moveSpd;
            atkSpd = _atkSpd;
        }

        // Methods
        public EntityStatus Clone()
        {
            return new EntityStatus(id, entityCombo,hp, dmg, moveSpd, atkSpd);
        }

        public EntityStatus ScaleStats(float hpMultiplier, float dmgMultiplier)
        {
            return new EntityStatus(id, entityCombo, hp * hpMultiplier, dmg * dmgMultiplier, moveSpd, atkSpd);
        }
    }

    [System.Serializable]
    public enum EntityComboType
    {
        Combo1,
        Combo2,
        Combo3,
        Combo4,
        Combo5,
        Combo6,
        Combo7
    }
}
