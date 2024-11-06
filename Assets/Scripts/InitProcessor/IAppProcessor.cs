using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IAppProcessor : IArchitectureModel
    {
        StageSettingContent CurrentStageSettingContent { get; }
        void SetEnterStageAudioKey(string audioKey);
    }
}