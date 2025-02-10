using UnityEngine;

namespace GameCore
{
    public interface IGameSetting
    {
        int ScoreBallStartCountDownValue { get; }
        int SuccessSettleScore { get; }
        int SpawnCatchNetFreq { get; }
        int CatchNetLimit { get; }
        Vector2Int CatchNetNumberRange { get; }
        float HpMax { get; }
        IScoreBallTextColorSetting ScoreBallTextColorSetting { get; }
        float AccuracyPassThreshold { get; }
        int FeverEnergyIncrease { get; }
        int FeverEnergyDecrease { get; }
        int[] FeverEnergyBarSetting { get; }
    }
}