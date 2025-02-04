namespace GameCore
{
    public interface IBeaterView
    {
        float CurrentTimer { get; }
        void SetBeatHintActive(bool isActive);
        void SetHalfBeatTimeOffset(float halfBeatTimeOffset);
        void ClearHalfBeatEventTimer();
        void PlayBeatAnimation();
    }
}