using System.Collections.Generic;
using UnityEngine;

namespace GameCore
{
    public interface IGameSetting
    {
        int ScoreBallStartCountDownValue { get; }
        int SuccessSettleScore { get; }
        Vector2Int CatchNetNumberRange { get; }
        float HpMax { get; }
        IScoreBallTextColorSetting ScoreBallTextColorSetting { get; }
        float AccuracyPassThreshold { get; }
        int FeverEnergyIncrease { get; }
        int FeverEnergyDecrease { get; }
        int[] FeverEnergyBarSetting { get; }
        Dictionary<int, int> CatchNetLimitByFeverStageSetting { get; }
        Dictionary<int, int> GetScoreBallFlagWeightSetting(int feverStage);
    }
}