namespace GameCore
{
    public interface IGameSetting
    {
        int ScoreBallStartCountDownValue { get; }
        float ScoreBallSpawnBeatFreq { get; }
        float BeatTimeThreshold { get; }
        int SuccessSettleScore { get; }
        int SpawnCatchNetFreq { get; }
        int CatchNetLimit { get; }
    }
}