using SNShien.Common.MonoBehaviorTools;

namespace GameCore
{
    public interface IBeaterView : IArchitectureView
    {
        void SetBeatHintActive(bool isActive);
        void PlayBeatAnimation();
    }
}