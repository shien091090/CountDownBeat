using System.Collections.Generic;

namespace GameCore
{
    public interface IStageSettingContent
    {
        int ScoreBallStartCountDownValue { get; }
        int SuccessSettleScore { get; }
        Dictionary<int, int> CatchNetLimitByFeverStageSetting { get; }
        int[] FeverEnergyBarSetting { get; }
        int FeverEnergyIncrease { get; }
        int FeverEnergyDecrease { get; }
        List<int> SpawnBeatIndexList { get; }
        int CountDownBeatFreq { get; }
        string AudioKey { get; }
        float HpDecreasePerSecond { get; }
        float HpIncreasePerCatch { get; }
        Dictionary<int, int> GetScoreBallFlagWeightSetting(int feverStage);
        void SetHpDecreasePerSecond(float decreaseValue);
        void SetHpIncreasePerCatch(float increaseValue);
    }
}