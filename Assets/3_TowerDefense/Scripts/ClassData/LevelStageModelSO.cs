using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefense
{
    [CreateAssetMenu(fileName = "LevelStageModelSO", menuName = "TowerDefenese/LevelStageModelSO", order = 1)]
    public class LevelStageModelSO : ScriptableObject
    {
        public LevelStageModel levelStageModel;

        public BarrackModel GetBarrackModel(BarrackType _barrackType)
        {
            BarrackModel barrackModel = null;
            levelStageModel.barrackModelSO.ToList().ForEach(o => {
                if (o.barrackModel.barrackType == _barrackType)
                {
                    barrackModel = o.barrackModel;
                }
            });

            return barrackModel;
        }
    }
}
