using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IAppProcessor : IArchitectureModel
    {
        void SetEnterStageAudioKey(string audioKey);
    }
}