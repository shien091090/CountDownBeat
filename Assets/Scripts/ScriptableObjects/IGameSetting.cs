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
    }
}