using System.Collections.Generic;

namespace GameCore
{
    public interface IStageSettingContent
    {
        int ScoreBallStartCountDownValue { get; }
        int SuccessSettleScore { get; }
        List<CatchNetLimitByFeverStageSetting> CatchNetLimitByFeverStageSettings { get; }
        int[] FeverEnergyPhaseSettings { get; }
        int FeverEnergyIncrease { get; }
        int FeverEnergyDecrease { get; }
        List<int> SpawnBeatIndexList { get; }
        int CountDownBeatFreq { get; }
        string AudioKey { get; }
        float HpDecreasePerSecond { get; }
        float HpIncreasePerCatch { get; }
        ICatchFlagMergeSetting FlagMergeSetting { get; }
        List<ScoreBallFlagWeightDefine> GetScoreBallFlagWeightSetting(int feverStage);
        void SetHpDecreasePerSecond(float decreaseValue);
        void SetHpIncreasePerCatch(float increaseValue);
    }
}