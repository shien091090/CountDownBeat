namespace GameCore
{
    public interface IAppProcessor
    {
        StageSettingContent CurrentStageSettingContent { get; }
        void SetEnterStageAudioKey(string audioKey);
    }
}