namespace GameCore
{
    public interface IGameSetting
    {
        int ScoreBallStartCountDownValue { get; }
        float ScoreBallSpawnBeatFreq { get; }
        float BeatTimeThreshold { get; }
    }
}