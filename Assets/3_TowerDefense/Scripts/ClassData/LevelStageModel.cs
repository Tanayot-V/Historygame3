using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public enum BaseType
    {
        Offensive,  // ฐานบุก
        Defensive   // ฐานตั้งรับ
    }

    [System.Serializable]
    public class StageDetail
    {
        public string mission;
        public string win;
        public string lose;
        public string trivia; //เกร็ดความรู้
        public string[] popups;
    }

    public enum LevelType
    {
        None,
        Level1Tutorial,
        Level1Play,
        Level2,
        Level3,
        Level4,
        Level5,
        Level6,
        Level7
    }

    [System.Serializable]
    public class LevelStageModel
    {
        public string levelID;
        public LevelType levelType;
        public BaseType baseType;
        public StageDetail stageDetail;
        public BarrackModelSO[] barrackModelSO;

        [Header("Enemy")]
        public List<WaveData> enemyWaves;
        // public List<EnemySpwanModelList> enemySpwanModelList;
        // public EnemySpwanModel[] enemySpwanModels;
        // public EnemySpwanModel[] enemySpwanModels_B;
    }
}
