using SNShien.Common.ProcessTools;

namespace GameCore
{
    public interface IBeaterModel : IArchitectureModel
    {
        float GetNextBeatTiming();
        void TriggerHalfBeat();
        BeatAccuracyResult DetectBeatAccuracy(float currentTime);
        BeatAccuracyResult DetectBeatAccuracyCurrentTime();
    }
}