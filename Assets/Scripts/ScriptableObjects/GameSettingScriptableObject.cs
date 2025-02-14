using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class GameSettingScriptableObject : SerializedScriptableObject, IGameSetting
    {
        [SerializeField] private ScoreBallTextColorSettingScriptableObject scoreBallTextColorSetting;
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private int successSettleScore;
        [SerializeField] private int spawnCatchNetFreq;
        [SerializeField] private int catchNetLimit;
        [SerializeField] private Vector2Int catchNetNumberRange;
        [SerializeField] private float hpMax;
        [SerializeField] private float accuracyPassThreshold;
        [SerializeField] private int feverEnergyIncrease;
        [SerializeField] private int feverEnergyDecrease;
        [SerializeField] private int[] feverEnergyBarSetting;
        [SerializeField] private Dictionary<int, Dictionary<int, int>> scoreBallFlagWeightSettingDict;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public int SuccessSettleScore => successSettleScore;
        public int SpawnCatchNetFreq => spawnCatchNetFreq;
        public int CatchNetLimit => catchNetLimit;
        public Vector2Int CatchNetNumberRange => catchNetNumberRange;
        public float HpMax => hpMax;
        public IScoreBallTextColorSetting ScoreBallTextColorSetting => scoreBallTextColorSetting;
        public float AccuracyPassThreshold => accuracyPassThreshold;
        public int FeverEnergyIncrease => feverEnergyIncrease;
        public int FeverEnergyDecrease => feverEnergyDecrease;
        public int[] FeverEnergyBarSetting => feverEnergyBarSetting;

        public Dictionary<int, int> GetScoreBallFlagWeightSetting(int feverStage)
        {
            return scoreBallFlagWeightSettingDict.TryGetValue(feverStage, out Dictionary<int, int> result) ?
                result :
                new Dictionary<int, int>();
        }
    }
}