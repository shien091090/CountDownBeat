using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameCore
{
    [CreateAssetMenu]
    public class GameSettingScriptableObject : SerializedScriptableObject, IGameSetting
    {
        [Header("分數球")] [SerializeField] private ScoreBallTextColorSettingScriptableObject scoreBallTextColorSetting;
        [SerializeField] private int scoreBallStartCountDownValue;
        [SerializeField] private Dictionary<int, Dictionary<int, int>> scoreBallFlagWeightSettingDict;

        [Header("捕獲網")] [SerializeField] private int successSettleScore;
        [SerializeField] private Vector2Int catchNetNumberRange;
        [SerializeField] private Dictionary<int, int> catchNetLimitByFeverStageSetting;

        [Header("血條")] [SerializeField] private float hpMax;

        [Header("拍點準度判定")] [SerializeField] private float accuracyPassThreshold;

        [Header("Fever能量條")] [SerializeField] private int feverEnergyIncrease;
        [SerializeField] private int feverEnergyDecrease;
        [SerializeField] private int[] feverEnergyBarSetting;

        public int ScoreBallStartCountDownValue => scoreBallStartCountDownValue;
        public int SuccessSettleScore => successSettleScore;
        public Vector2Int CatchNetNumberRange => catchNetNumberRange;
        public float HpMax => hpMax;
        public IScoreBallTextColorSetting ScoreBallTextColorSetting => scoreBallTextColorSetting;
        public float AccuracyPassThreshold => accuracyPassThreshold;
        public int FeverEnergyIncrease => feverEnergyIncrease;
        public int FeverEnergyDecrease => feverEnergyDecrease;
        public int[] FeverEnergyBarSetting => feverEnergyBarSetting;
        public Dictionary<int, int> CatchNetLimitByFeverStageSetting => catchNetLimitByFeverStageSetting;

        public Dictionary<int, int> GetScoreBallFlagWeightSetting(int feverStage)
        {
            if (scoreBallFlagWeightSettingDict.TryGetValue(feverStage, out Dictionary<int, int> result))
            {
                return result ?? new Dictionary<int, int>();
            }
            else
                return new Dictionary<int, int>();
        }
    }
}