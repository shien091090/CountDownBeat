using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IBeaterModel : IArchitectureModel
    {
        void TriggerHalfBeat();
        BeatAccuracyResult DetectBeatAccuracy(float currentTime);
        BeatAccuracyResult  DetectBeatAccuracyCurrentTime();
    }
}