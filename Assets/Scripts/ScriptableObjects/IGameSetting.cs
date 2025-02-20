namespace GameCore
{
    public interface IGameSetting
    {
        float HpMax { get; }
        IScoreBallTextColorSetting ScoreBallTextColorSetting { get; }
        IScoreBallFrameColorByFlagSetting ScoreBallFrameColorByFlagSetting { get; }
        float AccuracyPassThreshold { get; } //數字越大判定越簡單
    }
}